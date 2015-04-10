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
		cf.force = target_point * aShotFrorce;
//		cf.relativeForce = target_point * aShotFrorce;
		cf.enabled = false;

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
		addMyForce ();
	}

	private const float START_SCALE = 2;
	float m_scale = START_SCALE;
	void addMyForce()
	{
//		float h = Input.GetAxis("Horizontal") * Time.deltaTime * move_speed;
//		float v = Input.GetAxis("Vertical") * Time.deltaTime * move_speed;

		Vector3 force = target_point * Time.deltaTime * move_speed;
		move_speed -= 2;
		Rigidbody rg = getRigidbody ();
		rg.AddForce ( force ,ForceMode.Impulse );

		if (m_scale > 1) {
			m_scale -= 0.3f;
			rg.mass -= 0.01f;
			rg.transform.localScale = new Vector3 (m_scale, m_scale, m_scale);
		} else {
			rg.mass = 1;
		}
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
