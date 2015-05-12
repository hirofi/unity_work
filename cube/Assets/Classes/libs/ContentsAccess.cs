using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ContentInformation
{
	// ダウンロードステータス
	private ContentsAccess.enmDownloadStatus m_download_status = 0;
	public ContentsAccess.enmDownloadStatus p_DownloadStatus {
		get { return m_download_status;	}
		set { m_download_status = value;}
	}
	//ファイル名
	private string m_file_name = null;
	public string p_FileName {
		get { return m_file_name;	}
		set { m_file_name = value;	}
	}
	// コンテンツタイプ
	private ContentsAccess.enmDownloadContentType m_content_type = 0;
	public ContentsAccess.enmDownloadContentType p_ContentType {
		get { return m_content_type;	}
		set { m_content_type = value;	}
	}
	// バージョン
	private int m_content_version = 0;
	public int p_Version{
		get { return m_content_version;	}
		set { m_content_version = value;	}
	}
	
	private UnityEngine.Object m_unity_object = null;
	public UnityEngine.Object p_UnityObjectData {
		get { return m_unity_object;	}
		set { m_unity_object = value;	}
	}
	
	// オーディオデータ
	public AudioClip p_AudioData {
		get { return m_unity_object as AudioClip;	}
	}
	
	// ゲームオブジェクト
	public GameObject p_GameObjectData {
		get { return m_unity_object as GameObject;	}
	}
	// テキスト
	public TextAsset p_TextData2{
		get { return m_unity_object as TextAsset; }
	}

	private string m_text = null;
	public string p_StringData{
		get { return m_text; }
		set { m_text = value;	}
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
			if( m_req_contents.p_DownloadStatus != enmDownloadStatus.WAITING_FOR_START )
				continue;
			
			if( m_read_thread_count > aReqDownloadList.Count )
				continue;
			
			f_GetAssetBandle( m_req_contents.p_FileName, m_req_contents.p_Version );
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
		
		f_SetDownloadStatus ( aContentInformation.p_FileName, enmDownloadStatus.DOWNLOADING);

		// 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードする
		// またはダウンロードしてキャッシュに格納します。
		using (WWW w3 = WWW.LoadFromCacheOrDownload(aUrl, aVersion) )
		{
			// ファイル名でオブジェクト配列を検索
			int req_idx = -1;
			foreach (ContentInformation req in m_req_contents) {
				if( req.p_FileName == aContentInformation.p_FileName )
				{
					req.p_DownloadStatus = enmDownloadStatus.DOWNLOADING;
					req_idx = m_req_contents.IndexOf( req );
					break;
				}
			}
			
			if( req_idx <= -1 )
				throw new Exception ("ダウンロード対象でない:" + w3.error);
			
			yield return w3;
			
			if ( w3.error != null ) {
				f_SetDownloadStatus (aContentInformation.p_FileName, enmDownloadStatus.ERROR_EXIT);
				throw new Exception ("WWWダウンロードにエラーがありました:" + w3.error);
			}
			else
			{
				m_req_contents[req_idx].p_UnityObjectData = w3.assetBundle.mainAsset;
				if (aUrl.IndexOf("texture") > -1)
				{
					//画像
					m_req_contents[req_idx].p_ContentType = enmDownloadContentType.TEXTURE;
					
				}
				else if (aUrl.IndexOf("3d") > -1)
				{
					//3Dデータ
					m_req_contents[req_idx].p_ContentType = enmDownloadContentType.GAMEOBJECT;
				}
				else if (aUrl.IndexOf("audio") > -1)
				{
					//サウンド
					m_req_contents[req_idx].p_ContentType = enmDownloadContentType.AUDIO;
				}
				else if (aUrl.IndexOf("text") > -1 )
				{
					// テキスト
					m_req_contents[req_idx].p_ContentType = enmDownloadContentType.TEXT;
				}
				else
				{
					// 不明なコンテンツ
					m_req_contents[req_idx].p_ContentType = enmDownloadContentType.UNKNOWN;
				}
				
			}
			
			f_SetDownloadStatus (aContentInformation.p_FileName, enmDownloadStatus.COMPLETE);
			
		}
		yield return null;
	}

	public void f_SetDownloadStatus( string aAssetName , enmDownloadStatus aStatus )
	{
		foreach (ContentInformation req in m_req_contents) {
			if( req.p_FileName == aAssetName )
				req.p_DownloadStatus = aStatus;
		}
	}
	
	private bool f_IsCompleateDownload()
	{
		bool ret = false;
		
		foreach (ContentInformation req in m_req_contents) {
			if( req.p_DownloadStatus == enmDownloadStatus.WAITING_FOR_START ||
			   req.p_DownloadStatus == enmDownloadStatus.DOWNLOADING )
				return false;
			else if( req.p_DownloadStatus == enmDownloadStatus.COMPLETE ||
			        req.p_DownloadStatus == enmDownloadStatus.ERROR_EXIT )
				ret = true;
		}
		
		return ret;
	}
}
