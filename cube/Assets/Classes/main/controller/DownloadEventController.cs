using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DownloadEventController : GameEventDynamic {


	public enum enmDownloadStatus
	{
		LOCAL_FILE_NOT_EXIST = 0,
		FILE_NOW_DOWNLOADING,
		LOCAL_FILE_EXIST,
		SERVER_ERROR,
		ENTRY_MAX
	}

	public enum enmDownloadFileType
	{
		DOWNLOAD_ORDER_LIST = 0,
		DOWNLOAD_ASSETBANDLE,
		ENTRY_MAX
	}

	enmDownloadFileType m_download_file_type;
	public enmDownloadFileType p_DownloadFileType {
		get { return m_download_file_type;	}
		set { m_download_file_type = value;	}
	}

	private WWW m_www = null;
	public WWW www {
		get { return m_www;		}
		set { m_www = value;	}
	}

	private DownloadToken m_token = null;
	public DownloadToken p_Token {
		get { return m_token;	}
		set { m_token = value;	}
	}

	// 複数ダウンロード時の取得コンテンツ一覧
	private List<ContentInformation> m_req_contents;
	public List<ContentInformation> p_RequestContents {
		get { return m_req_contents;	}
		set { m_req_contents = value;	}
	}

	public GameObject m_download_object = null;

	public DownloadEventController( List<ContentInformation> aRequestContents )
	{
		this.m_req_contents = aRequestContents;
	}
}
