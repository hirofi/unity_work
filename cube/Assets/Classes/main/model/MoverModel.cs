using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoverModel {

	private int m_life_step;
	public int _life_step{ get{ return m_life_step;} set { m_life_step = value;	} }

	private float m_body_radius;
	public float _body_radius{ get{ return m_body_radius;} set { m_body_radius = value;	} }

	private int m_power;
	public int _power{ get{ return m_power;} set { m_power = value;	} }

	private int m_speed;
	public int _speed{ get{ return m_speed;} set { m_speed = value;	} }

	private int m_anchor_point;
	public int _anchor_point{ get{ return m_anchor_point;} set { m_anchor_point = value;	} }

	private int m_territory_size;
	public int _territory_size{ get{ return m_territory_size;} set { m_territory_size = value;	} }

	private int m_sensitivity;
	public int _sensitivity{ get{ return m_sensitivity;} set { m_sensitivity = value;	} }


	private Vector3 m_position; // 現在の位置
	public Vector3 _position{ get{ return m_position;} set { m_position = value;	} }

	private float m_search_radius; // 検索半径
	public float _search_radius{ get{ return m_search_radius;} set { m_search_radius = value;	} }

	private List<MoverModel> m_tartget; // ターゲットリスト
	public List<MoverModel> _tartget{ get{ return m_tartget;} set { m_tartget = value;	} }

	private int m_lock_on_target_num; // 対象ターゲット番号 : なければ -1
	public int _lock_on_target_num{ get{ return m_lock_on_target_num;} set { m_lock_on_target_num = value;	} }

	// 性格
	enum enmPersonality
	{
		BATTLEFUL,
		FRIENDLY,
		INTROVERT,
		ENTRY_MAX
	}
	enmPersonality m_personality;

	// 
	enum enmTraceType
	{
		TARGET_TO_LINE,
		RANDOM_TO_LINE,
		ENTRY_MAX
	}
	enmTraceType m_trace_type;


	public void f_Behavior()
	{
		m_power--;
	}

	// 周囲検索
	public void f_Search( MoverModel p_the_other )
	{
		// ターゲット検索
		if (f_Collision (p_the_other))
		{
			if(m_tartget == null)
				m_tartget = new List<MoverModel>();

			// 範囲内になにかあれば保持
			m_tartget.Add( p_the_other );
		}
	}

	// 移動
	public Vector3 f_MoveTo()
	{

		Vector3 next_potion;

		if ( m_tartget.Count > 0 && m_lock_on_target_num >= 0) 
		{

			Vector3 v =  m_tartget[m_lock_on_target_num]._position - m_position;

			float l = Mathf.Sqrt ( (v.x * v.x) + (v.y * v.y) + (v.z * v.z) );
			next_potion = (l > 0) ? new Vector3 (v.x / l, v.y / l, v.z / l) : new Vector3 (0, 0, 0);

			next_potion = next_potion * m_speed;
		}
		else
		{
			next_potion = new Vector3( (float)Random.Range(0,m_speed) , (float)Random.Range(0,m_speed), (float)	Random.Range(0,m_speed) );
		}

		return next_potion;
				
	}

	bool f_Collision(MoverModel p_target_mover )
	{
		float dx,dy,r;
		dx = m_position.x-p_target_mover._position.x;	// 水平方向の距離
		dy = m_position.y-p_target_mover._position.y;	// 鉛直方向の距離
		r = m_search_radius + p_target_mover._body_radius;	// 検索範囲と、ターゲットのボディサイズ半径の和
		
		//三平方の定理
		return ((dx*dx)+(dy*dy)<(r*r));			// 当たっていたらtrue
	}


}
