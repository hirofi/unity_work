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
	
	MapController m_map = null;
	TextMesh m_progress_tm = null;

	GameObject[] m_gm_objects;
	int m_gm_object_count;

	// サウンドコントローラ用のGameObject
	GameObject m_sound_go = null;

	// データコントローラのGameObject
	GameObject m_data_go = null;

	GameObject m_system_info = null;

	// シーン切り替えディスパッチャ用
	GameObject m_scene_go = null;

	// ポップアップ切り替えディスパッチャ用
	GameObject m_popup_go = null;

	private MoverController m_mover_controller = null;


	// データ
	void Awake()
	{
		m_system_info = new GameObject("SystemWatcher");
		m_system_info.AddComponent<AllocMem>();

		m_sound_go = new GameObject ("SoundController");
		m_sound_go.AddComponent<SoundController>();

		m_data_go = new GameObject ("DataController");
		m_data_go.AddComponent<DataController> ();

		m_scene_go = new GameObject ("SceneDispater");
		m_scene_go.AddComponent<SceneView> ();

		m_popup_go = new GameObject ("PopUpDispatcher");
		m_popup_go.AddComponent<PopUpView> ();

	}

	// Use this for initialization
	void Start () {
		m_now_sequence = enmSequence.INITIALIZE;

		// サウンドの登録
		SoundController.Instance.f_Attach ("bgm_wav_01");
		SoundController.Instance.f_Attach ("bgm_ogg_01");
		SoundController.Instance.f_Attach ("bgm_wav_02");
		SoundController.Instance.f_Attach ("bgm_ogg_02");

		//
		GameObject obj = Instantiate(Resources.Load("Prefab/obj01")) as GameObject;
		obj.name = "save";
		obj.transform.position = new Vector3(5,5,0);
		obj.AddComponent<ObjPrefabView> ();

		// シーンとポップアップの切り替え
		SceneView.Instance._event_manager.f_AddListener<Scene01Event> (f_OnScene01);
		SceneView.Instance._event_manager.f_AddListener<SceneGeneralEvent> (f_OnSceneGeneral);
		PopUpView.Instance._event_manager.f_AddListener<PopUpEvent> (f_OnPopUp);

		m_mover_controller = new MoverController ();
		for (int i=0; i<10; i++) {

			Vector3 addpos = new Vector3 (Random.value*10, Random.value*10, Random.value*10);
			Vector3 mypos = transform.position + addpos;

			m_mover_controller.f_CreateMover ("mover" + i.ToString (), mypos);
		}

	}
	
	// Update is called once per frame
	void Update () {
		Sequence ();
	}

	MainController()
	{
		DataManager.Instance.f_AddListener<GameEventDataAccess> (OnDataAccessCompleate);
		ErrorManager.Instance.f_AddListener<GameEventError> (OnError);
	}

	// -------------
	// シーケンス切り替え処理
	// -------------

	enum enmSequence{
		NOP = 0,			// 処理なし
		INITIALIZE,			//　初期化処理
		LOAD_DOWNLOAD_LIST, // ダウンロード対象のアセット一覧取得
		LOAD_ASSET,			// アセットダウンロード
		LOAD_SOUND,			// サウンドファイル
		LOAD_COMPLEATE,		// ダウンロード完了
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
			f_GetDownloadList();

			break;
		case enmSequence.LOAD_ASSET:
			m_now_sequence = enmSequence.NOP;
			m_down_load.f_StartDownloadAssetBandles ();
			break;
		case enmSequence.LOAD_SOUND:
			m_now_sequence = enmSequence.NOP;
			m_sound_file_download.f_StartDownload();
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
	DownloadController m_list_download = null;
	void f_GetDownloadList()
	{
		m_list_download = new DownloadController ();
		m_list_download.f_AddListener<DownloadEvent>(f_OnCompleateListDownload);
		ContentInformation info = new ContentInformation ("dllist.txt", 0);
		info._request_save_data = true;
		m_list_download._download_list.Add (info);

		m_list_download._domain_name = "http://210.140.154.119:82/ab/";
		m_list_download._download_request_type = DownloadController.enmDownloadRequestType.REQUEST_DOWNLOAD_FILE_LIST;

		m_list_download.f_StartDownload ();
	}

	// -------------
	// ダウンロード対象のアセット一覧取得完了処理
	// -------------
	List<string> m_prefab_name_list = new List<string>();
	List<string> m_sound_file_name_list = new List<string>();
	public void f_OnCompleateListDownload( DownloadEvent aEvent )
	{
		// ダウンロードリスト取得完了時
		if (aEvent._download_file_type != DownloadEvent.enmDownloadFileType.DOWNLOAD_ORDER_LIST)
			return;

		// リスト文字列をパースしてアセットとサウンドファイルのダウンロードを開始する
		foreach ( ContentInformation content_info in aEvent._request_contents)
		{
			if( content_info._string_data != null )
			{
				Dictionary< string, List<string> > req_list = m_list_download.f_GetDownloadLists(content_info._string_data);

				// アセットのダウンロード
				f_DoDownloadAsset( req_list[DownloadController.PREFAB_FILE_LIST] );
				
				// サウンドファイルのダウンロード
				f_DoDownloadFile( req_list[DownloadController.SOUND_FILE_LIST] );

			}

			// 1リストのみ対応のため、ここでブレイク
			break;
		}

		// ダウンロードの完了状態を確認、エラーなら終了
		if(m_list_download.f_IsDownloadSucceed() == false )
		{
			List<ContentInformation> retry_list = m_list_download.f_GetRetryList();
			m_now_sequence = enmSequence.NOP;
			return;
		}

		// シーケンスをアセットバンドルの取得に移行
		m_now_sequence = enmSequence.LOAD_ASSET;
	}

	// -------------
	// アセットダウンロード開始処理
	// -------------
	DownloadController m_down_load = null;
	void f_DoDownloadAsset( List<string> aFileNameList )
	{
		m_down_load = new DownloadController ();
		m_down_load.f_AddListener<DownloadEvent> (f_OnCompleateAssetDownload);

		foreach (string file_name in aFileNameList)
			m_down_load._download_list.Add (new ContentInformation (file_name, 0));

		m_down_load._domain_name = "http://210.140.154.119:82/ab/";
		m_down_load._download_request_type = DownloadController.enmDownloadRequestType.REQUEST_ASSET_BANDLE;

	}

	// -------------
	// アセットダウンロード完了処理
	// -------------
	public void f_OnCompleateAssetDownload( DownloadEvent aEvent )
	{

		// アセットダウダウンロード完了時
		if (aEvent._download_file_type != DownloadEvent.enmDownloadFileType.DOWNLOAD_ASSETBANDLE)
			return;

		if( m_gm_objects == null )
			m_gm_objects = new GameObject[10];

		float x = -5.0f;
		float y = 0.0f;
		foreach (ContentInformation content_info in aEvent._request_contents) {

			if( content_info._download_status == ContentsAccess.enmDownloadStatus.COMPLETE )
			{
				m_gm_objects [m_gm_object_count] = Instantiate (content_info._game_object_data);
				m_gm_objects [m_gm_object_count].name = content_info._file_name;
				m_gm_objects [m_gm_object_count].AddComponent<ObjPrefabView>();
				var mypos = transform.position;
//				var addpos = new Vector3 (Random.value*10, Random.value*10, Random.value*10);
				var addpos = new Vector3 (x+=2, y++, 0.0f);
				mypos += addpos;

				m_gm_objects [m_gm_object_count].transform.position = mypos;
				m_gm_object_count++;
			}

		}

		// シーケンスをアセットバンドルの取得に移行
		m_now_sequence = enmSequence.LOAD_SOUND;

		Debug.Log ("★asset download compleate " );

	}

	// -------------
	// ファイルダウンロード開始処理
	// -------------
	DownloadController m_sound_file_download = null;
	List<DownloadController> m_file_download = new List<DownloadController>();
	void f_DoDownloadFile( List<string> p_file_name )
	{

		m_sound_file_download = new DownloadController ();
//		m_sound_file_download._download_list = new List<ContentInformation> ();

		foreach (string file_name in p_file_name) {
			ContentInformation info = new ContentInformation (file_name, 0);
			info._request_save_data = true;
			m_sound_file_download._download_list.Add (info);
		}

		m_sound_file_download._domain_name = "http://210.140.154.119:82/";
		m_sound_file_download._download_request_type = DownloadController.enmDownloadRequestType.REQUEST_DATA_FILE;

		// リスナーを登録
		m_sound_file_download.f_AddListener<DownloadEvent> (OnCompleateFileDownload);

	}
	
	// -------------
	// ファイルダウンロード完了処理
	// -------------
	public void OnCompleateFileDownload( DownloadEvent aEvent )
	{

		// シーケンスを終了
		m_now_sequence = enmSequence.NOP;
		
		Debug.Log ("★file download compleate " );
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

	// create sound
	void f_OnSoundChangeStatus( SoundEvent aEvent )
	{
		
	}

	void f_OnPopUp(PopUpEvent e)
	{
		Debug.Log ("f_OnPopUp");
	}

	void f_OnScene01(Scene01Event e)
	{
		Debug.Log ("f_OnScene01");
	}

	void f_OnSceneGeneral(SceneGeneralEvent e)
	{
		Debug.Log ("f_OnSceneGeneral");
	}

}
