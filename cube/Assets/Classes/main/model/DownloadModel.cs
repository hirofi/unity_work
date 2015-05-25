using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DownloadModel : FileAccess {

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

	public delegate void OnComplete ( List<ContentInformation>p_content_information );
	public OnComplete f_OnCompleat = null;
	public bool m_is_req_download = false;

	public delegate void OnProgress(int aProgress);
	public OnProgress on_progress = null;

	private List<ContentInformation> m_req_contents = new List<ContentInformation>();

	private int m_read_thread_count = 1;
	public int _read_thread_count {
		get { return m_read_thread_count;	}
		set { m_read_thread_count = value; }
	}

	public void f_RequestDownloadFiles( List<ContentInformation> p_req_download_list , bool p_is_req_asset, string p_proc_uuid )
	{
		// リクエスト処理開始、初期化
		f_ClearRequest ();
		foreach (ContentInformation req_contents in p_req_download_list)
		{
			if (req_contents._download_status != ContentsAccess.enmDownloadStatus.WAITING_FOR_START)
				continue;

			if (m_read_thread_count > p_req_download_list.Count)
				continue;

			req_contents._proc_uuid = p_proc_uuid;
			if ( p_is_req_asset )
			{
				f_GetAssetBandle ( req_contents );
			}
			else
			{
				f_GetFile ( req_contents );
			}
		}
	}

	public void f_ClearRequest()
	{
		m_request_done = false;

		if (m_req_contents != null && m_req_contents.Count > 0 ) {
			m_req_contents.Clear ();
		}
	}

	public void f_ClearRequest( string p_req_name , bool p_abort )
	{

		m_request_done = false;

		foreach (ContentInformation req in m_req_contents) {
			
			// 名前が指定されてなければステータス COMPLETE のエントリを削除する
			if( p_req_name == null)
			{
				if( req._download_status == ContentsAccess.enmDownloadStatus.COMPLETE )
				{
					m_req_contents.Remove( req );
				}
			}
			else if( req._file_name == p_req_name )
			{
				if( p_abort || req._download_status == ContentsAccess.enmDownloadStatus.COMPLETE )
				{
					m_req_contents.Remove( req );
				}
			}

			// エントリがなくなったらブレイク
			if(m_req_contents.Count == 0)
				break;
		}
	}

	private void f_GetFile( ContentInformation p_content_information )
	{
		m_req_contents.Add( p_content_information );

		// Clear Cache
		Caching.CleanCache();

		StartCoroutine ( f_Download( p_content_information ));
	}

	// IEnumerator の外にthrow すと例外エラーになるのでこのメソッド内でエラー処理しておく
	private IEnumerator f_Download( ContentInformation p_content_information )
	{

		string url = m_domain + p_content_information._file_name;
		using (WWW w3 = new WWW (url)) {

			while (!w3.isDone) { // ダウンロードの進捗を表示
				if (on_progress != null)
					on_progress (Mathf.CeilToInt (w3.progress * 100));
				yield return null;
			}

			if (w3.error != null) {
				f_SetDownloadStatus (p_content_information._file_name, ContentsAccess.enmDownloadStatus.ERROR_EXIT);
			} else {

				f_SetDownloadStatus (p_content_information._file_name, ContentsAccess.enmDownloadStatus.COMPLETE);
				p_content_information._content_type = ContentsAccess.enmDownloadContentType.TEXT;
				p_content_information._string_data = w3.text;
				p_content_information._byte_data = w3.bytes;

				// 保存フラグがtrueならローカルファイルに書き出し
				if ( p_content_information._request_save_data ) {
					f_Save (w3, p_content_information._file_name, p_content_information._request_save_data );
				}

				f_SetDownloadStatus (p_content_information._file_name, ContentsAccess.enmDownloadStatus.COMPLETE);
			}
		}

		if ( f_IsCompleateDownload ()) {
			f_OnCompleat (m_req_contents);
		}
		
	}

	/// <summary>
	/// アセットをダウンロードする。
	/// 既にキャッシュがあればそちらから取り出す。
	/// </summary>
	/// <param name="p_download_file_name">A download file name.</param>
	private void f_GetAssetBandle( ContentInformation p_content_information )
	{
		
		m_req_contents.Add( p_content_information );
		
		// Clear Cache
		Caching.CleanCache();
		

		StartCoroutine ( f_DownloadAndCache( p_content_information ) );

	}

	private IEnumerator f_DownloadAndCache ( ContentInformation p_content_information )
	{

		#if   UNITY_ANDROID && !UNITY_EDITOR
		string url = m_domain + p_content_information._file_name + ".unity3d.android.unity3d?v="+p_content_information._version;
		#elif UNITY_IPHONE  && !UNITY_EDITOR
		string url = m_domain + p_content_information._file_name + ".unity3d.iphone.unity3d?v="+p_content_information._version;
		#else
		string url = m_domain + p_content_information._file_name + ".unity3d.unity3d?v="+p_content_information._version;
		#endif


		// キャッシュシステムの準備が完了するのを待ちます
		while (!Caching.ready)
			yield return null;

		// 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードする
		// またはダウンロードしてキャッシュに格納します。
		using (WWW w3 = WWW.LoadFromCacheOrDownload( url, p_content_information._version )) {

			// ファイル名でオブジェクト配列を検索
			int req_idx = -1;
			foreach (ContentInformation req in m_req_contents) {

				// ダウンロード要求のファイルが待ち状態であればインデックスを有効にする
				if (req._file_name == p_content_information._file_name &&
					req._download_status == ContentsAccess.enmDownloadStatus.WAITING_FOR_START)
				{
					req._download_status = ContentsAccess.enmDownloadStatus.DOWNLOADING;
					req_idx = m_req_contents.IndexOf (req);
					break;
				}
			}

			if (req_idx <= -1)
				yield return null;

			while (!w3.isDone) { // ダウンロードの進捗を表示
				if (on_progress != null)
					on_progress (Mathf.CeilToInt (w3.progress * 100));
				yield return null;
			}

			if (w3.error != null)
			{
				f_SetDownloadStatus (p_content_information._file_name, ContentsAccess.enmDownloadStatus.ERROR_EXIT);
			}
			else
			{
				m_req_contents [req_idx]._unity_object_data = w3.assetBundle.mainAsset;
				if (url.IndexOf ("texture") > -1) {
					//画像
					m_req_contents [req_idx]._content_type = ContentsAccess.enmDownloadContentType.TEXTURE;

				} else if (url.IndexOf ("3d") > -1) {
					//3Dデータ
					m_req_contents [req_idx]._content_type = ContentsAccess.enmDownloadContentType.GAMEOBJECT;
				} else if (url.IndexOf ("audio") > -1) {
					//サウンド
					m_req_contents [req_idx]._content_type = ContentsAccess.enmDownloadContentType.AUDIO;
				} else if (url.IndexOf ("text") > -1) {
					// テキスト
					m_req_contents [req_idx]._content_type = ContentsAccess.enmDownloadContentType.TEXT;
				} else {
					// 不明なコンテンツ
					m_req_contents [req_idx]._content_type = ContentsAccess.enmDownloadContentType.UNKNOWN;
				}

				f_SetDownloadStatus (p_content_information._file_name, ContentsAccess.enmDownloadStatus.COMPLETE);
			}


		}

		if ( f_IsCompleateDownload ()) {
			f_OnCompleat (m_req_contents);
			m_request_done = true;
		}
	}

	public void f_SetDownloadStatus( string aAssetName , ContentsAccess.enmDownloadStatus aStatus )
	{
		foreach (ContentInformation req in m_req_contents) {
			if( req._file_name == aAssetName )
			{
				req._download_status = aStatus;
			}
		}
	}

	private bool f_IsCompleateDownload()
	{
		if( m_request_done )
			return false;

		if (m_req_contents.Count <= 0)
			return false;

		// ダウンロード開始前、または、ダウンロード中であればFALSEを返す
		foreach (ContentInformation req in m_req_contents) {
			if( req._download_status == ContentsAccess.enmDownloadStatus.WAITING_FOR_START ||
			   req._download_status == ContentsAccess.enmDownloadStatus.DOWNLOADING )
				return false;
		}

		// エラーか、完了していれば,要求完了フラグをtrueにし、TRUEを返す
		m_request_done = true;
		return true;
	}
}
