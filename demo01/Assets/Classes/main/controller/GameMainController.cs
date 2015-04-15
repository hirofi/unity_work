using UnityEngine;
using System.Collections;

public class GameMainController : MonoBehaviour {


	void Awake()
	{
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

	public void OnDownloadStart()
	{
		;
	}

}
