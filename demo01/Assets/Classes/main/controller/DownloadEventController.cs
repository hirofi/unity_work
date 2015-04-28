using UnityEngine;
using System.Collections;

public class DownloadEventController : GameEventDynamic {


	enum enmDownloadStatus
	{
		LOCAL_FILE_NOT_EXIST,
		FILE_NOW_DOWNLOADING,
		LOCAL_FILE_EXIST,
		SERVER_ERROR,
		ENTRY_MAX
	}

	string m_file_name;
	public string FileName {
		get { return m_file_name;	}
		set { m_file_name = value;	}
	}

	string m_url;
	public string Url {
		get { return m_url;		}
		set { m_url = value;	}
	}

	private WWW m_www = null;
	public WWW www {
		get { return m_www;		}
		set { m_www = value;	}
	}

	private ContentInformation m_content;
	public ContentInformation Content
	{
		get { return m_content;	}
		set { m_content = value;}
	}

	private DownloadToken m_token = null;
	public DownloadToken Token {
		get { return m_token;	}
		set { m_token = value;	}
	}

	public GameObject m_download_object = null;

	public DownloadEventController( string aFileName )
	{
		this.m_file_name = aFileName;
	}

	public string getLocalFilePath()
	{
		string path = "file://" + Application.streamingAssetsPath + "/" + this.m_file_name;
		return path;
	}

}
