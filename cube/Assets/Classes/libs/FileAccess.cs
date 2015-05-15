using UnityEngine;
using System.Collections;
using System.IO;

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

	string m_text_data = null;
	public string _text_data {
		get { return m_text_data; 	}
		set { m_text_data = value;	}
	}

	byte[] m_byte_data = null;
	public byte[] _byte_data {
		get { return m_byte_data; 	}
		set { m_byte_data = value;	}
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

	public void f_Load( string p_file_name )
	{
		StartCoroutine (Load (p_file_name));
	}

	// aFilePath : 
	private IEnumerator Load ( string aSaveFileName ) {
		WWW www = new WWW("file://" + Application.persistentDataPath + aSaveFileName);
		
		while (!www.isDone)
		{
			if( f_ReadProgress != null )
				f_ReadProgress(www.progress);

			yield return null;
		}
		
		if (!string.IsNullOrEmpty(www.error)) {
			m_file_access_status = enmFileAccessStatus.LOAD_ERROR;
		} else {
			m_file_access_status = enmFileAccessStatus.LOAD_SUCCESSFUL;
			m_text_data = www.text;
			m_byte_data = www.bytes;
		}
	}

}
