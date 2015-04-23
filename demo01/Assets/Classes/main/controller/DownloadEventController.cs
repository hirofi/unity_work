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

	private GameObject m_game_object;
	public GameObject Game_Object {
		get { return m_game_object;	}
		set { m_game_object = value;	}
	}

	private Texture2D m_texture = null;
	public Texture2D TextureData {
		get { return m_texture;	}
		set { m_texture = value;	}
	}

	private GameObject m_gameobject = null;
	public GameObject GameobjectData {
		get { return m_gameobject;	}
		set { m_gameobject = value;	}
	}

	private AudioClip m_audio = null;
	public AudioClip AudioData {
		get { return m_audio;	}
		set { m_audio = value;	}
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
