using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

public class BinaryAccess {


	/* データのシリアライズ
	byte[] ReadSerializedData( string path )
	{
		FileStream ifs = new FileStream("datafile.bin",FileMode.Open,FileAccess.Read);
		BinaryFormatter bf = new BinaryFormatter();
		data = (DataClass)bf.Deserialize(ifs);
		ifs.Close();
	}
	*/

	public string getAssetPath()
	{
		string path = "";

		// Androidでの実行の場合.
		if (Application.platform == RuntimePlatform.Android) {
			path = "jar:file://" + Application.dataPath + "!/assets/";
		}
		// iOSでの実行の場合.
		else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			path = Application.dataPath + "/Raw";
		}
		// Windowsでの実行の場合.
		else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			path = Application.dataPath + "/StreamingAssets";
		}
		else
			path = Application.dataPath + "/Resources";

		return path;
	}

	
	byte[] ReadBinaryFile(string path){
		FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		BinaryReader bin = new BinaryReader(fileStream);
		byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);
		
		bin.Close();
		
		return values;
	}
	
	public Texture ReadTexture(string path, int width, int height){
		byte[] readBinary = ReadBinaryFile(path);
		
		Texture2D texture = new Texture2D(width, height);
		texture.LoadImage(readBinary);
		
		return texture;
	}
}
