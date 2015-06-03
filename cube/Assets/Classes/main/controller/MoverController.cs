using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoverController : MonoBehaviour {

	MoverModel m_mover_model;
	MoverView m_mover_view;

	List<GameObject> m_movers = null;
	EventManagerDynamic m_event_manager = null;
	public EventManagerDynamic _event_manager {
		get { return m_event_manager; 	}
	}


	// Use this for initialization
	void Start ()
	{
		m_mover_model = new MoverModel ();
		m_mover_view = new MoverView (this);
		m_event_manager = new EventManagerDynamic ();


		m_event_manager.f_AddListener<MoverEvent> (f_MoverEventNotify);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void f_CreateMover( string p_name, Vector3 p_position )
	{

		GameObject obj_mover = Instantiate (Resources.Load ("Prefab/mover_01")) as GameObject;
		obj_mover.name = p_name;
		obj_mover.transform.position = p_position;
		m_movers.Add(obj_mover);

	}

	void f_MoverEventNotify(MoverEvent e)
	{
	}

}
