using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishShadow : MonoBehaviour
{
    VrPlayerFishing playerFishing;
    PoolObject poolObj;
    [SerializeField]
    SpriteRenderer fishSpriteRenderer;

    void Start()
    {
        playerFishing = ContentsManager.Instance.player.GetComponent<VrPlayerFishing>();
        TryGetComponent(out poolObj);
    }

    public void RotateFish()
    {
        fishSpriteRenderer.color = new Color(0,0,0, 0.2f);
        // var loopRandNumber = UnityEngine.Random.Range(2, 6);

        Vector3[] randVecArr = new Vector3[UnityEngine.Random.Range(0,11)];
        for(int i = 0;i < randVecArr.Length; i++)
        {
            randVecArr[i] = new Vector3(UnityEngine.Random.insideUnitCircle.x,0, UnityEngine.Random.insideUnitCircle.y);
        }

        fishSpriteRenderer.gameObject.transform.DOLocalPath(randVecArr,5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            // 효과생성
            fishSpriteRenderer.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
             {
                 playerFishing.canCatch = true;

                 ObjectPoolManager.Instance.Despawn(poolObj);
             });
        });
    }
}
