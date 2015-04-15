﻿using UnityEngine;
using System;
using System.Collections;

public class ContentsDownloadApiModel : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string m_url = "pshpz01.isl.gacha.fujitv.co.jp/unity/";

	public void GetAssetBandle( string aDownloadFileName )
	{

		// Clear Cache
		Caching.CleanCache();

#if   UNITY_ANDROID && !UNITY_EDITOR
		string url = this.m_url + aDownloadFileName + ".unity3d.android.unity3d";
#elif UNITY_IPHONE  && !UNITY_EDITOR
		string url = this.m_url + aDownloadFileName + ".unity3d.iphone.unity3d";
#else
		string url = "http://" + this.m_url + aDownloadFileName + ".unity3d.unity3d?dl=1";
#endif
		
		StartCoroutine ( DownloadAndCache( aDownloadFileName, url, 0 ) );
	}

	private IEnumerator DownloadAndCache (string assetName, string url, int version = 1)
	{

		// キャッシュシステムの準備が完了するのを待ちます
		while (!Caching.ready)
			yield return null;
		
		// 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードする
		// またはダウンロードしてキャッシュに格納します。
		using (WWW www = WWW.LoadFromCacheOrDownload(url, version) )
		{
			yield return www;
			if (www.error != null) {
				throw new Exception ("WWWダウンロードにエラーがありました:" + www.error);
			}
			
			AssetBundle bundle = www.assetBundle;
			if (assetName == "")
				Instantiate ( bundle.mainAsset );
			else
				Instantiate ( bundle.LoadAsset (assetName) );
			// メモリ節約のため圧縮されたアセットバンドルのコンテンツをアンロード
			bundle.Unload (false);
			
		} // memory is freed from the web stream (www.Dispose() gets called implicitly)

		yield return null;
		Debug.Log(Caching.IsVersionCached(url, 1));
		Debug.Log("DownloadAndCache end");
	}

}
