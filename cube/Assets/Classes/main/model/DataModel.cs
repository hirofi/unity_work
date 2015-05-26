using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using System.Runtime.Serialization.Formatters.Binary;

public class DataModel : FileAccess
{

	private string m_local_save_file_name;

	public DataModel()
	{
	}
	
	public static bool f_SaveData<T>( string prefKey, T serializableObject )
	{
		MemoryStream memoryStream = new MemoryStream();

#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif

		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize( memoryStream, serializableObject );
		
		string tmp = System.Convert.ToBase64String( memoryStream.ToArray() );
		try {
			PlayerPrefs.SetString ( prefKey, tmp );
		} catch( PlayerPrefsException ) {
			return false;
		}
		return true;
	}
	
	public static T f_LoadData<T>( string prefKey )
	{
		if (!PlayerPrefs.HasKey(prefKey))
			return default(T);

#if UNITY_IPHONE || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
		BinaryFormatter bf = new BinaryFormatter();
		string serializedData = PlayerPrefs.GetString( prefKey );
		
		MemoryStream dataStream = new MemoryStream( System.Convert.FromBase64String(serializedData) );
		T deserializedObject = (T)bf.Deserialize( dataStream );
		
		return deserializedObject;
	}
	
	public void f_Save(string p_save_data_file_name, SaveData p_data)
	{
		// 保存用クラスにデータを格納.
		DataModel.f_SaveData( p_save_data_file_name, p_data);
		PlayerPrefs.Save();
	}
	
	public SaveData f_Load( string p_save_data_file_name )
	{
		SaveData data_tmp = f_LoadData<SaveData>( p_save_data_file_name );
		if( data_tmp != null)
		{
			return data_tmp;
		}
		else
		{
			return null;
		}
	}
}

