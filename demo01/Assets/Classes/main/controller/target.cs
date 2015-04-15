using UnityEngine;
using System.Collections;

public class target : MonoBehaviour {

	public enum enmTargetStatus
	{
		READY,
		START,
		HIT
	}

	private enmTargetStatus target_status;

	// Use this for initialization
	void Start () {
		target_status = enmTargetStatus.READY;
	}
	
	// Update is called once per frame
	void Update () {
		;
	}

	////////////////
	private GameObject getGameObject( string aObjName )
	{
		var obj_path = "/" + this.name + "/" + aObjName ; 
		return GameObject.Find(obj_path);
	}

	private HingeJoint getHingeJoint()
	{
		var obj_path = "/" + this.name + "/base_x1"; 
		return GameObject.Find (obj_path).GetComponent<HingeJoint> ();
	}

	// アニメーションを初期状態にする
	public void anim_ready()
	{
		getHingeJoint().useSpring = false;
	}

	// 開始
	public void anim_start()
	{
		// ヒンジのスプリングを有効にし、値をセット
		getHingeJoint ().useSpring = true;
	}

	// 的のステータスを取得
	public enmTargetStatus getStart()
	{
		return target_status;
	}

	public GameObject getTargetCylinder()
	{
		return this.getGameObject("Cylinder 1");
	}

}
