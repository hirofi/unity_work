using UnityEngine;
using System.Collections;

public class SceneDispater : SingletonMonoBehaviour<SceneDispater> {

	private EventManagerDynamic m_event_manager = null;
	public EventManagerDynamic _event_manager{
		get { return m_event_manager; 	}
		set { m_event_manager = value;	}
	}
	
	void Awake()
	{
		if (m_event_manager == null)
			m_event_manager = new EventManagerDynamic ();
	}

}
