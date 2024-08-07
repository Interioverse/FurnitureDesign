using UnityEditor;
using System.IO;
using UnityEngine;
using System;

public class CreateAssetBundles
{
    static string assetBundleDirectory = Application.persistentDataPath + "/AssetBundles";

    [MenuItem("Assets/Create Asset Bundles")]

    //[MenuItem("Assets/Build Asset bundle")]
    static void BuildAllAssetBundles()
    {
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
            CreateAssetBundle();
        }
        else
        {
            CreateAssetBundle();
        }
    }

    private static void CreateAssetBundle()
    {
        BuildPipeline.BuildAssetBundles(Application.persistentDataPath + "/AssetBundles", // path where your assetbundle will be saved
                                                BuildAssetBundleOptions.None,
                                                  EditorUserBuildSettings.activeBuildTarget);
    }
}