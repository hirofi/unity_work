using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ContentInformation
{
	// ダウンロードステータス
	private ContentsAccess.enmDownloadStatus m_download_status = 0;
	public ContentsAccess.enmDownloadStatus _download_status {
		get { return m_download_status;	}
		set { m_download_status = value;}
	}
	//ファイル名
	private string m_file_name = null;
	public string _file_name {
		get { return m_file_name;	}
		set { m_file_name = value;	}
	}
	// コンテンツタイプ
	private ContentsAccess.enmDownloadContentType m_content_type = 0;
	public ContentsAccess.enmDownloadContentType _content_type {
		get { return m_content_type;	}
		set { m_content_type = value;	}
	}
	// バージョン
	private int m_content_version = 0;
	public int _version{
		get { return m_content_version;	}
		set { m_content_version = value;	}
	}
	
	private UnityEngine.Object m_unity_object = null;
	public UnityEngine.Object _unity_object_data {
		get { return m_unity_object;	}
		set { m_unity_object = value;	}
	}
	
	// オーディオデータ
	public AudioClip _audio_data {
		get { return m_unity_object as AudioClip;	}
	}
	
	// ゲームオブジェクト
	public GameObject _game_object_data {
		get { return m_unity_object as GameObject;	}
	}
	// テキスト
	public TextAsset _text_data{
		get { return m_unity_object as TextAsset; }
	}

	private string m_string_data = null;
	public string _string_data{
		get { return m_string_data; }
		set { m_string_data = value;	}
	}

	private byte[] m_byte;
	public byte[] _byte_data
	{
		get { return m_byte; }
		set { m_byte = value; }
	}

	private int m_retry_count;
	public int _retry_count
	{
		get { return m_retry_count; }
		set { m_retry_count = value; }
	}

	private string m_proc_uuid;
	public string _proc_uuid
	{
		get {return m_proc_uuid;}
		set { m_proc_uuid = value; }
	}

	private bool m_request_download_list;
	public bool _request_download_list
	{
		get {return m_request_download_list;}
		set { m_request_download_list = value; }
	}

	private bool m_request_save_data = false;
	public bool _request_save_data {
		get { return m_request_save_data;	}
		set { m_request_save_data = value;	}
	}

	public ContentInformation( string aFileName , int aVersion )
	{
		m_file_name = aFileName;
		m_content_version = aVersion;
	}
}

