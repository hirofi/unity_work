using UnityEngine;
using System.Collections;

public class DataEvet : GameEventDynamic  {

	public enum enmDataStatus
	{
		LOADING_DATA,
		ENTRY_MAX
	}
	
	private string m_data_file_name;
	private enmDataStatus m_data_status;

}
