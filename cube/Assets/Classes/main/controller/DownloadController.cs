using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DownloadToken
{
	private int m_idx;
	public int _index {
		get { return m_idx; 	}
		set { m_idx = value;	}
	}

	private Vector3 m_position;
	public Vector3 _postion{
		get { return m_position; 	}
		set { m_position = value;	}
	}

	List<string> m_prefab_name_list;
	public List<string>_prefab_name_list{
		get { return m_prefab_name_list; 	}
		set { m_prefab_name_list = value;	}
	}

	List<string> m_sound_file_name_list;
	public List<string>_sound_file_name_list{
		get { return m_sound_file_name_list; 	}
		set { m_sound_file_name_list = value;	}
	}

}

//
public class DownloadController : EventManagerDynamic {

	const string DEFAULT_DOMAIN = "http://210.140.154.119:82/ab/";

	// ダウンロード要求種別
	public enum enmDownloadRequestType
	{
		REQUEST_DOWNLOAD_FILE_LIST, // ダウンロード一覧の取得を要求
		REQUEST_DATA_FILE,			// アセットバンドル以外のデータ取得を要求
		REQUEST_ASSET_BANDLE,		// アセットバンドルのデータ取得を要求
		ENTRY_MAX
	}

	private enmDownloadRequestType m_download_request_type;
	public enmDownloadRequestType _download_request_type{
		get { return m_download_request_type; }
		set { m_download_request_type = value;	}
	}

	// ダウンロード対象ドメイン
	private string m_domain_name;
	public string _domain_name {
		get { return m_domain_name; }
		set { m_domain_name = value;	}
	}

	// ダウンロードリスト
	private List<ContentInformation> m_download_list;
	public List<ContentInformation> _download_list {
		get { return m_download_list;	}
		set { m_download_list = value;	}
	}

	private GameObject m_download_go = null;
	private DownloadEvent.enmDownloadFileType m_download_file_type;
	private DownloadModel m_downloadmodel;


	public DownloadController()
	{
		m_download_list = new List<ContentInformation> ();
	}

	/// <summary>
	/// アセットバンドル以外のファイルをダウンロードする
	/// </summary>
	public void f_StartDownload()
	{

		// ContentsDownloadModelクラスは MonoBehaviour を継承しているので
		// new ではなく GameObject に AddComponet して生成する。。。 unity お作法により 
		string proc_uuid = null;
		if( m_download_go == null )
		{
			System.Guid guid = System.Guid.NewGuid ();
			proc_uuid = "DOUNLOAD>"+guid.ToString ();
			m_download_go = new GameObject(proc_uuid);
		}

		m_downloadmodel = m_download_go.AddComponent<DownloadModel> ();
		m_downloadmodel._domain = (_domain_name == null ) ? DEFAULT_DOMAIN : _domain_name;
		m_downloadmodel.f_OnCompleat = f_OnDownloadCompleate;

		m_downloadmodel.f_RequestDownloadFiles( m_download_list , false, proc_uuid );

	}

	/// <summary>
	/// アセットバンドルファイルをダウンロードする
	/// </summary>
	public void f_StartDownloadAssetBandles()
	{

	Debug.Log ("StartDownloadAssetBandles");

		if( m_download_list == null )
			return;

		m_download_file_type = DownloadEvent.enmDownloadFileType.DOWNLOAD_ASSETBANDLE;

		// ContentsDownloadModelクラスは MonoBehaviour を継承しているので
		// new ではなく GameObject に AddComponet して生成する。。。 unity お作法により 
		System.Guid guid = System.Guid.NewGuid ();
		string proc_uuid = "DOUNLOAD>"+guid.ToString ();
		GameObject emptyGameObject = new GameObject(proc_uuid);

		m_downloadmodel = emptyGameObject.AddComponent<DownloadModel> ();
		m_downloadmodel._domain = (_domain_name == null ) ? DEFAULT_DOMAIN : _domain_name;
		m_downloadmodel.f_OnCompleat = f_OnDownloadCompleate;
		m_downloadmodel.f_RequestDownloadFiles( m_download_list , true, proc_uuid );

	}


