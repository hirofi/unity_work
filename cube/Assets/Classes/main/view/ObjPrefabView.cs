using UnityEngine;
using System.Collections;
using System;

public class ObjPrefabView : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private SoundInformation m_sound_information;
	void OnMouseUp()
	{
		Debug.Log ("OnMouseUp");
	
		if (m_sound_information == null) {

			if( this.name == "pf01" )
				m_sound_information = SoundController.Instance.f_Play("bgm_wav_01");
			else if( this.name == "pf02")
				m_sound_information = SoundController.Instance.f_Play("bgm_ogg_01");
			else if( this.name == "pf03")
				m_sound_information = SoundController.Instance.f_Play("bgm_wav_02");
			else if( this.name == "pf04")
				m_sound_information = SoundController.Instance.f_Play("bgm_ogg_02");

			if (m_sound_information == null)
			{
				Debug.Log ("ERRRO:unknown sound");
			}

			return;
		}

		if (m_sound_information._audio_source.isPlaying)
			SoundController.Instance.f_Pause (m_sound_information);
		else if (m_sound_information._audio_source.time != 0)
			SoundController.Instance.f_UnPause (m_sound_information);
		else
			SoundController.Instance.f_Play (m_sound_information);

	}
}
