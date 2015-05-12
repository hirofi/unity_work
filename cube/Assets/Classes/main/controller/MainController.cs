using UnityEngine;
using System.Collections;
using System.Collections.Generic;



// ------------
// DataManager
// ------------
public class DataManager : EventManagerDynamic {

	private static DataManager s_Instance = null;

	public static DataManager Instance {
		get {
			if (s_Instance == null) {
				s_Instance = new DataManager();
			}
			return s_Instance;
		}
	}

}

public class GameEventDataAccess : GameEventDynamic
{
	public enum enmDataType{
		DATA_TEXT = 0,
		DATA_BINARY ,
		ENTRY_MAX
	}
	
	public enum enmDataAccessReadWrite{
		DATA_ACCESS_READ = 0,
		DATA_ACCESS_WRITE ,
		DATA_ACCESS_APPEND ,
		ENTRY_MAX
	}

	private GameObject m_game_object;
	public GameObject p_GameObject{
		get { return m_game_object;	}
		set { m_game_object = value;}
	}

	private enmDataType m_data_type;
	private enmDataAccessReadWrite m_access_read_write;

	public enmDataType p_DataType{
		get { return m_data_type;	}
		set { m_data_type = value;}
	}

	public enmDataAccessReadWrite p_AccessReadWrite{
		get { return m_access_read_write;	}
		set { m_access_read_write = value;}
	}

}



// --------
// ERROR MANAGER
// --------
public class ErrorManager : EventManagerDynamic
{

	private static ErrorManager s_Instance = null;

	public static ErrorManager Instance {
		get {
			if (s_Instance == null) {
				s_Instance = new ErrorManager();
			}
			return s_Instance;
		}
	}
}

public class GameEventError : GameEventDynamic
{
	private int m_error_code;
	public int p_ErrorCode{
		get { return m_error_code;	}
		set { m_error_code = value;	}
	}

	private string m_error_class;
	public string p_ErrorClass{
		get { return m_error_class;	}
		set { m_error_class = value;	}
	}

	private string m_error_function;
	public string p_ErrorFuntion{
		get { return m_error_function;	}
		set { m_error_function = value;	}
	}

	private int m_error_line;
	public int p_ErrorLine{
		get { return m_error_line;	}
		set { m_error_line = value;	}
	}

}



public class MainController : MonoBehaviour {

	EventManagerDynamic m_download_event_manager = null;
	DownloadController m_down_load = null;

	EventManagerDynamic m_list_download_event_manager = null;
	DownloadController m_list_down_load = null;


	MapController m_map = null;
	TextMesh m_progress_tm = null;

	GameObject[] m_gm_objects;
	int m_gm_object_count;



	// Use this for initialization
	void Start () {
		m_now_sequence = enmSequence.INITIALIZE;
	}
	
	// Update is called once per frame
	void Update () {
		Sequence ();
	}

	MainController()
	{
		DataManager.Instance.AddListener<GameEventDataAccess> (OnDataAccessCompleate);
		ErrorManager.Instance.AddListener<GameEventError> (OnError);
	}

	// -------------
	// シーケンス切り替え処理
	// -------------

	enum enmSequence{
		NOP = 0,			// 処理なし
		INITIALIZE,			//　初期化処理
		LOAD_DOWNLOAD_LIST, // ダウンロード対象のアセット一覧取得
		LOAD_ASSET,			// アセットダウンロード
		ENTRY_MAX
	}

	private static enmSequence m_now_sequence;
	void Sequence()
	{
		switch (m_now_sequence) {
		case enmSequence.NOP:
			break;
		case enmSequence.INITIALIZE:
			m_now_sequence = enmSequence.NOP;
			Initialize(enmInitializeMode.CREATE_INSTANCE);
			break;
		case enmSequence.LOAD_DOWNLOAD_LIST:
			m_now_sequence = enmSequence.NOP;
			DoDownloadList();

			break;
		case enmSequence.LOAD_ASSET:
			m_now_sequence = enmSequence.NOP;
			m_down_load.StartDownloadAssetBandles ();

			break;
		
		}
	}

	// -------------
	// 初期化処理
	// -------------
	enum enmInitializeMode
	{
		CREATE_INSTANCE = 0,
		REQUEST_DOWNLOAD_LIST,
		ENTRY_MAX
	}
	
