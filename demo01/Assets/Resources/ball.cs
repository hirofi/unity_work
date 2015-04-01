using UnityEngine;
using System.Collections;

public class ball : MonoBehaviour {

	private Vector3 target_point;
	private float shot_force = 0f;
	private float move_speed = 0f;

	public void init( Vector3 aTargetPoint, float aShotFrorce, float aMoveSpeed ){
		target_point = aTargetPoint;
		shot_force = aShotFrorce;
		move_speed = aMoveSpeed;

		Rigidbody rg = getRigidbody ();
		rg.useGravity = true;//false;

		ConstantForce cf = rg.GetComponent<ConstantForce> ();
//		cf.force = target_point*aShotFrorce;//new Vector3(0,0,10);
		cf.force = target_point * aShotFrorce;
		cf.enabled = true;

		Destroy (getGameObj(), 5);
	}

	private Rigidbody getRigidbody() {
		var obj_path = "/" + this.name;
		return GameObject.Find(obj_path).GetComponent<Rigidbody>();
	}

	private GameObject getGameObj() {
		var obj_path = "/" + this.name;
		return GameObject.Find(obj_path);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		addMyForce ();
	}


	void addMyForce()
	{
		float h = Input.GetAxis("Horizontal") * Time.deltaTime * move_speed;
		float v = Input.GetAxis("Vertical") * Time.deltaTime * move_speed;

		Rigidbody rg = getRigidbody ();
		rg.AddForce (new Vector3 (h, v, 0f));
/*
			transform.Translate(new Vector3(h, v, 0f));
		
		var myforce = new Vector3 (0, 0, shot_force);
		Rigidbody myball = getRigidbody();
		myball.AddForce( myforce);
*/
	}

	public Camera _setCamera = null;
	void destroy()
	{
		if (_setCamera == null)
			_setCamera = Camera.main;

		Vector3 left_top = _setCamera.ScreenToWorldPoint( new Vector3( 0, 0, 10.0f) );
		Vector3 right_down = _setCamera.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 10.0f));

		Rigidbody rg = getRigidbody ();


	}
}
