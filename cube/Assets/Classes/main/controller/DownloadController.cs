using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DownloadToken
{
	private int m_idx;
	public int Index {
		get { return m_idx; 	}
		set { m_idx = value;	}
	}

	private Vector3 m_position;
	public Vector3 Postion{
		get { return m_position; 	}
		set { m_position = value;	}
	}

	private string m_name;
	public string Name{
		get { return m_name; 	}
		set { m_name = value;	}
	}

	private Texture m_texture;
	public Texture Textuer{
		get { return m_texture; 	}
		set { m_texture = value;	}
	}
}

public class DownloadController : EventManagerDynamic {

	const string DEFAULT_DOMAIN = "http://210.140.154.119:82/ab/";

	private string m_file_name;
	public string p_FileName {
		get { return m_file_name; }
		set { m_file_name = value;	}
	}

	private string m_domain_name;
	public string p_DomainName {
		get { return m_domain_name; }
		set { m_domain_name = value;	}
	}

	private DownloadToken m_token;
	public DownloadToken p_Token {
		get { return m_token;	}
		set { m_token = value;	}
	}

	private bool m_request_save_data = false;
	public bool _request_save_data {
		get { return m_request_save_data;	}
		set { m_request_save_data = value;	}
	}

	private List<ContentInformation> m_download_list;
	public List<ContentInformation> p_DownloadList {
		get { return m_download_list;	}
		set { m_download_list = value;	}
	}

	private DownloadEvent.enmDownloadFileType m_download_file_type;

	private ContentsDownloadModel m_downloadmodel;
	public void StartDownload()
	{

	Debug.Log ("StartDownload");

		try
		{
			if( m_file_name == null )
				return;

			m_download_file_type = DownloadEvent.enmDownloadFileType.DOWNLOAD_ORDER_LIST;

			// ダウンロードするファイル名をリスト化しリクエストする。
			var downloadlist = new List<ContentInformation>();
			downloadlist.Add( new ContentInformation(m_file_name,0) );

			// ContentsDownloadModelクラスは MonoBehaviour を継承しているので
			// new ではなく GameObject に AddComponet して生成する。。。 unity お作法により 

			GameObject emptyGameObject = new GameObject();
			m_downloadmodel = emptyGameObject.AddComponent<ContentsDownloadModel> ();
			m_downloadmodel._domain = (p_DomainName == null ) ? DEFAULT_DOMAIN : p_DomainName;
			m_downloadmodel.on_compleat = OnDownloadCompleate;
			m_downloadmodel.RequestDownloadFiles(downloadlist , true, false);
		}
		catch
		{
			Debug.Log("catch");
		}
		finally
		{
			;
		}

	}

	public void StartDownloadAssetBandles()
	{

	Debug.Log ("StartDownloadAssetBandles");

		try
		{
			if( m_download_list == null )
				return;

			m_download_file_type = DownloadEvent.enmDownloadFileType.DOWNLOAD_ASSETBANDLE;

			// ContentsDownloadModelクラスは MonoBehaviour を継承しているので
			// new ではなく GameObject に AddComponet して生成する。。。 unity お作法により 
			GameObject emptyGameObject = new GameObject();
			m_downloadmodel = emptyGameObject.AddComponent<ContentsDownloadModel> ();
			m_downloadmodel._domain = (p_DomainName == null ) ? DEFAULT_DOMAIN : p_DomainName;
			m_downloadmodel.on_compleat = OnDownloadCompleate;
			m_downloadmodel.RequestDownloadFiles( m_download_list , false, false);
		}
		catch
		{
			Debug.Log("catch");
		}
		finally
		{
			;
		}

	}

	// ダウンロードが終了したらイベントマネージャに通知(イベントキュー)
	private void OnDownloadCompleate( List<ContentInformation>aContentInformation )
	{

	Debug.Log ("OnDownloadCompleate");

		DownloadEvent download_event = new DownloadEvent ( aContentInformation );
		download_event.p_DownloadFileType = m_download_file_type;

		Dispatch(download_event);

		// ダウンロードリクエスト対象をクリア
		m_file_name = null;
		m_download_list = null;
	}
	
}
