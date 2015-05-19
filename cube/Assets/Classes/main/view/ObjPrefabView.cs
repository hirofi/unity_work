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
			m_sound_information = SoundController.Instance.f_Play("bgm_wav_01");
			if (m_sound_information == null)
			{
				Debug.Log ("ERRRO:unknown sound");
			}

			return;
		}

		if (m_sound_information._audio_source.isPlaying)
			SoundController.Instance.f_Stop (m_sound_information);
		else
			SoundController.Instance.f_Play (m_sound_information);

	}
}