public class ContentsAccess : MonoBehaviour {
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ( f_IsCompleateDownload() ) {
			on_compleat (m_req_contents);
			m_req_contents.Clear();
		}
	}
	
	string m_url = "pshpz01.isl.gacha.fujitv.co.jp/unity/";
	public enum enmDownloadStatus
	{
		WAITING_FOR_START = 0,
		DOWNLOADING,
		COMPLETE,
		ERROR_EXIT,
		ENTRY_MAX
	}
	
	public enum enmDownloadContentType
	{
		UNKNOWN = 0,
		TEXTURE,
		GAMEOBJECT,
		AUDIO,
		TEXT,
		ENTRY_MAX
	}
	
	private enmDownloadContentType m_download_content_type = enmDownloadContentType.UNKNOWN;
	public enmDownloadContentType DownloadContentType {
		get { return m_download_content_type; }
	}
	
	private enmDownloadStatus m_download_status = enmDownloadStatus.WAITING_FOR_START;
	public enmDownloadStatus p_DownloadStatus
	{
		get{ return m_download_status; }
	}
	
	public delegate void f_OnComplete ( List<ContentInformation>aContentInformation );
	public f_OnComplete on_compleat = null;
	public bool m_is_req_download = false;
	
	private List<ContentInformation> m_req_contents = new List<ContentInformation>();
	
	private int m_read_thread_count = 1;
	public int ReadThreadCount {
		get { return m_read_thread_count;	}
		set { m_read_thread_count = value; }
	}

	public string p_ServerPath {
		get { return m_url;		}
		set { m_url = value;	}
	}

	public void f_RequestDownloadFiles( List<ContentInformation> aReqDownloadList )
	{
		int count = 0;
		foreach( ContentInformation m_req_contents in aReqDownloadList )
		{
			if( m_req_contents._download_status != enmDownloadStatus.WAITING_FOR_START )
				continue;
			
			if( m_read_thread_count > aReqDownloadList.Count )
				continue;
			
			f_GetAssetBandle( m_req_contents._file_name, m_req_contents._version );
			count++;
		}
	}
	
	/// <summary>
	/// アセットをダウンロードする。
	/// 既にキャッシュがあればそちらから取り出す。
	/// </summary>
	/// <param name="aDownloadFileName">A download file name.</param>
	private void f_GetAssetBandle( string aDownloadFileName , int aVersion)
	{
		ContentInformation content_info = new ContentInformation( aDownloadFileName , aVersion );
		m_req_contents.Add( content_info );
		
		// Clear Cache
		Caching.CleanCache();
		
		#if   UNITY_ANDROID && !UNITY_EDITOR
		string url = "http://" + m_url + aDownloadFileName + ".unity3d.android.unity3d";
		#elif UNITY_IPHONE  && !UNITY_EDITOR
		string url = "http://" + m_url + aDownloadFileName + ".unity3d.iphone.unity3d";
		#else
		string url = "http://" + m_url + aDownloadFileName + ".unity3d.unity3d";
		#endif
		
		StartCoroutine ( f_DownloadAndCache( content_info, url, 0 ) );
		
	}
	
	private IEnumerator f_DownloadAndCache ( ContentInformation aContentInformation, string aUrl, int aVersion)
	{
		
		// キャッシュシステムの準備が完了するのを待ちます
		while (!Caching.ready)
			yield return null;
		
		f_SetDownloadStatus ( aContentInformation._file_name, enmDownloadStatus.DOWNLOADING);

		// 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードする
		// またはダウンロードしてキャッシュに格納します。
		using (WWW w3 = WWW.LoadFromCacheOrDownload(aUrl, aVersion) )
		{
			// ファイル名でオブジェクト配列を検索
			int req_idx = -1;
			foreach (ContentInformation req in m_req_contents) {
				if( req._file_name == aContentInformation._file_name )
				{
					req._download_status = enmDownloadStatus.DOWNLOADING;
					req_idx = m_req_contents.IndexOf( req );
					break;
				}
			}
			
			if( req_idx <= -1 )
				throw new Exception ("ダウンロード対象でない:" + w3.error);
			
			yield return w3;
			
			if ( w3.error != null ) {
				f_SetDownloadStatus (aContentInformation._file_name, enmDownloadStatus.ERROR_EXIT);
				throw new Exception ("WWWダウンロードにエラーがありました:" + w3.error);
			}
			else
			{
				m_req_contents[req_idx]._unity_object_data = w3.assetBundle.mainAsset;
				if (aUrl.IndexOf("texture") > -1)
				{
					//画像
					m_req_contents[req_idx]._content_type = enmDownloadContentType.TEXTURE;
					
				}
				else if (aUrl.IndexOf("3d") > -1)
				{
					//3Dデータ
					m_req_contents[req_idx]._content_type = enmDownloadContentType.GAMEOBJECT;
				}
				else if (aUrl.IndexOf("audio") > -1)
				{
					//サウンド
					m_req_contents[req_idx]._content_type = enmDownloadContentType.AUDIO;
				}
				else if (aUrl.IndexOf("text") > -1 )
				{
					// テキスト
					m_req_contents[req_idx]._content_type = enmDownloadContentType.TEXT;
				}
				else
				{
					// 不明なコンテンツ
					m_req_contents[req_idx]._content_type = enmDownloadContentType.UNKNOWN;
				}
				
			}
			
			f_SetDownloadStatus (aContentInformation._file_name, enmDownloadStatus.COMPLETE);
			
		}
		yield return null;
	}

	public void f_SetDownloadStatus( string aAssetName , enmDownloadStatus aStatus )
	{
		foreach (ContentInformation req in m_req_contents) {
			if( req._file_name == aAssetName )
				req._download_status = aStatus;
		}
	}
	
	private bool f_IsCompleateDownload()
	{
		bool ret = false;
		
		foreach (ContentInformation req in m_req_contents) {
			if( req._download_status == enmDownloadStatus.WAITING_FOR_START ||
			   req._download_status == enmDownloadStatus.DOWNLOADING )
				return false;
			else if( req._download_status == enmDownloadStatus.COMPLETE ||
			        req._download_status == enmDownloadStatus.ERROR_EXIT )
				ret = true;
		}
		
		return ret;
	}
}
