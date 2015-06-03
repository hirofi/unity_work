using UnityEngine;
using System.Collections;

public class MoverView : MonoBehaviour {

	MoverController m_controller = null;

	public MoverView( MoverController p_controller )
	{
		m_controller = p_controller;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void f_DoNotify()
	{
		m_controller._event_manager.f_Dispatch (
			new MoverEvent(
				MoverEvent.enmMoverBehavior.MOVE,
				MoverEvent.enmUserOperationOrder.MOVE,
				m_prev_position,
				m_now_position )
		);
	}

	Vector3 m_prev_position;
	Vector3 m_now_position;
	bool m_is_drag = false;
	void OnMouseDown()
	{
		m_prev_position = transform.position;
		m_is_drag = true;
	}

	void OnMouseUp()
	{
		Debug.Log ("OnMouseUp");

		m_now_position = transform.position;
		m_prev_position.Set(0.0f,0.0f,0.0f);
		m_is_drag = false;

	}
}
