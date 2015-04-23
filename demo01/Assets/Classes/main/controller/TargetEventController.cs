using UnityEngine;
using System.Collections;

public class TargetEventController : GameEventDynamic {

	private int m_score;
	public TargetEventController( int aScore )
	{
		this.m_score = aScore;
	}

	public int Score {
		get { return m_score; }
		set { m_score = value;	}
	}
}
