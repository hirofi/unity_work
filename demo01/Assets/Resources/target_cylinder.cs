using UnityEngine;
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
		}

	}


	private void effectHit()
	{
		GetComponent<HingeJoint> ().useSpring = false;
	}
}
