using UnityEngine;
using System.Collections;

public class DataController : SingletonMonoBehaviour<DataController>
{

	const string SAVE_DATA_FILE_NAME = "mori.dat";
	private SaveData m_save_data;
	private DataModel m_data_model;

	public void Awake()
	{
		if (this != Instance)
		{
			Destroy(this);
			return;
		}
		
		DontDestroyOnLoad(this.gameObject);
		
		m_data_model = GameObject.Find(this.name).AddComponent<DataModel> ();
		if ( m_data_model == null )
		{
			Debug.LogError("ERROR : new m_save_data");
		}
	}


	DataController()
	{
		;
	}

	public void f_SaveData( SaveData p_save_data )
	{
		m_data_model.f_Save( SAVE_DATA_FILE_NAME, p_save_data);
	}

	public SaveData f_LoadData()
	{
		return m_data_model.f_Load( SAVE_DATA_FILE_NAME );
	}

}