	void Initialize( enmInitializeMode aMode )
	{
		
		switch( aMode )
		{
		case enmInitializeMode.CREATE_INSTANCE:
			m_now_sequence = enmSequence.LOAD_DOWNLOAD_LIST;
			break;
			
		case enmInitializeMode.REQUEST_DOWNLOAD_LIST:
			if( m_down_load == null)
				break;
			
			break;
		}
	}

	// -------------
	// ダウンロード対象のアセット一覧取得開始処理
	// -------------
	void DoDownloadList()
	{
		m_list_download_event_manager = new EventManagerDynamic ();
		m_list_download_event_manager.AddListener<DownloadEventController> (OnCompleateListDownload);
		
		m_list_down_load = new DownloadController ();
		
		m_list_down_load.p_DomainName = "http://210.140.154.119:82/ab/";
		m_list_down_load.p_FileName = "dllist.txt";
		m_list_down_load.SetEventManager (m_list_download_event_manager);
		m_list_down_load.StartDownload ();
	}

	// -------------
	// ダウンロード対象のアセット一覧取得完了処理
	// -------------
	public void OnCompleateListDownload( DownloadEventController aEvent )
	{
		// ダウンロードリスト取得完了時
		if (aEvent.p_DownloadFileType != DownloadEventController.enmDownloadFileType.DOWNLOAD_ORDER_LIST)
			return;

		foreach ( ContentInformation content_info in aEvent.p_RequestContents)
		{
			if( content_info.p_StringData != null )
			{
				string[] file_name_list = content_info.p_StringData.Split (
					new char[]{'\n','\r'},
				System.StringSplitOptions.RemoveEmptyEntries);
				DoDownloadAsset( file_name_list );
			}

			// 1リストのみ対応のため、ここでブレイク
			break;
		}

		// 次回イベント発行の為にここでクリア
		aEvent = null;

		// シーケンスをアセットバンドルの取得に移行
		m_now_sequence = enmSequence.LOAD_ASSET;

	}

	// -------------
	// アセットダウンロード開始処理
	// -------------
	void DoDownloadAsset( string[] aFileNameList )
	{
		// イベントマネージャを生成し、リスナー登録する
		m_download_event_manager = new EventManagerDynamic ();
		m_download_event_manager.AddListener<DownloadEventController> (OnCompleateAssetDownload);
		
		m_down_load = new DownloadController ();
		
		if (m_down_load.p_DownloadList == null)
			m_down_load.p_DownloadList = new List<ContentInformation> ();
		else
			m_down_load.p_DownloadList.Clear ();
		
		foreach (string file_name in aFileNameList)
			m_down_load.p_DownloadList.Add (new ContentInformation (file_name, 0));

		m_down_load.p_DomainName = "http://210.140.154.119:82/ab/";

		// ダウンロードコントローラでイベントを発行できるようにする為、
		// 生成したイベントマネージャを登録する
		m_down_load.SetEventManager (m_download_event_manager);
	}

	// -------------
	// アセットダウンロード完了処理
	// -------------
	public void OnCompleateAssetDownload( DownloadEventController aEvent )
	{

		// アセットダウダウンロード完了時
		if (aEvent.p_DownloadFileType != DownloadEventController.enmDownloadFileType.DOWNLOAD_ASSETBANDLE)
			return;

		if( m_gm_objects == null )
			m_gm_objects = new GameObject[10];

		foreach (ContentInformation content_info in aEvent.p_RequestContents) {

			m_gm_objects [m_gm_object_count] = Instantiate (content_info.p_GameObjectData);
			m_gm_objects [m_gm_object_count].name = content_info.p_FileName;

			var mypos = transform.position;
			var addpos = new Vector3 (Random.value, Random.value, Random.value);
			mypos += addpos;

			m_gm_objects [m_gm_object_count].transform.position = mypos;

			m_gm_object_count++;
		}

		// 次回イベント発行の為にここでクリア
		aEvent = null;

		// シーケンスを終了
		m_now_sequence = enmSequence.NOP;

	}


	void DrawMap()
	{
		
	}
	
	void DrawProgress()
	{
		
	}

	void OnDataAccessCompleate( GameEventDataAccess aEventDataAccess )
	{
		if (aEventDataAccess.p_DataType == GameEventDataAccess.enmDataType.DATA_BINARY) {
			Debug.Log("Data Binary");
		} else if (aEventDataAccess.p_DataType == GameEventDataAccess.enmDataType.DATA_TEXT) {
			Debug.Log("Data Text");
		}
	}


	void OnError( GameEventError aEventError )
	{
		Debug.Log ("Data Text");
	}
}
