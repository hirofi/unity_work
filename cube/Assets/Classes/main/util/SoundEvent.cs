using UnityEngine;
using System.Collections;

public class SoundEvent : GameEventStatic  {

	public enum enmSoundStatus
	{
		LOADING_SOUND_FILE,
		PLAY,
		STOP,
		PAUSE,
		ENTRY_MAX
	}

	private string m_sound_file_name;
	private enmSoundStatus m_sound_status;

}
