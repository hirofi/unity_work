using UnityEngine;
using System.Collections;

public class DownloadEventController : GameEvent {


	enum enmDownloadStatus
	{
		SERVER_ERROR,
		ENTRY_MAX
	}

	private string m_url;
	private string m_file_name;

	public DownloadEventController( string aFileName )
	{
		this.m_file_name = aFileName;
	}


}
