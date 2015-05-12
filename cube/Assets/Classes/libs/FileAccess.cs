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
	public f_ProgressDelegate f_DownloadProgress = null;

	string m_text_data = null;
	byte[] m_byte_data = null;

	public FileAccess ()
	{
	}

	string LoadText ( string aFileName )
	{
		string path = Application.persistentDataPath + aFileName;
		string text = "";//File.ReadAllText(path);
		return text;
	}

	public void f_DownLoadAssetAndSave()
	{
	}

	public void f_GetDownLoadAssetAndSaveStatus()
	{

	}

	public void f_GetDownloadProgress()
	{

	}

	static void f_DownLoadProgress( float aProgress )
	{

	}

	// aURL : http://domain:port/path/filename.ext
	private IEnumerator DownLoadAndSave ( string aURL , string aSaveFileName ) {

		WWW www = new WWW( aURL );
		
		while ( !www.isDone ) {
			if( f_DownloadProgress != null )
				f_DownLoadProgress(www.progress);

			yield return null;
		}
		
		if (!string.IsNullOrEmpty(www.error)) { // ダウンロードでエラーが発生した
			m_file_access_status = enmFileAccessStatus.LOAD_ERROR;
		} else { // ダウンロードが正常に完了した

			string save_file_name = null;
			if( aSaveFileName == null )
				save_file_name = Path.GetFileName(www.url);
			else
				save_file_name = aSaveFileName;

//			File.WriteAllBytes(Application.persistentDataPath + "/" + save_file_name, www.bytes);
			m_text_data = www.text;
			m_byte_data = www.bytes;
		}
	}

	// aFilePath : 
	private IEnumerator Load ( string aSaveFileName ) {
		WWW www = new WWW("file://" + Application.persistentDataPath + aSaveFileName);
		
		while (!www.isDone)
		{
			if( f_DownloadProgress != null )
				f_DownLoadProgress(www.progress);

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
