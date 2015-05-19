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

	private string m_file_name;
	public string _file_name {
		get { 
			if( m_file_path != null )
				return System.IO.Path.GetFileName (m_file_path);
			else
				return null;
		}
	}

	private string m_file_extention;
	public string _file_extention {
		get { 
			if( m_file_path != null )
				return System.IO.Path.GetExtension (m_file_path);
			else
				return null;
		}
	}

	private int m_loop_count;
	public int _loop_count{
		get { return m_loop_count; }
		set { m_loop_count = value; }
	}

	private AudioClip m_audio_clip;
	public AudioClip _audio_clip
	{
		get { return m_audio_clip; }
		set { m_audio_clip = value; }
	}

	private AudioSource m_audio_source;
	public AudioSource _audio_source
	{
		get { return m_audio_source; }
		set { m_audio_source = value; }
	}

	// リストを使用するので一意性を担保する為にハッシュ値を持つ
	private int m_hash;

	public SoundInformation( string p_file_name )
	{
		m_file_path = p_file_name;
		float t = Time.realtimeSinceStartup;

		string st = p_file_name + t.ToString ();
		m_hash = st.GetHashCode();
	}

}

public class SoundController : SingletonMonoBehaviour<SoundController>
{
	public const int SE_CHANEL_MAX = 3;

	private static SoundController s_Instance;

	EventManagerDynamic m_event_manager;

	private List<SoundInformation> m_info_list = new List<SoundInformation>();
	private List<GameObject> m_go_list = new List<GameObject>();

	private SoundModel m_sound_model = null;
	private GameObject m_sound_game_object = null;

	AudioSource m_sound_source_bgm;
	AudioSource[] m_sound_source_se = new AudioSource[SE_CHANEL_MAX];

	public void Awake()
	{
		if (this != Instance) {
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		GameObject emptyGameObject = new GameObject("SoundModel");
		m_sound_model = emptyGameObject.AddComponent<SoundModel> ();

		m_sound_game_object = new GameObject ("SoundGameObject");
		if (m_sound_game_object == null) {
			Debug.LogError("ERROR : new m_sound_game_objcet");
		}
	}

	private void f_init_event_manager()
	{
		m_event_manager = new EventManagerDynamic();
	}

	public bool f_AttachList( List<string> p_request_list )
	{
		if (p_request_list == null)
			return false;

		bool is_new = true;
		foreach (string request in p_request_list) {
			f_Attach( request );
		}

		return true;
	}

	private SoundInformation f_search_information(string p_name)
	{
		if (m_info_list == null || m_info_list.Count < 0)
			return null;

		foreach (SoundInformation info in m_info_list )
		{
			// 上書き
			if( p_name == info._file_name )
			{
				return info;
			}
		}

		return null;
	}

	// サウンド情報をアタッチする
	public SoundInformation f_Attach( string p_request_file_name )
	{

		SoundInformation info = f_search_information( p_request_file_name );
		
		// 既に存在する場合は上書き
		if (info != null)
		{
			return info;
		}
		// 無ければ追加
		else
		{
			AudioClip audio_clip = m_sound_model.f_AttachSoundFile( p_request_file_name );
			if( audio_clip != null )
			{
				info = new SoundInformation( p_request_file_name );
				info._audio_clip = audio_clip;
				info._audio_source = m_sound_game_object.AddComponent<AudioSource>();
				m_info_list.Add(info);
			}

		}

		Debug.Log("file_name="+p_request_file_name);

		return info;
	}

	public void f_Detach( SoundInformation p_request )
	{
		SoundInformation info = f_search_information( p_request._file_name );

		m_info_list.Remove (info);
	}

	private int m_bgm_loop_count = -1;
	// 初回プレー時、ファイル名からサウンド詳細を生成し再生開始
	public SoundInformation f_Play( string p_file_name, int p_loop_count = -1 )
	{
		SoundInformation info = f_search_information (p_file_name);
		if (info == null) {
			Debug.Log("Error:Unknown File =>"+p_file_name);
			return null;
		}

		info._loop_count = p_loop_count;
		info._audio_source.clip = info._audio_clip;
		info._audio_source.Play ();

		Debug.Log ("f_Play status="+info._audio_source.isPlaying +" file="+ p_file_name);

		return info;
	}

	// 次回からの再生
	public void f_Play( SoundInformation p_info )
	{
		if (p_info == null) {
			Debug.Log("Error: p_info is null ");
		}
		
		p_info._audio_source.Play();
		
		Debug.Log ("f_Stop status="+p_info._audio_source.isPlaying +" file="+ p_info._file_name);
	}

	// 停止
	public void f_Stop( SoundInformation p_info )
	{
		if (p_info == null) {
			Debug.Log("Error: p_info is null ");
		}

		p_info._audio_source.Stop();

		Debug.Log ("f_Stop status="+p_info._audio_source.isPlaying +" file="+ p_info._file_name);
	}

	// 一時停止
	public void f_Pause( SoundInformation p_info )
	{
		if (p_info == null) {
			Debug.Log("Error: p_info is null ");
		}
		
		p_info._audio_source.Pause();

		Debug.Log ("f_Pause status="+p_info._audio_source.isPlaying +" file="+ p_info._file_name);
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
