using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundModel : FileAccess {

	const string PREFAB_ROOT = "Prefab/";
	const string SOUND_ROOT = "Sound/";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private List<AudioClip> m_sound = new List<AudioClip>();
	private List<GameObject> m_game_objcet = new List<GameObject>();
	public AudioClip f_AttachSoundFile( string p_request_file_name )
	{

		AudioClip audio_clip = new AudioClip ();

		// ダウンロードファイルから検索
		string file_path = PREFAB_ROOT + p_request_file_name;
		if( f_Exists (file_path) ){
			audio_clip = new AudioClip();
			audio_clip = Resources.Load<AudioClip>(file_path);
			Debug.Log ("sound from included prefabdata file");
			return audio_clip;
		}

		// 同梱の Asset/Resource/sound から検索
		string sound_name = SOUND_ROOT + p_request_file_name;
		audio_clip = (AudioClip)Resources.Load( sound_name );

		Debug.Log ("sound from data file");

		return audio_clip;
	}
}
