using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class StaticDataModule
{
    public List<SDItem> sdItem = new List<SDItem>();

    public void Init()
    {
        var loader = new StaticDataLoader();

        loader.Load<SDItem>(out sdItem);
    }

    private class StaticDataLoader
    {
        private string path;

        public StaticDataLoader()
        {
            path = $"{Application.dataPath}/StaticData/Json";
        }

        public void Load<T>(out List<T> data) where T : StaticData
        {
            // �����̸��� Ÿ���̸����� SD�� �����ϸ� �����ϴٴ� ��Ģ�� ����..
            var fileName = typeof(T).Name.Remove(0, "SD".Length);

            var json = File.ReadAllText($"{path}/{fileName}.json");

            data = DataManager.Instance.Json.FromJsonList<T>(json);
        }
    }
}

