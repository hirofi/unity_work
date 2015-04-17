using UnityEngine;
using System.Collections;
using System;

public class DownloadController /* : MonoBehaviour */ {


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
/*
	void Awake()
	{

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
*/
	private ContentsDownloadApiModel m_downloadmodel;
	public void StartDownload()
	{

		try
		{
			// ContentsDownloadApiModelクラスは MonoBehaviour を継承しているので
			// new ではなく GameObject に AddComponet して生成する。。。 unity お作法により 
			GameObject emptyGameObject = new GameObject();
			m_downloadmodel = emptyGameObject.AddComponent<ContentsDownloadApiModel> ();

			m_downloadmodel.GetAssetBandle ( this.m_file_name );
		}
		catch
		{
			Debug.Log("catch");
		}
		finally
		{
			OnDownloadCompleate ();
		}

	}

	// ダウンロードが終了したいらイベントマネージャに通知(イベントキュー)
	private void OnDownloadCompleate()
	{
		EventManagerController.Instance.QueueEvent ( new DownloadEventController(this.m_file_name) );
	}

}
