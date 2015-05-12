using UnityEngine;
using System.Collections;


public class MapData {

	private Vector3 m_position;
	public Vector3 position {
		get { return m_position;	}
		set { m_position = value;	}
	}

	private int m_type;
	public int type {
		get { return m_type;	}
		set { m_type = value;	}
	}

}

public class MapModel {

	public void f_FileAccessProgress( float aProgress )
	{
		Debug.Log ("progress=" + aProgress);
	}

	public void f_LoadMapData( Vector3 aRangeFrom, Vector3 aRangeTo )
	{
		FileAccess file_access = new FileAccess();
	 	file_access.f_DownLoadAssetAndSave();
		file_access.f_DownloadProgress = f_FileAccessProgress;

	}

	public void f_SaveMapData()
	{
		int a;
		a = 0;
	}


}
