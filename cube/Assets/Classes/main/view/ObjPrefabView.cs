using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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
			else if( this.name == "save")
			{
				SaveData save_data = DataController.Instance.f_LoadData();
				if( save_data == null)
				{
					save_data = new SaveData();
					save_data._game_datas = new List<SaveData.GameData>();
					save_data._game_datas.Add( new SaveData.GameData() );
					save_data._game_datas[0]._score = 0;
					save_data._game_datas[0]._time = 0;
				}
				else
				{
					Transform bar = transform.Find("bar");
					TextMesh txt = transform.Find("txtProgress").GetComponent<TextMesh>();

					save_data._game_datas[0]._score += 1;
					save_data._game_datas[0]._time += 1;

					bar.localScale = new Vector3( (float)(save_data._game_datas[0]._score/100), 0.5f, 0.1f );
					txt.text = "score:"+save_data._game_datas[0]._score;
				}

				DataController.Instance.f_SaveData(save_data);
			}


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
