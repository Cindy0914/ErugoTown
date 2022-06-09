using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonManager
{
    public void ToJson<T>(T data, string savePath)
    {
        string jsonText = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.dataPath, $"{Define.StaticDataPath}/Json/{savePath}.json");
        File.WriteAllText(path, jsonText);
    }

    public T FromJson<T>(string jsonFilePath)
    {
        string path = Path.Combine(Application.dataPath, $"{Define.StaticDataPath}/Json/{jsonFilePath}.json");
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public List<T> FromJsonList<T>(string loadDataPath)
    {
        return JsonConvert.DeserializeObject<List<T>>(loadDataPath);
    }
}
