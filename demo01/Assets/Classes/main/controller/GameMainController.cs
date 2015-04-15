using UnityEngine;
using System.Collections;

public class GameMainController : MonoBehaviour {

	void Awake()
	{
		SetupListeners ();

	//	m_target = new TargetEventController;
	//	EventManager.Instance.AddListener<TargetEventController> (OnHit);
		GameObject main = GameObject.Find ("EventManagerGameObject");
		DontDestroyOnLoad( main );
	}

	// Use this for initialization
	void Start () {
//		m_event_manager.Instance.AddListener<MainEventController>( OnHit );

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnHit( TargetEventController aEvent )
	{
		int a = 0;
		a++;
	}

	// 
	// クリック時に呼び出されるようにインスペクタで設定される
	public void OnClickGameStart()
	{
		Application.LoadLevel ( "s02" );
	}

	public void OnClickStartDownload()
	{
		DownloadController download = new DownloadController ();

		download.FileName = "pf_target";
		download.DomainName = "pshpz01.isl.gacha.fujitv.co.jp/unity/";

		download.StartDownload();
	}

	//　ダウンロードイベントリスナーの登録
	public void SetupListeners()
	{
		EventManager.Instance.AddListener<DownloadEventController> (OnCompleateDownload);
	}
	
	public void Dispose()
	{
		EventManager.Instance.RemoveListener<DownloadEventController> (OnCompleateDownload);
	}
	
	public void OnCompleateDownload( DownloadEventController aEvent )
	{
		Debug.Log ("downloaded.");
	}

}
