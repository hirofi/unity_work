using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

public struct EventStruct
{
	public int from;
	public int to;
	public string subject;
}

class DelegateObject : MonoBehaviour
{
	public delegate void OnEventDelegate( GameObject aTarget, EventStruct aMsg );
	public event OnEventDelegate onEvent;

	public void DoEvent( EventStruct aMsg )
	{
		onEvent ( gameObject , aMsg );
	}
	
}
