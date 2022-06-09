using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using KeyType = System.String;

[DisallowMultipleComponent]
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public override bool dontDestroy { get; set; } = false;

    // �ν����Ϳ��� ������Ʈ Ǯ�� ��� ���� �߰�
    public const int PODATA_COUNT = 5;

    [SerializeField]
    private List<PoolObjectData> _poolObjectDataList = new List<PoolObjectData>(PODATA_COUNT);

    // ������ ������Ʈ�� ���� ��ųʸ�
    private Dictionary<KeyType, PoolObject> _originDict;

    // Ǯ�� ���� ��ųʸ�
    private Dictionary<KeyType, PoolObjectData> _dataDict;

    // Ǯ ��ųʸ�
    private Dictionary<KeyType, Stack<PoolObject>> _poolDict;

    protected override void Initialize()
    {
        base.Initialize();

        int length = _poolObjectDataList.Count;
        if (length == 0)
            return;

        // 1. Dict����
        _originDict = new Dictionary<KeyType, PoolObject>(length);
        _dataDict = new Dictionary<KeyType, PoolObjectData>(length);
        _poolDict = new Dictionary<KeyType, Stack<PoolObject>>(length);

        // 2. Data�κ��� ���ο� Pool������Ʈ ���� ����
        foreach (var data in _poolObjectDataList)
            Register(data);
    }

    /// <summary>
    /// Pool �����ͷκ��� ���ο� Pool������Ʈ ���� ���
    /// </summary>
    private void Register(PoolObjectData data)
    {
        // �ߺ� üũ
        if (_poolDict.ContainsKey(data.key))
            return;

        GameObject origin = Object.Instantiate(data.prefab);

        origin.name = data.prefab.name;
        if (!origin.TryGetComponent(out PoolObject po))
        {
            po = origin.AddComponent<PoolObject>();
            po.key = data.key;
        }

        // Root ����
        po.Root = new GameObject { name = $"Root_{data.prefab.name}" }.transform;
        origin.SetActive(false);

        // Pool Dictionary�� Ǯ ���� + Ǯ�� �̸� ������Ʈ���� ����� ��Ƴ���
        Stack<PoolObject> pool = new Stack<PoolObject>(data.maxObjectCount);
        for (int i = 0; i < data.initialObjectCount; i++)
        {
            PoolObject clone = po.Clone();
            clone.Root = po.Root;
            pool.Push(clone);
        }

        // Dictionary �߰�
        _originDict.Add(data.key, po);
        _dataDict.Add(data.key, data);
        _poolDict.Add(data.key, pool);
    }

    /// <summary>
    /// Pool�� �����ϴ�(���������� ��Ȱ��ȭ ����) ������Ʈ�� ������ �ִ� Getter 
    /// </summary>
    public PoolObject GetPoolObject(KeyType key)
    {
        if (_poolDict.ContainsKey(key))
        {
            if (_poolDict.TryGetValue(key, out var pool))
                return _poolDict[key].Peek();
            else
            {
                Debug.Log("Pool�� ���� ������Ʈ�� �����ϴ�.");
                Debug.Log("Pool�� ��� �� ��ȯ�մϴ�.");
                PoolObject po = _originDict[key].Clone();
                return po;
            }
        }

        Debug.Log("Pool�� �������� �ʴ� ������Ʈ �Դϴ�.");
        return null;
    }

    // Ǯ���� ��������
    public PoolObject Spawn(KeyType key)
    {
        if (!_poolDict.TryGetValue(key, out var pool))
            return null;

        PoolObject po;

        // Ǯ�� ������Ʈ�� �ִ� ���
        if (pool.Count > 0)
            po = pool.Pop();
        else // ���ٸ� ��������
            po = _originDict[key].Clone();

        po.transform.SetParent(null);
        po.Activate();

        return po;
    }

    public void Despawn(PoolObject po)
    {
        if (!_poolDict.TryGetValue(po.key, out var pool))
            return;

        KeyType key = po.key;

        // Ǯ�� ������ �ִ� ���
        if (pool.Count < _dataDict[key].maxObjectCount)
        {
            pool.Push(po);
            po.transform.SetParent(po.Root);
            po.Deactivate();
        }
        else // Ǯ�� �ѵ��� ���� �� ���
            Destroy(po.gameObject);
    }

    void Clear()
    {
        GameManager.Instance.SceneClearAction -= Clear;
        GameManager.Instance.SceneClearAction += Clear;


    }
}
