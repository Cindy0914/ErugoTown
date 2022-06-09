using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using KeyType = System.String;

[DisallowMultipleComponent]
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public override bool dontDestroy { get; set; } = false;

    // 인스펙터에서 오브젝트 풀링 대상 정보 추가
    public const int PODATA_COUNT = 5;

    [SerializeField]
    private List<PoolObjectData> _poolObjectDataList = new List<PoolObjectData>(PODATA_COUNT);

    // 복제될 오브젝트의 원본 딕셔너리
    private Dictionary<KeyType, PoolObject> _originDict;

    // 풀링 정보 딕셔너리
    private Dictionary<KeyType, PoolObjectData> _dataDict;

    // 풀 딕셔너리
    private Dictionary<KeyType, Stack<PoolObject>> _poolDict;

    protected override void Initialize()
    {
        base.Initialize();

        int length = _poolObjectDataList.Count;
        if (length == 0)
            return;

        // 1. Dict생성
        _originDict = new Dictionary<KeyType, PoolObject>(length);
        _dataDict = new Dictionary<KeyType, PoolObjectData>(length);
        _poolDict = new Dictionary<KeyType, Stack<PoolObject>>(length);

        // 2. Data로부터 새로운 Pool오브젝트 정보 생성
        foreach (var data in _poolObjectDataList)
            Register(data);
    }

    /// <summary>
    /// Pool 데이터로부터 새로운 Pool오브젝트 정보 등록
    /// </summary>
    private void Register(PoolObjectData data)
    {
        // 중복 체크
        if (_poolDict.ContainsKey(data.key))
            return;

        GameObject origin = Object.Instantiate(data.prefab);

        origin.name = data.prefab.name;
        if (!origin.TryGetComponent(out PoolObject po))
        {
            po = origin.AddComponent<PoolObject>();
            po.key = data.key;
        }

        // Root 설정
        po.Root = new GameObject { name = $"Root_{data.prefab.name}" }.transform;
        origin.SetActive(false);

        // Pool Dictionary에 풀 생성 + 풀에 미리 오브젝트들을 만들어 담아놓기
        Stack<PoolObject> pool = new Stack<PoolObject>(data.maxObjectCount);
        for (int i = 0; i < data.initialObjectCount; i++)
        {
            PoolObject clone = po.Clone();
            clone.Root = po.Root;
            pool.Push(clone);
        }

        // Dictionary 추가
        _originDict.Add(data.key, po);
        _dataDict.Add(data.key, data);
        _poolDict.Add(data.key, pool);
    }

    /// <summary>
    /// Pool에 존재하는(꺼내지않은 비활성화 상태) 오브젝트를 얻을수 있는 Getter 
    /// </summary>
    public PoolObject GetPoolObject(KeyType key)
    {
        if (_poolDict.ContainsKey(key))
        {
            if (_poolDict.TryGetValue(key, out var pool))
                return _poolDict[key].Peek();
            else
            {
                Debug.Log("Pool에 남은 오브젝트가 없습니다.");
                Debug.Log("Pool에 등록 후 반환합니다.");
                PoolObject po = _originDict[key].Clone();
                return po;
            }
        }

        Debug.Log("Pool에 존재하지 않는 오브젝트 입니다.");
        return null;
    }

    // 풀에서 꺼내오기
    public PoolObject Spawn(KeyType key)
    {
        if (!_poolDict.TryGetValue(key, out var pool))
            return null;

        PoolObject po;

        // 풀에 오브젝트가 있는 경우
        if (pool.Count > 0)
            po = pool.Pop();
        else // 없다면 복제생성
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

        // 풀에 넣을수 있는 경우
        if (pool.Count < _dataDict[key].maxObjectCount)
        {
            pool.Push(po);
            po.transform.SetParent(po.Root);
            po.Deactivate();
        }
        else // 풀의 한도가 가득 찬 경우
            Destroy(po.gameObject);
    }

    void Clear()
    {
        GameManager.Instance.SceneClearAction -= Clear;
        GameManager.Instance.SceneClearAction += Clear;


    }
}