	/// <summary>
	/// ダウンロードが終了したらイベントマネージャに通知(イベントキュー)
	/// </summary>
	/// <param name="p_content_information">P_content_information. ダウンロード完了情報のリスト</param>
	private void f_OnDownloadCompleate( List<ContentInformation>p_content_information )
	{

	Debug.Log ("OnDownloadCompleate");

		DownloadEvent download_event = new DownloadEvent ( p_content_information );
		download_event._download_file_type = m_download_file_type;

		// リクエストのダウンロード結果を保持する
		m_download_list = download_event._request_contents;

		// ダウンロード完了イベント発行
		f_Dispatch(download_event);

		// 完了したDownloadController を削除する
		f_DestroyDownloadController (p_content_information);


		// ダウンロードリクエスト対象をクリア
		m_download_list = null;
		m_download_go = null;

	}

	/// <summary>
	/// ダウンロード完了情報により、ダウンロードに使用した GameObject を破棄する
	/// </summary>
	/// <param name="p_content_information">P_content_information.</param>
	public void f_DestroyDownloadController (List<ContentInformation>p_content_information)
	{
		foreach (ContentInformation info in p_content_information)
		{
			Debug.Log(">>>uuid:"+info._proc_uuid);

			if(info._download_status == ContentsAccess.enmDownloadStatus.COMPLETE || 
			   info._download_status == ContentsAccess.enmDownloadStatus.ERROR_EXIT)
			{
				GameObject download_controller = GameObject.Find( info._proc_uuid );
				GameObject.Destroy(download_controller);
			}
			else{
				Debug.Log("status none"+info._proc_uuid+" name="+info._file_name);
			}
		}
	}


	/// <summary>
	/// ダウンロード完了情報にエラーのエントリがあればfalseを返す
	/// </summary>
	/// <returns><c>true</c>, if is download succeed was f_ed, <c>false</c> otherwise.</returns>
	public bool f_IsDownloadSucceed()
	{
		foreach(ContentInformation download_list in m_download_list ){

			if( download_list._download_status == ContentsAccess.enmDownloadStatus.ERROR_EXIT )
				return false;
		}

		return true;
	}

	/// <summary>
	/// ダウンロード対象一覧から、ダウンロード情報を生成する
	/// 生成したリストはメンバ変数(m_prefab_name_listまたはm_sound_file_name_list)に保持する
	/// </summary>
	/// <returns>The get download lists.</returns>
	/// <param name="p_order_text">P_order_text.</param>
	public const string PREFAB_FILE_LIST = "prefab_file_list";
	public const string SOUND_FILE_LIST = "sound_file_list";
	public Dictionary< string, List<string> > f_GetDownloadLists( string p_order_text )
	{
		List<string> prefab_name_list = new List<string>();
		List<string> sound_file_name_list = new List<string>();

		string[] file_name_list = p_order_text.Split (
			new char[]{'\n','\r'},
		System.StringSplitOptions.RemoveEmptyEntries);
		
		
		for(int i=0; file_name_list.Length > i; i++ )
		{
			string fn = System.IO.Path.GetFileNameWithoutExtension(file_name_list[i]);
			string ext = System.IO.Path.GetExtension (file_name_list[i]);
			
			if(ext == ".pf")
				prefab_name_list.Add(fn);
			else if(ext == ".ogg" || ext == ".wav")
				sound_file_name_list.Add(file_name_list[i]);
		}

		Dictionary< string, List<string> > ret_list = new Dictionary< string, List<string> >();
		ret_list.Add (PREFAB_FILE_LIST, prefab_name_list);
		ret_list.Add (SOUND_FILE_LIST, sound_file_name_list);

		return ret_list;
	}

	// リトライ用のリストを生成し返す
	/// <summary>
	/// ダウンロード完了情報からエラーのエントリを抽出し、リトライ用のダウンロード情報リストを生成して返す
	/// </summary>
	/// <returns>The get retry list.</returns>
	public List<ContentInformation> f_GetRetryList()
	{
		List<ContentInformation> retry_list = new List<ContentInformation>();

		foreach (ContentInformation download_list in m_download_list)
		{
			if (download_list._download_status == ContentsAccess.enmDownloadStatus.ERROR_EXIT)
			{
				ContentInformation info = new ContentInformation(download_list._file_name,0);

				// リトライカウントが指定されていればここでディクリメント
				// 0以下なら-1をセットする
				info._retry_count = (info._retry_count > 0) ? info._retry_count-- : -1;
				retry_list.Add (info);
			}
		}

		// 解放
		m_download_list.Clear ();

		return retry_list;
	}

}
