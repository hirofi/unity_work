using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ContentInformation
{
	private string m_file_name = null;
	public string Filename {
		get { return m_file_name;	}
		set { m_file_name = value;	}
	}

	private ContentsDownloadApiModel.enmDownloadStatus m_download_status = 0;
	public ContentsDownloadApiModel.enmDownloadStatus Download_Status {
		get { return m_download_status;	}
		set { m_download_status = value;}
	}

	private int m_content_type = 0;
	public int Content_Type {
		get { return m_content_type;	}
		set { m_content_type = value;	}
	}

	public ContentInformation( string aFileName )
	{
		m_file_name = aFileName;
	}
}

public class ContentsDownloadApiModel : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ( IsCompleateDownload() ) {
			on_compleat (req_contents);
			req_contents.Clear();
		}
	}

	string m_url = "pshpz01.isl.gacha.fujitv.co.jp/unity/";

	private GameObject m_gameobject = null;
	public GameObject GameObjectData {
		get { return m_gameobject;	}
	}

	private AudioClip m_audio = null;
	public AudioClip AudioData {
		get { return m_audio;	}
	}

	private Texture2D m_texture = null;
	public Texture2D TextureData {
		get { return m_texture;	}
	}

	public enum enmDownloadStatus
	{
		WAITING_FOR_START,
		DOWNLOADING,
		COMPLETE,
		ERROR_EXIT,
		ENTRY_MAX
	}

	public enum enmDownloadContentType
	{
		UNKNOWN,
		TEXTURE,
		GAMEOBJECT,
		AUDIO,
		ENTRY_MAX
	}

	private enmDownloadContentType m_download_content_type;
	public enmDownloadContentType DownloadContentType {
		get { return m_download_content_type; }
	}

	private enmDownloadStatus m_download_status;
	public enmDownloadStatus DownloadStatus
	{
		get{ return m_download_status; }
	}

	public delegate void OnComplete ( List<ContentInformation>aContentInformation );
	public OnComplete on_compleat = null;
	public bool m_is_req_download = false;

	private List<ContentInformation> req_contents = new List<ContentInformation>();

	public void RequestDownloadFiles( List<string> aFileNameList )
	{
		foreach( string file_name in aFileNameList ){
			GetAssetBandle( file_name );
		}
	}

	/// <summary>
	/// アセットをダウンロードする。
	/// 既にキャッシュがあればそちらから取り出す。
	/// </summary>
	/// <param name="aDownloadFileName">A download file name.</param>
	public void GetAssetBandle( string aDownloadFileName )
	{

		req_contents.Add( new ContentInformation(aDownloadFileName));

		// Clear Cache
		Caching.CleanCache();

#if   UNITY_ANDROID && !UNITY_EDITOR
		string url = "http://" + this.m_url + aDownloadFileName + ".unity3d.android.unity3d?dl=1";

#elif UNITY_IPHONE  && !UNITY_EDITOR
		string url = "http://" + this.m_url + aDownloadFileName + ".unity3d.iphone.unity3d?dl=1";
#else
		string url = "http://" + this.m_url + aDownloadFileName + ".unity3d.unity3d?dl=1";
#endif
		Debug.Log ("Download URL=" + url);

		StartCoroutine ( DownloadAndCache( aDownloadFileName, url, 0 ) );

	}

	private IEnumerator DownloadAndCache (string aAssetName, string aUrl, int aVersion)
	{

		// キャッシュシステムの準備が完了するのを待ちます
		while (!Caching.ready)
			yield return null;

		SetDownloadStatus ( aAssetName, enmDownloadStatus.DOWNLOADING);

		// 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードする
		// またはダウンロードしてキャッシュに格納します。
		using (WWW w3 = WWW.LoadFromCacheOrDownload(aUrl, aVersion) )
		{
			foreach (ContentInformation req in req_contents) {
				if( req.Filename == aAssetName )
					req.Download_Status = enmDownloadStatus.DOWNLOADING;
			}

			yield return w3;

			if ( w3.error != null ) {
				SetDownloadStatus (aAssetName, enmDownloadStatus.ERROR_EXIT);
				throw new Exception ("WWWダウンロードにエラーがありました:" + w3.error);
			}
			else
			{
				if (aUrl.IndexOf("texture") > -1) {
					//画像
					m_texture = w3.assetBundle.mainAsset as Texture2D;
					m_download_content_type = enmDownloadContentType.TEXTURE;
				} else if (aUrl.IndexOf("3d") > -1) {
					//3Dデータ
					m_gameobject = Instantiate(w3.assetBundle.mainAsset) as GameObject;
					m_download_content_type = enmDownloadContentType.GAMEOBJECT;
				} else if (aUrl.IndexOf("audio") > -1) {
					//サウンド
					m_audio = w3.assetBundle.mainAsset as AudioClip;
					m_download_content_type = enmDownloadContentType.AUDIO;
				}
			}

			SetDownloadStatus (aAssetName, enmDownloadStatus.COMPLETE);

		}

		yield return null;

		Debug.Log(Caching.IsVersionCached(aUrl, 1));
		Debug.Log("DownloadAndCache end");
	}

	public void SetDownloadStatus( string aAssetName , enmDownloadStatus aStatus )
	{
		foreach (ContentInformation req in req_contents) {
			if( req.Filename == aAssetName )
				req.Download_Status = aStatus;
		}
	}

	private bool IsCompleateDownload()
	{
		bool ret = false;

		foreach (ContentInformation req in req_contents) {
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
