using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// サウンド情報
/// </summary>
public class SoundInformation
{
	// ファイルパス
	private string m_file_path;
	public string _file_path {
		get { return m_file_path; 	}
		set { m_file_path = value;	}
	}

	// ファイル名
	private string m_file_name;
	public string _file_name {
		get { 
			if( m_file_path != null )
				return System.IO.Path.GetFileName (m_file_path);
			else
				return null;
		}
	}

	// 拡張子
	private string m_file_extention;
	public string _file_extention {
		get { 
			if( m_file_path != null )
				return System.IO.Path.GetExtension (m_file_path);
			else
				return null;
		}
	}

	// 再生繰り返し数
	private int m_loop_count;
	public int _loop_count{
		get { return m_loop_count; }
		set { m_loop_count = value; }
	}

	// オーディオクリップ
	private AudioClip m_audio_clip;
	public AudioClip _audio_clip
	{
		get { return m_audio_clip; }
		set { m_audio_clip = value; }
	}

	// オーディイオソース
	private AudioSource m_audio_source;
	public AudioSource _audio_source
	{
		get { return m_audio_source; }
		set { m_audio_source = value; }
	}

	// 再生時間
	public float _play_time
	{
		get{ return (m_audio_source != null) ? m_audio_source.time : -0.1f; }
	}

	// 総再生時間
	public float _sound_length
	{
		get{ return (m_audio_clip != null) ? m_audio_clip.length : -0.1f; }
	}

	// ボリューム
	public float _volume
	{
		get { return (m_audio_source != null) ? m_audio_source.volume : -0.1f; }
		set { m_audio_source.volume = value; }
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

/// <summary>
/// サウンドコントローラ(シングルトン　)
/// ■コンストラクタ
/// GameObject "SoundController"に紐づける為、メインのAwake()で AddComponet すること。
/// ex).
/// 
/// GameObject m_sound_go = null;
/// void Awake()
/// {
/// 	m_sound_go = new GameObject ("SoundController");
/// 	m_sound_go.AddComponent<SoundController>();
/// }
/// 
/// ■メソッド
/// コンストラクタの呼び出し後であれば、シーンにまたがり、以下の書式で
/// アクセス可能
/// 
/// SoundController.Instance.M-e-t-h-o-d(p_-)
/// 
/// </summary>
public class SoundController : SingletonMonoBehaviour<SoundController>
{
	public const int SE_CHANEL_MAX = 3;

	private static SoundController s_Instance;

	private List<SoundInformation> m_info_list = new List<SoundInformation>();
	private List<GameObject> m_go_list = new List<GameObject>();

	private SoundModel m_sound_model = null;

	AudioSource m_sound_source_bgm;
	AudioSource[] m_sound_source_se = new AudioSource[SE_CHANEL_MAX];

	public void Awake()
	{
		if (this != Instance) {
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);

		m_sound_model = GameObject.Find(this.name).AddComponent<SoundModel> ();
		if (m_sound_model == null) {
			Debug.LogError("ERROR : new m_sound_model");
		}

	}

	/// <summary>
	/// サウンドファイル名一覧から、再生可能なオブジェクトを生成する。
	/// </summary>
	/// <returns><c>true</c>, if attach list was f_ed, <c>false</c> otherwise.</returns>
	/// <param name="p_request_list">p_request_list.</param>
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

	/// <summary>
	/// サウンド名でサウンド情報を検索する
	/// </summary>
	/// <param name="p_name">p_name.検索するサウンド名</param>
	public SoundInformation f_SearchInformation(string p_name)
	{
		if (m_info_list == null || m_info_list.Count < 0)
			return null;

		foreach (SoundInformation info in m_info_list )
		{
			// 検索該当あり
			if( p_name == info._file_name )
			{
				return info;
			}
		}

		return null;
	}

	/// <summary>
	/// サウンド情報を内部リストから削除する
	/// </summary>
	/// <param name="p_info">p_info.サウンド情報</param>
	public bool f_RemoveInformation( SoundInformation p_info )
	{
		if (m_info_list == null || m_info_list.Count < 0)
			return false;

		m_info_list.Remove (p_info);

		return true;
	}

	/// <summary>
	/// サウンド情報をアタッチする
	/// </summary>
	/// <returns>The attach.</returns>
	/// <param name="p_request_file_name">p_request_file_name.アタッチするサウンド名</param>
	public SoundInformation f_Attach( string p_request_file_name )
	{

		SoundInformation info = f_SearchInformation( p_request_file_name );
		
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
				info._audio_source = GameObject.Find(this.name).AddComponent<AudioSource>();
				info._audio_source.clip = info._audio_clip;
				m_info_list.Add(info);
			}
		}

		Debug.Log("file_name="+p_request_file_name);

		return info;
	}

