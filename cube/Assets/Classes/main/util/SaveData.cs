using System;
using System.Collections;
using System.Collections.Generic;

// ゲームデータクラス
[System.Serializable()]
public class SaveData
{
	
	private string m_name;
	public string _name {
		get { return m_name; }
		set { m_name = value; }
	}

	public string m_session_key;
	public string _session_key {
		get { return m_session_key; }
		set { m_session_key = value; }
	}

	[System.Serializable()]
	public class GameData
	{
		private uint m_score;
		public uint _score{
			get { return m_score; }
			set { m_score = value; }
		}

		public uint m_time;
		public uint _time{
			get { return m_time; }
			set { m_time = value; }
		}
	}
	
	public List<GameData> m_game_datas;
	public List<GameData> _game_datas{
		get { return m_game_datas; }
		set { m_game_datas = value; }
	}
	
}

