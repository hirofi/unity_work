﻿using UnityEngine;
using System.Collections;

public class target_cylinder : MonoBehaviour {

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

	private EventManagerDynamic m_event_manage_base = null;
	public void SetEventManager( EventManagerDynamic aManagerBase )
	{
		m_event_manage_base = aManagerBase;
	}

	// 当たりが発生したらイベントマネージャに通知(イベントキュー)
	public void DoHit( int aScore )
	{
//		EventManagerController.Instance.QueueEvent (new TargetEventController( aScore ) );
		m_event_manage_base.TriggerEvent ( new TargetEventController(aScore) );
	}

	private void OnCollisionEnter(Collision aCollision)
	{

		// ball か確認する
		if (aCollision.gameObject.name.IndexOf ("ball") >= 0) {
			effectHit();
			// 当たりにつきイベント発行
			DoHit( 10 );
		}

	}

	private void effectHit()
	{
		GetComponent<HingeJoint> ().useSpring = false;
	}

}
