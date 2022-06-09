using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public override bool dontDestroy { get; set; } = true;

    private float loadProgress = 0f;

    public Action SceneClearAction = null;

    protected override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// 씬을 비동기로 로드하는 기능
    /// 다른 씬 간의 전환에 사용 (ex: Title -> InGame)
    /// </summary>
    /// <param name="sceneName">로드할 씬의 이름을 갖는 열거형</param>
    /// <param name="loadCoroutien">씬 전환 시 로딩 씬에서 미리 처리할 작업</param>
    /// <param name="loadComplete">씬 전환 완료 후 실행할 기능</param>
    public void LoadScene(Define.SceneType sceneName, IEnumerator loadCoroutine = null, Action loadComplete = null)
    {
        StartCoroutine(WaitForLoad());

        // LoadScene 메서드에서만 사용가능한 로컬함수 선언
        IEnumerator WaitForLoad()
        {
            // 로딩 진행상태를 나타냄 (0~1)
            loadProgress = 0;

            // 이전 씬의 데이터를 비움
            Clear();

            yield return SceneManager.LoadSceneAsync(Define.SceneType.Loading.ToString());

            // 로딩 씬으로 전환 완료 후에 아래 로직이 들어옴

            // 내가 변경하고자하는 씬을 추가
            var asyncOper = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
            // 방금 추가한 씬을 비활성화
            asyncOper.allowSceneActivation = false;

            // 변경하고자 하는 씬에 필요한 작업이 존재한다면 실행
            if (loadCoroutine != null)
            {
                yield return StartCoroutine(loadCoroutine);
            }

            // 비동기로 로드한 씬이 활성화가 완료되지 않았다면 특정 작업을 반복
            while (!asyncOper.isDone)
            {
                if (loadProgress >= .9f)
                {
                    loadProgress = 1f;

                    // 로딩바가 마지막까지 차는 것을 확인하기 위해 1초 정도 대기
                    yield return new WaitForSeconds(1.5f);

                    // 변경하고자 하는 씬을 다시 활성화
                    asyncOper.allowSceneActivation = true;
                }
                else
                    loadProgress = asyncOper.progress;

                yield return null;
            }

            // 로딩 씬에서 다음 씬에 필요한 작업을 전부 수행했으므로 로딩씬을 비활성화 시킴
            yield return SceneManager.UnloadSceneAsync(Define.SceneType.Loading.ToString());

            // 모든 작업이 완료되었으므로 모든 작업 완료 후 실행시킬 로직이 있다면 실행
            loadComplete?.Invoke();
        }
    }

    /// <summary>
    /// 인게임 씬에서 스테이지 전환 시 사용 (실제 씬을 변경하는 것이 아닌, 로딩씬을 이용하여 씬 전환처럼 보이게 만듬)
    /// 로딩 씬을 이용하여 변경하고자 하는 스테이지에 필요한 리소스 로드나 초기화 작업등을 처리
    /// </summary>
    /// <param name="loadCoroutine">씬 전환시 필요한 작업</param>
    /// <param name="loadComplete">씬 전환을 완료한 후 실행할 작업</param>
    public void OnAdditiveLoadingScene(IEnumerator loadCoroutine = null, Action loadComplete = null)
    {
        StartCoroutine(WaitForLoad());

        IEnumerator WaitForLoad()
        {
            loadProgress = 0;

            // 이전씬의 데이터를 비움
            Clear();

            var asyncOper = SceneManager.LoadSceneAsync(Define.SceneType.Loading.ToString(), LoadSceneMode.Additive);

            #region 로딩바 진행상태 처리
            while (!asyncOper.isDone)
            {
                loadProgress = asyncOper.progress;
                yield return null;
            }


            loadProgress = 1f;
            #endregion


            #region 스테이지 전환 시 필요한 작업
            if (loadCoroutine != null)
                yield return StartCoroutine(loadCoroutine);
            #endregion

            #region 스테이지 전환 완료 후 실행할 작업
            yield return SceneManager.UnloadSceneAsync(Define.SceneType.Loading.ToString());

            loadComplete?.Invoke();
            #endregion
        }
    }

    public void Clear()
    {
        if (SceneClearAction != null)
            SceneClearAction.Invoke();
    }
}
