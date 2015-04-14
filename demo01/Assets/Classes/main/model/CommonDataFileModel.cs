using UnityEngine;
using System.Collections;

public class CommonDataFileModel {

	BinaryAccess m_binary_access;

	CommonDataFileModel()
	{
		m_binary_access = new BinaryAccess ();
	}

	void readUserData()
	{

	}

	void writeUserData()
	{

	}


	void readAssetBandle()
	{
		string path = m_binary_access.getAssetPath();


	}


}
