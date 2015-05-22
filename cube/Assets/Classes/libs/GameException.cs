using UnityEngine;
using System;
using System.Collections;

public class GameException : SystemException {

	private string m_caller_info;
	public string _caller_info
	{
		get { return m_caller_info; }
	}
	private UInt32 m_exception_code;

	public enum enmExceptionDifficulty
	{
		IGNORE = 0,
		USER_ALERT,
		RETRY,
		APP_EXIT,
		ENTRY_MAX
	}

	private enmExceptionDifficulty m_difficulty;
	public enmExceptionDifficulty _difficulty
	{
		get { return m_difficulty; 	}
		set { m_difficulty = value;	}
	}

	public GameException(
		string p_message ,
		UInt32 p_code = 0x00000001 ,
		enmExceptionDifficulty p_difficulty = enmExceptionDifficulty.IGNORE ): base( p_message )
	{
		m_exception_code = p_code;
		System.Diagnostics.StackTrace stack  = new System.Diagnostics.StackTrace(false);
		m_caller_info = stack.GetFrame (1).GetMethod().DeclaringType.FullName;
	}

}
