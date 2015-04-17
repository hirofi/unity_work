using UnityEngine;
using System.Collections;

public class TargetKasaEventController : TargetEventController {

	private int m_bonus;
//	private int m_score;
	public TargetKasaEventController( int aScore , int aBouns ) : base( aScore )
	{
		m_bonus = aBouns;
	}
	
	public int Bonus {
		get { return m_bonus; }
		set { m_bonus = value;	}
	}

}
