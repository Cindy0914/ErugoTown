using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public static class StaticDataImporter
{
    public static void Import(string[] importedAssets, string[] deletedAssets,
        string[] movedAssets, string[] movedFromAssetPaths)
    {
        ImportNewOrModified(importedAssets);
        Delete(deletedAssets);
        Move(movedAssets, movedFromAssetPaths);
    }

    private static void Delete(string[] deletedAssets)
    {
        ExcelToJson(deletedAssets, true);
    }

    private static void Move(string[] movedAssets, string[] movedFromAssetPaths)
    {
        // 이전 경로 에셋 삭제 기능 실행
        Delete(movedFromAssetPaths);
        // 새로운 경로 에셋 수정
        ImportNewOrModified(movedAssets);
    }

    private static void ImportNewOrModified(string[] importedAssets)
    {
        ExcelToJson(importedAssets, false);
    }

    private static void ExcelToJson(string[] assets, bool isDeleted)
    {
        List<string> staticDataAssets = new List<string>();

        foreach (var asset in assets)
        {
            if (IsStaticData(asset, isDeleted))
                staticDataAssets.Add(asset);
        }

        foreach (var staticDataAsset in staticDataAssets)
        {
            try
            {
                var fileName = staticDataAsset.Substring(staticDataAsset.LastIndexOf('/') + 1);
                fileName = fileName.Remove(fileName.LastIndexOf('.'));

                var rootPath = Application.dataPath;
                rootPath = rootPath.Remove(rootPath.LastIndexOf('/'));

                var fileFullPath = $"{rootPath}/{staticDataAsset}";

                var excelToJsonConvert = new ExcelToJsonConvert(fileFullPath,
                    $"{rootPath}/Assets/StaticData/Json");

                if (excelToJsonConvert.SaveJsonFiles() > 0)
                {
                    AssetDatabase.ImportAsset($"Assets/StaticData/Json/{fileName}.json");
                    Debug.Log($"##### StaticData {fileName} reimported");
                }
            }

            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogErrorFormat("Couldn't convert assets = {0}", staticDataAsset);
                EditorUtility.DisplayDialog("Error Convert",
                    string.Format("Couldn't convert assets = {0}", staticDataAsset),
                    "OK");
            }
        }
    }

    private static bool IsStaticData(string path, bool isDeleted)
    {
        if (path.EndsWith(".xlsx") == false)
            return false;

        var absolutePath = Application.dataPath + path.Remove(0, "Assets".Length);

        return ((isDeleted || File.Exists(absolutePath)) && path.StartsWith("Assets/StaticData/Excel"));
    }
}