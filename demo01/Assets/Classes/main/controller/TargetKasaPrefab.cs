using UnityEngine;
using System.Collections;

public class TargetKasaPrefab : MonoBehaviour {

	// マトの状態
	public enum enmTargetStatus
	{
		READY,
		START,
		HIT
	}
	
	private enmTargetStatus target_status;
	
	// Use this for initialization
	void Start () {
		;
	}
	
	// Update is called once per frame
	void Update () {
		;
	}
	
	// 当たりが発生したらイベントマネージャに通知(イベントキュー)
	public void DoHit( int aBouns , int aScore )
	{
		EventManagerController.Instance.TriggerEvent (new TargetKasaEventController( aBouns , aScore ) );
	}
	
	private void OnCollisionEnter(Collision aCollision)
	{
		
		// ball か確認する
		if (aCollision.gameObject.name.IndexOf ("ball") >= 0) {
		//	effectHit();
			// 当たりにつきイベント発行
			DoHit( 20, 10 );
		}
		
	}
	
	private void effectHit()
	{
		GetComponent<HingeJoint> ().useSpring = false;
	}

	// アニメーションを初期状態にする
	public void anim_ready()
	{
		;//getHingeJoint().useSpring = false;
	}
	
	// 開始
	public void anim_start()
	{
		// ヒンジのスプリングを有効にし、値をセット
		;//getHingeJoint ().useSpring = true;
	}
}
