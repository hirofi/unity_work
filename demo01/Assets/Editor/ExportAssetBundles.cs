// C# の例
// プロジェクト ウィンドウの選択されたオブジェクトからアセットバンドルを作成
// コンパイルした後は "Menu" -> "Assets" へ移動して選択肢から一つを選択して
// アセットバンドルをビルド
using UnityEngine;
using UnityEditor;

public class ExportAssetBundles
{
    [
     MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    static void ExportResource ()
    {
        // 保存ウィンドウのパネルを表示
        string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0) {
            // アクティブなセレクションに対してリソースファイルをビルド
            
            Object[] selection = 
                Selection.GetFiltered (typeof(Object), SelectionMode.DeepAssets);
            
            // via https://gist.github.com/yaeda/5410868
            // require iOS Pro, Android Pro Lisence
            // for Android
            BuildPipeline.BuildAssetBundle(Selection.activeObject,
                                           selection, path + ".android.unity3d",
                                           BuildAssetBundleOptions.CollectDependencies |
                                           BuildAssetBundleOptions.CompleteAssets,
                                           BuildTarget.Android);
            
            // for iPhone
            BuildPipeline.BuildAssetBundle(Selection.activeObject,
                                           selection, path + ".iphone.unity3d",
                                           BuildAssetBundleOptions.CollectDependencies |
                                           BuildAssetBundleOptions.CompleteAssets,
                                           BuildTarget.iOS);
            
            // for WebPlayer
            BuildPipeline.BuildAssetBundle(Selection.activeObject,
                                           selection, path + ".unity3d",
                                           BuildAssetBundleOptions.CollectDependencies |
                                           BuildAssetBundleOptions.CompleteAssets,
                                           BuildTarget.WebPlayer);

            Selection.objects = selection;
        }
    }

    [
     MenuItem("Assets/Build AssetBundle From Selection - No dependency tracking")]
    static void ExportResourceNoTrack ()
    {
        // 保存ウィンドウのパネルを表示
        string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0) {
            // アクティブなセレクションに対してリソースファイルをビルド
            
            BuildPipeline.BuildAssetBundle (
                Selection.activeObject, 
                Selection.objects, path);
        }
    }
}