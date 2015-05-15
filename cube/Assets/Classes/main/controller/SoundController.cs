using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SoundInformation
{
	private string m_file_path;
	public string _file_path {
		get { return m_file_path; 	}
		set { m_file_path = value;	}
	}

	public string _file_name {
		get { 
			if( m_file_path != null )
				return System.IO.Path.GetFileName (m_file_path);
			else
				return null;
		}
	}

	public string _file_extention {
		get { 
			if( m_file_path != null )
				return System.IO.Path.GetExtension (m_file_path);
			else
				return null;
		}
	}

	// リストを使用するので一意性を担保する為にハッシュ値を持つ
	private int m_hash;

	SoundInformation( string p_file_name )
	{
		float t = Time.realtimeSinceStartup;

		string st = p_file_name + t.ToString ();
		m_hash = st.GetHashCode();
	}

}

public class SoundController
{

	private static SoundController s_Instance;

	EventManagerDynamic m_event_manager;
	private List<SoundInformation> m_sound_list;

	private SoundController()
	{

	}

	public static SoundController Instance {
		get {
			if( s_Instance == null )
			{
				s_Instance = new SoundController();
			}

			return s_Instance;
		}
	}

	// サウンド情報をアタッチする
	public void f_Attach( SoundInformation p_request )
	{
		m_sound_list.Add (p_request);
	}

	public void f_Detach( SoundInformation p_request )
	{
		m_sound_list.Remove (p_request);
	}


	public void f_Play( string p_file_name )
	{
		;
	}

	public void f_Stop()
	{
	}

	public void f_Pause()
	{
	}


	void f_OnChangeStatus( SoundEvent aEventDataAccess )
	{

	}

	void f_PauseComplete()
	{
	}

	void f_PlayComplete()
	{
	}

	void f_StopComplete()
	{
	}

}
