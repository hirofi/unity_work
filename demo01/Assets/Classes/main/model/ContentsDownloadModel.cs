using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ContentInformation
{
	// ダウンロードステータス
	private ContentsDownloadModel.enmDownloadStatus m_download_status = 0;
	public ContentsDownloadModel.enmDownloadStatus Download_Status {
		get { return m_download_status;	}
		set { m_download_status = value;}
	}
	//ファイル名
	private string m_file_name = null;
	public string File_Name {
		get { return m_file_name;	}
		set { m_file_name = value;	}
	}
	// コンテンツタイプ
	private ContentsDownloadModel.enmDownloadContentType m_content_type = 0;
	public ContentsDownloadModel.enmDownloadContentType Content_Type {
		get { return m_content_type;	}
		set { m_content_type = value;	}
	}
	// バージョン
	private int m_content_version = 0;
	public int Version{
		get { return m_content_version;	}
		set { m_content_version = value;	}
	}

	private UnityEngine.Object m_unity_object = null;
	public UnityEngine.Object UnityObjectData {
		get { return m_unity_object;	}
		set { m_unity_object = value;	}
	}

	// オーディオデータ
	public AudioClip AudioData {
		get { return m_unity_object as AudioClip;	}
	}

	// ゲームオブジェクト
	public GameObject GameObjectData {
		get { return m_unity_object as GameObject;	}
	}
	// テキスト
	public TextAsset TextData2{
		get { return m_unity_object as TextAsset; }
	}

	public ContentInformation( string aFileName , int aVersion )
	{
		m_file_name = aFileName;
		m_content_version = aVersion;
	}
}

public class ContentsDownloadModel : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ( IsCompleateDownload() ) {
			on_compleat (m_req_contents);
			m_req_contents.Clear();
		}
	}

	string m_url = "210.140.154.119:81/assetbandle/";
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
	public enmDownloadStatus DownloadStatus
	{
		get{ return m_download_status; }
	}

	public delegate void OnComplete ( List<ContentInformation>aContentInformation );
	public OnComplete on_compleat = null;
	public bool m_is_req_download = false;

	private List<ContentInformation> m_req_contents = new List<ContentInformation>();

	private int m_read_thread_count = 1;
	public int ReadThreadCount {
		get { return m_read_thread_count;	}
		set { m_read_thread_count = value; }
	}

	public void RequestDownloadFiles( List<ContentInformation> aReqDownloadList )
	{
		int count = 0;
		foreach( ContentInformation m_req_contents in aReqDownloadList )
		{
			if( m_req_contents.Download_Status != enmDownloadStatus.WAITING_FOR_START )
				continue;

			if( m_read_thread_count > aReqDownloadList.Count )
				continue;

			GetAssetBandle( m_req_contents.File_Name , m_req_contents.Version );
			count++;
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
		string url = "http://" + this.m_url + aDownloadFileName + ".unity3d.android.unity3d?dl=1";

#elif UNITY_IPHONE  && !UNITY_EDITOR
		string url = "http://" + this.m_url + aDownloadFileName + ".unity3d.iphone.unity3d?dl=1";
#else
		string url = "http://" + this.m_url + aDownloadFileName + ".unity3d.unity3d?dl=2";
#endif

		StartCoroutine ( DownloadAndCache( content_info, url, 0 ) );

	}

	private IEnumerator DownloadAndCache ( ContentInformation aContentInformation, string aUrl, int aVersion)
	{

		// キャッシュシステムの準備が完了するのを待ちます
		while (!Caching.ready)
			yield return null;

		SetDownloadStatus ( aContentInformation.File_Name, enmDownloadStatus.DOWNLOADING);

		// 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードする
		// またはダウンロードしてキャッシュに格納します。
		using (WWW w3 = WWW.LoadFromCacheOrDownload(aUrl, aVersion) )
		{
			// ファイル名でオブジェクト配列を検索
			int req_idx = -1;
			foreach (ContentInformation req in m_req_contents) {
				if( req.File_Name == aContentInformation.File_Name )
				{
					req.Download_Status = enmDownloadStatus.DOWNLOADING;
					req_idx = m_req_contents.IndexOf( req );
					break;
				}
			}

			if( req_idx <= -1 )
				throw new Exception ("ダウンロード対象でない:" + w3.error);

			yield return w3;

			if ( w3.error != null ) {
				SetDownloadStatus (aContentInformation.File_Name, enmDownloadStatus.ERROR_EXIT);
				throw new Exception ("WWWダウンロードにエラーがありました:" + w3.error);
			}
			else
			{
				m_req_contents[req_idx].UnityObjectData = w3.assetBundle.mainAsset;
				if (aUrl.IndexOf("texture") > -1)
				{
					//画像
					m_req_contents[req_idx].Content_Type = enmDownloadContentType.TEXTURE;

				}
				else if (aUrl.IndexOf("3d") > -1)
				{
					//3Dデータ
					m_req_contents[req_idx].Content_Type = enmDownloadContentType.GAMEOBJECT;
				}
				else if (aUrl.IndexOf("audio") > -1)
				{
					//サウンド
					m_req_contents[req_idx].Content_Type = enmDownloadContentType.AUDIO;
				}
				else if (aUrl.IndexOf("text") > -1 )
				{
					// テキスト
					m_req_contents[req_idx].Content_Type = enmDownloadContentType.TEXT;
				}
				else
				{
					// 不明なコンテンツ
					m_req_contents[req_idx].Content_Type = enmDownloadContentType.UNKNOWN;
				}

			}

			SetDownloadStatus (aContentInformation.File_Name, enmDownloadStatus.COMPLETE);

		}
		yield return null;
	}

	public void SetDownloadStatus( string aAssetName , enmDownloadStatus aStatus )
	{
		foreach (ContentInformation req in m_req_contents) {
			if( req.File_Name == aAssetName )
				req.Download_Status = aStatus;
		}
	}

	private bool IsCompleateDownload()
	{
		bool ret = false;

		foreach (ContentInformation req in m_req_contents) {
			if( req.Download_Status == enmDownloadStatus.WAITING_FOR_START ||
				req.Download_Status == enmDownloadStatus.DOWNLOADING )
				return false;
			else if( req.Download_Status == enmDownloadStatus.COMPLETE ||
					 req.Download_Status == enmDownloadStatus.ERROR_EXIT )
				ret = true;
		}

		return ret;
	}
}
