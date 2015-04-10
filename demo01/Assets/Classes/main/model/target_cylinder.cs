using UnityEngine;
using System.Collections;

public class target_cylinder : MonoBehaviour , IObserver {

	// マトの状態
	public enum enmTargetStatus
	{
		READY,
		START,
		HIT
	}
	
	private enmTargetStatus target_status;

// デリゲート

	// デリゲート：宣言
	public delegate void OnHitDelegate( GameObject aTarget, float aScore );

	// デリゲート:イベント発行時に呼び出される関数を登録する
	public event OnHitDelegate onHit;

	// デリゲート:イベント発行関数
	// 　この関数を呼び出すと委譲元の関数にパラメータを渡す
	public void DoHit( float aScore )
	{
		onHit ( gameObject, aScore);
	}

/*
	public delegate void OnEventDelegate( GameObject aTarget, EventStruct aMsg );
	
	// デリゲート:イベント発行時に呼び出される関数を登録する
	public event OnEventDelegate onHit;
	
	// デリゲート:イベント発行関数
	// 　この関数を呼び出すと委譲元の関数にパラメータを渡す
	public void DoEvent( EventStruct aMsg )
	{
		onHit ( gameObject, aMsg);
	}
*/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void OnCollisionEnter(Collision aCollision)
	{

		// ball か確認する
		if (aCollision.gameObject.name.IndexOf ("ball") >= 0) {
			effectHit();
			// 当たりにつきイベント発行
			DoHit(10);
/*
			EventStruct msg = new EventStruct();
			msg.subject = "10";
			DoEvent( msg );
*/
		}

	}
/*
	public void DoEvent( EventStruct aEvent )
	{
		onEvent ( GameObject , aEvent );
	}
*/
	private void effectHit()
	{
		GetComponent<HingeJoint> ().useSpring = false;
	}

	int score = 0;

	public int Score
	{
		get { return score; }
		set {
			if( value != score ) {
				score = value;
//				RaiseUpdate( "propScore" );
			}
		}
	}

	public void Notify(NotifyStruct notifySubject)
	{
		notifySubject.subject = "hit";
	}
	
}
