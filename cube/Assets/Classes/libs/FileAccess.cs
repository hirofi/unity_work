using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

// Tip : セーブ先について
// PC ローカルなら C:\Users\inoue\AppData\LocalLow\DefaultCompany\プロジェクト名\
// 
public class FileAccess : MonoBehaviour
{

	enum enmFileAccessStatus
	{
		LOAD_READY = 0,
		LOAD_START,
		LOAD_PROGRESS,
		LOAD_COMPLEATE,
		LOAD_SUCCESSFUL,
		LOAD_ERROR,
		DOWN_LOAD_ERROR,
		ENTRY_MAX
	}

	enum enmFileAccessMode
	{
		BYNARY = 0,
		TEXT,
		ENTRY_MAX
	}

	enmFileAccessStatus m_file_access_status = enmFileAccessStatus.LOAD_READY;
	
	public delegate void f_ProgressDelegate( float aProgress );
	public f_ProgressDelegate f_ReadProgress = null;

	string m_string_data = null;
	public string _string_data {
		get { return m_string_data; 	}
		set { m_string_data = value;	}
	}

	byte[] m_byte_data = null;
	public byte[] _byte_data {
		get { return m_byte_data; 	}
		set { m_byte_data = value;	}
	}

	AudioClip m_audio_data = null;
	public AudioClip _audio_clip {
		get{ return m_audio_data; }
	}

	Texture2D m_texture_data = null;
	public Texture _texture{
		get{ return m_texture_data; }
	}

	UnityEngine.Object m_unity_object_data = null;
	public UnityEngine.Object _unity_object{
		get{ return m_unity_object_data; }
	}

	// オーディオデータ
	public AudioClip _audio_data {
		get { return m_unity_object_data as AudioClip;	}
	}
	
	// ゲームオブジェクト
	public GameObject _game_object_data {
		get { return m_unity_object_data as GameObject;	}
	}
	// テキスト
	public TextAsset _text_data{
		get { return m_unity_object_data as TextAsset; }
	}

	// aURL : http://domain:port/path/filename.ext
	public void f_Save( WWW p_w3 , string p_save_name , bool p_over_write )
	{

		string path = Application.persistentDataPath + "/" + p_save_name;

		if ( File.Exists( path ) )
		{
			Debug.Log ("Find local file." + Application.persistentDataPath + "/" + p_save_name);
			File.Delete( path );
		} else {
			Debug.Log ("Can't find local file, Downloading..");

			while (!p_w3.isDone) {
			}

			File.WriteAllBytes(path, p_w3.bytes);
		}
	}

	public void f_LoadAsset( string p_file_name )
	{
		StartCoroutine ( f_WwwLoad(p_file_name) );
	}

	// aFilePath : 
	IEnumerator f_WwwLoad( string p_save_file_name )
	{
		string url = "file://" + Application.persistentDataPath + p_save_file_name;

		WWW www = new WWW(url);

		while (!www.isDone)
		{
			if( f_ReadProgress != null )
				f_ReadProgress(www.progress);

//			yield return null;
		}
		
		if (!string.IsNullOrEmpty(www.error)) {
			m_file_access_status = enmFileAccessStatus.LOAD_ERROR;
		} else {
			m_file_access_status = enmFileAccessStatus.LOAD_SUCCESSFUL;
			m_string_data = www.text;
			m_byte_data = www.bytes;
			m_texture_data = www.texture;
			m_audio_data = www.audioClip;
			m_unity_object_data = www.assetBundle.mainAsset;
		}

		yield return www;
	}

	// ファイル有無
	public bool f_Exists(string p_file_name)
	{
		return System.IO.File.Exists( Application.persistentDataPath + p_file_name );
	}

}
