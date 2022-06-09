using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResourceManager : Singleton<ResourceManager>
{
    public override bool dontDestroy { get; set; } = true;

    Dictionary<string, UnityEngine.Object> _currentSceneResourceDict;
    Dictionary<string, UnityEngine.Object> _allSceneResourceDict;

    protected override void Initialize()
    {
        base.Initialize();

        _currentSceneResourceDict = new Dictionary<string, UnityEngine.Object>();
        _allSceneResourceDict = new Dictionary<string, UnityEngine.Object>();
    }

    public T Load<T>(string path,bool canRemain = false) where T : UnityEngine.Object
    {
        string name = path.Substring(path.LastIndexOf('/'));

        // �̹� Load�ߴ� ���ҽ���� ����
        if (canRemain)
            if (_allSceneResourceDict.ContainsKey(name))
                return _allSceneResourceDict[name] as T;
        else
            if (_currentSceneResourceDict.ContainsKey(name))
                return _currentSceneResourceDict[name] as T;

        // ������ �ɷ��������� ������(��, ó�� �ε��ϴ� �������� ���)
        var data = Resources.Load<T>(path);

        if(data == null)
        {
            Debug.Log("�߸��� ����̰ų�, ������ ������ �������� �ʽ��ϴ�.");
            Debug.Log($"Path : {path}");
            return null;
        }

        if (canRemain)
            _allSceneResourceDict.Add(data.name, data);
        else
            _currentSceneResourceDict.Add(data.name, data);

        return data;
    }

    void Clear()
    {
        GameManager.Instance.SceneClearAction -= Clear;
        GameManager.Instance.SceneClearAction += Clear;

        _currentSceneResourceDict.Clear();
    }
}