	/// <summary>
	/// サウンド情報を内部リストから削除する
	/// </summary>
	/// <param name="p_request">p_request.</param>
	public void f_Detach( SoundInformation p_request )
	{
		SoundInformation info = f_SearchInformation( p_request._file_name );
		Destroy (info._audio_source);
		if (f_RemoveInformation (info) == false) {
			Debug.Log("ERROR : f_RemoveInformation ");
		}
	}

	private int m_bgm_loop_count = -1;

	/// <summary>
	/// 初回再生時、ファイル名からサウンド詳細を生成し再生開始
	/// </summary>
	/// <returns>SoundInfomation.サウンド情報を返す</returns>
	/// <param name="p_file_name">p_file_name.ファイル名</param>
	/// <param name="p_loop_count">p_loop_count.ループカウント</param>
	public SoundInformation f_Play( string p_file_name, int p_loop_count = -1 )
	{
		SoundInformation info = f_SearchInformation (p_file_name);
		if (info == null) {
			Debug.Log("Error:Unknown File =>"+p_file_name);
			return null;
		}

		if( p_loop_count == -1 )
			info._audio_source.loop = true;
		else
			info._loop_count = p_loop_count;

		info._audio_source.Play ();

		Debug.Log ("f_Play status="+info._audio_source.isPlaying +" file="+ p_file_name);

		return info;
	}

	/// <summary>
	/// サウンド情報で再生.
	/// </summary>
	/// <param name="p_info">p_info.サウンド情報</param>
	public void f_Play( SoundInformation p_info )
	{
		if (p_info == null) {
			Debug.Log("Error: p_info is null ");
		}

		float t = p_info._audio_source.time;

		p_info._audio_source.Play();

		Debug.Log ("f_Stop status="+p_info._audio_source.isPlaying +" file="+ p_info._file_name);
	}

	/// <summary>
	/// 停止
	/// </summary>
	/// <param name="p_info">p_info.</param>
	public void f_Stop( SoundInformation p_info )
	{
		if (p_info == null) {
			Debug.Log("Error: p_info is null ");
		}

		p_info._audio_source.Stop();

		Debug.Log ("f_Stop status="+p_info._audio_source.isPlaying +" file="+ p_info._file_name);
	}

	/// <summary>
	/// 一時停止
	/// </summary>
	/// <param name="p_info">p_info.</param>
	public void f_Pause( SoundInformation p_info )
	{
		if (p_info == null) {
			Debug.Log("Error: p_info is null ");
		}
		
		p_info._audio_source.Pause();

		Debug.Log ("f_Pause status="+p_info._audio_source.isPlaying +" file="+ p_info._file_name);
	}

	/// <summary>
	/// 一時停止解除
	/// </summary>
	/// <param name="p_info">p_info.</param>
	public void f_UnPause( SoundInformation p_info )
	{
		if (p_info == null) {
			Debug.Log("Error: p_info is null ");
		}
		p_info._audio_source.UnPause();
		
		Debug.Log ("f_Pause status="+p_info._audio_source.isPlaying +" file="+ p_info._file_name);

	}

}
