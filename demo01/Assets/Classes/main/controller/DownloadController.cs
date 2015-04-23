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

}

public class DownloadController {


	private EventManagerDynamic m_event_manager;

	private string m_file_name;
	public string FileName {
		get { return m_file_name; }
		set { m_file_name = value;	}
	}

	private string m_domain_name;
	public string DomainName {
		get { return m_domain_name; }
		set { m_domain_name = value;	}
	}

	private DownloadToken m_token;
	public DownloadToken Token {
		get { return m_token;	}
		set { m_token = value;	}
	}

	// ダウンロードステータス通知の為にイベントマネージャを登録
	public void SetEventManager( EventManagerDynamic aEventManager )
	{
		m_event_manager = aEventManager;
	}

	private ContentsDownloadApiModel m_downloadmodel;
	public void StartDownload()
	{

		try
		{
			// ダウンロードするファイル名をリスト化しリクエストする。
			var downloadlist = new List<string>();
			downloadlist.Add(this.m_file_name);

			// ContentsDownloadApiModelクラスは MonoBehaviour を継承しているので
			// new ではなく GameObject に AddComponet して生成する。。。 unity お作法により 
			GameObject emptyGameObject = new GameObject();
			m_downloadmodel = emptyGameObject.AddComponent<ContentsDownloadApiModel> ();
			m_downloadmodel.RequestDownloadFiles(downloadlist);
			m_downloadmodel.on_compleat = this.OnDownloadCompleate;
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

	// ダウンロードが終了したいらイベントマネージャに通知(イベントキュー)
	private void OnDownloadCompleate( List<ContentInformation>aContentInformation )
	{
		DownloadEventController download_event = new DownloadEventController (this.m_file_name);
		download_event.Token = m_token;
		download_event.TextureData = m_downloadmodel.TextureData;
		download_event.GameobjectData = m_downloadmodel.GameObjectData;
		download_event.AudioData = m_downloadmodel.AudioData;

		m_event_manager.TriggerEvent (download_event);
	}

}
