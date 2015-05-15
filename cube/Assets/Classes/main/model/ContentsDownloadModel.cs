using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ContentsDownloadModel : FileAccess {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

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

	string m_domain = null;
	public string _domain {
		get { return m_domain; 	}
		set { m_domain = value;	}
	}

	private enmDownloadContentType m_download_content_type = enmDownloadContentType.UNKNOWN;
	public enmDownloadContentType DownloadContentType {
		get { return m_download_content_type; }
	}

	private enmDownloadStatus m_download_status = enmDownloadStatus.WAITING_FOR_START;
	public enmDownloadStatus DownloadStatus
	{
		get{ return m_download_status; }
	}

	private static bool m_request_done = false;

	public delegate void OnComplete ( List<ContentInformation>aContentInformation );
	public OnComplete on_compleat = null;
	public bool m_is_req_download = false;

	public delegate void OnProgress(int aProgress);
	public OnProgress on_progress = null;

	private List<ContentInformation> m_req_contents = new List<ContentInformation>();

	private int m_read_thread_count = 1;
	public int ReadThreadCount {
		get { return m_read_thread_count;	}
		set { m_read_thread_count = value; }
	}

	public void RequestDownloadFiles( List<ContentInformation> aReqDownloadList , bool aIsReqFileList, bool p_req_save_data )
	{
		int count = 0;

		// リクエスト処理開始、初期化
		ClearRequest();
		foreach( ContentInformation req_contents in aReqDownloadList )
		{
			if( req_contents.p_DownloadStatus != ContentsAccess.enmDownloadStatus.WAITING_FOR_START )
				continue;

			if( m_read_thread_count > aReqDownloadList.Count )
				continue;

			if( aIsReqFileList )
			{
				GetFile( req_contents.p_FileName , p_req_save_data );
			}
			else
			{
				GetAssetBandle( req_contents.p_FileName , req_contents.p_Version );
				count++;
			}
		}
	}

	public void ClearRequest()
	{
		m_request_done = false;

		if (m_req_contents != null && m_req_contents.Count > 0 ) {
			m_req_contents.Clear ();
		}
	}

	public void ClearRequest( string aRequestName , bool aAbort )
	{

		m_request_done = false;

		foreach (ContentInformation req in m_req_contents) {
			
			// 名前が指定されてなければステータス COMPLETE のエントリを削除する
			if( aRequestName == null)
			{
				if( req.p_DownloadStatus == ContentsAccess.enmDownloadStatus.COMPLETE )
				{
					m_req_contents.Remove( req );
				}
			}
			else if( req.p_FileName == aRequestName )
			{
				if( aAbort || req.p_DownloadStatus == ContentsAccess.enmDownloadStatus.COMPLETE )
				{
					m_req_contents.Remove( req );
				}
			}

			// エントリがなくなったらブレイク
			if(m_req_contents.Count == 0)
				break;
		}
	}

	private void GetFile( string aDownloadFileName , bool p_req_save_data )
	{

		ContentInformation content_info = new ContentInformation( aDownloadFileName , 0 );
		m_req_contents.Add( content_info );
		
		// Clear Cache
		Caching.CleanCache();

		string url = _domain + aDownloadFileName;

		StartCoroutine ( Download(content_info, url, p_req_save_data));

	}

	private IEnumerator Download( ContentInformation aContentInformation, string aUrl, bool p_req_save_data )
	{
		WWW w3 = new WWW (aUrl);

		while (!w3.isDone) { // ダウンロードの進捗を表示
			Debug.Log ("w3:" + Mathf.CeilToInt(w3.progress*100));
			if(on_progress != null)
				on_progress(Mathf.CeilToInt(w3.progress*100));
			yield return null;
		}

//		yield return w3;

		if (w3.error != null) {
			SetDownloadStatus (aContentInformation.p_FileName, ContentsAccess.enmDownloadStatus.ERROR_EXIT);
			throw new Exception ("WWWダウンロードにエラーがありました[" + aContentInformation.p_FileName + "]" + w3.error);
		} else {

			SetDownloadStatus (aContentInformation.p_FileName, ContentsAccess.enmDownloadStatus.COMPLETE);
			aContentInformation.p_ContentType = ContentsAccess.enmDownloadContentType.TEXT;
			aContentInformation.p_StringData = w3.text;
			aContentInformation._byte_data = w3.bytes;

			// 保存フラグがtrueならローカルファイルに書き出し
			if( p_req_save_data )
			{
				f_Save( w3, aContentInformation.p_FileName, p_req_save_data );
			}
		}

		if ( IsCompleateDownload() ) {
			on_compleat (m_req_contents);
		}


	}

	/// <summary>
	/// アセットをダウンロードする。
	/// 既にキャッシュがあればそちらから取り出す。
	/// </summary>
	/// <param name="aDownloadFileName">A download file name.</param>
	private void GetAssetBandle( string aDownloadFileName , int aVersion)
	{
		
		ContentInformation content_info = new ContentInformation( aDownloadFileName , aVersion );
		m_req_contents.Add( content_info );
		
		// Clear Cache
		Caching.CleanCache();
		
		#if   UNITY_ANDROID && !UNITY_EDITOR
		string url = m_domain + aDownloadFileName + ".unity3d.android.unity3d?dl=1";
		
		#elif UNITY_IPHONE  && !UNITY_EDITOR
		string url = m_domain + aDownloadFileName + ".unity3d.iphone.unity3d?dl=1";
		#else
		string url = m_domain + aDownloadFileName + ".unity3d.unity3d?dl=2";
		#endif
		
		StartCoroutine ( DownloadAndCache( content_info, url, 0 ) );

	}

	private IEnumerator DownloadAndCache ( ContentInformation aContentInformation, string aUrl, int aVersion)
	{

		// キャッシュシステムの準備が完了するのを待ちます
		while (!Caching.ready)
			yield return null;

		// 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードする
		// またはダウンロードしてキャッシュに格納します。
		using (WWW w3 = WWW.LoadFromCacheOrDownload(aUrl, aVersion) )
		{

			// ファイル名でオブジェクト配列を検索
			int req_idx = -1;
			foreach (ContentInformation req in m_req_contents) {

				// ダウンロード要求のファイルが待ち状態であればインデックスを有効にする
				if( req.p_FileName == aContentInformation.p_FileName &&
				   req.p_DownloadStatus == ContentsAccess.enmDownloadStatus.WAITING_FOR_START
				   )
				{
					req.p_DownloadStatus = ContentsAccess.enmDownloadStatus.DOWNLOADING;
					req_idx = m_req_contents.IndexOf( req );
					break;
				}
			}

			if( req_idx <= -1)
				yield return null;

//			yield return w3;
			while (!w3.isDone) { // ダウンロードの進捗を表示
				Debug.Log ("w3:"+aContentInformation.p_FileName +" >> "+ Mathf.CeilToInt(w3.progress*100));
				if(on_progress != null)
					on_progress(Mathf.CeilToInt(w3.progress*100));
				yield return null;
			}

			if ( w3.error != null ) {
				SetDownloadStatus (aContentInformation.p_FileName,  ContentsAccess.enmDownloadStatus.ERROR_EXIT);
				throw new Exception ("WWWダウンロードにエラーがありました[" +aContentInformation.p_FileName +"]"+ w3.error);
			}
			else
			{
				m_req_contents[req_idx].p_UnityObjectData = w3.assetBundle.mainAsset;
				if (aUrl.IndexOf("texture") > -1)
				{
					//画像
					m_req_contents[req_idx].p_ContentType = ContentsAccess.enmDownloadContentType.TEXTURE;

				}
				else if (aUrl.IndexOf("3d") > -1)
				{
					//3Dデータ
					m_req_contents[req_idx].p_ContentType = ContentsAccess.enmDownloadContentType.GAMEOBJECT;
				}
				else if (aUrl.IndexOf("audio") > -1)
				{
					//サウンド
					m_req_contents[req_idx].p_ContentType = ContentsAccess.enmDownloadContentType.AUDIO;
				}
				else if (aUrl.IndexOf("text") > -1 )
				{
					// テキスト
					m_req_contents[req_idx].p_ContentType = ContentsAccess.enmDownloadContentType.TEXT;
				}
				else
				{
					// 不明なコンテンツ
					m_req_contents[req_idx].p_ContentType = ContentsAccess.enmDownloadContentType.UNKNOWN;
				}

			}

			SetDownloadStatus (aContentInformation.p_FileName, ContentsAccess.enmDownloadStatus.COMPLETE);

		}

		if ( IsCompleateDownload() ) {
			on_compleat (m_req_contents);
			m_request_done = true;
		}

	}

	public void SetDownloadStatus( string aAssetName , ContentsAccess.enmDownloadStatus aStatus )
	{
		foreach (ContentInformation req in m_req_contents) {
			if( req.p_FileName == aAssetName )
			{
				req.p_DownloadStatus = aStatus;
			}
		}
	}

	private bool IsCompleateDownload()
	{
		if( m_request_done )
			return false;

		if (m_req_contents.Count <= 0)
			return false;

		// ダウンロード開始前、または、ダウンロード中であればFALSEを返す
		foreach (ContentInformation req in m_req_contents) {
			if( req.p_DownloadStatus == ContentsAccess.enmDownloadStatus.WAITING_FOR_START ||
			   req.p_DownloadStatus == ContentsAccess.enmDownloadStatus.DOWNLOADING )
				return false;
		}

		// エラーか、完了していれば,要求完了フラグをtrueにし、TRUEを返す
		m_request_done = true;
		return true;
	}
}
