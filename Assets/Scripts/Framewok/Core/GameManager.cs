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
    /// ���� �񵿱�� �ε��ϴ� ���
    /// �ٸ� �� ���� ��ȯ�� ��� (ex: Title -> InGame)
    /// </summary>
    /// <param name="sceneName">�ε��� ���� �̸��� ���� ������</param>
    /// <param name="loadCoroutien">�� ��ȯ �� �ε� ������ �̸� ó���� �۾�</param>
    /// <param name="loadComplete">�� ��ȯ �Ϸ� �� ������ ���</param>
    public void LoadScene(Define.SceneType sceneName, IEnumerator loadCoroutine = null, Action loadComplete = null)
    {
        StartCoroutine(WaitForLoad());

        // LoadScene �޼��忡���� ��밡���� �����Լ� ����
        IEnumerator WaitForLoad()
        {
            // �ε� ������¸� ��Ÿ�� (0~1)
            loadProgress = 0;

            // ���� ���� �����͸� ���
            Clear();

            yield return SceneManager.LoadSceneAsync(Define.SceneType.Loading.ToString());

            // �ε� ������ ��ȯ �Ϸ� �Ŀ� �Ʒ� ������ ����

            // ���� �����ϰ����ϴ� ���� �߰�
            var asyncOper = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
            // ��� �߰��� ���� ��Ȱ��ȭ
            asyncOper.allowSceneActivation = false;

            // �����ϰ��� �ϴ� ���� �ʿ��� �۾��� �����Ѵٸ� ����
            if (loadCoroutine != null)
            {
                yield return StartCoroutine(loadCoroutine);
            }

            // �񵿱�� �ε��� ���� Ȱ��ȭ�� �Ϸ���� �ʾҴٸ� Ư�� �۾��� �ݺ�
            while (!asyncOper.isDone)
            {
                if (loadProgress >= .9f)
                {
                    loadProgress = 1f;

                    // �ε��ٰ� ���������� ���� ���� Ȯ���ϱ� ���� 1�� ���� ���
                    yield return new WaitForSeconds(1.5f);

                    // �����ϰ��� �ϴ� ���� �ٽ� Ȱ��ȭ
                    asyncOper.allowSceneActivation = true;
                }
                else
                    loadProgress = asyncOper.progress;

                yield return null;
            }

            // �ε� ������ ���� ���� �ʿ��� �۾��� ���� ���������Ƿ� �ε����� ��Ȱ��ȭ ��Ŵ
            yield return SceneManager.UnloadSceneAsync(Define.SceneType.Loading.ToString());

            // ��� �۾��� �Ϸ�Ǿ����Ƿ� ��� �۾� �Ϸ� �� �����ų ������ �ִٸ� ����
            loadComplete?.Invoke();
        }
    }

    /// <summary>
    /// �ΰ��� ������ �������� ��ȯ �� ��� (���� ���� �����ϴ� ���� �ƴ�, �ε����� �̿��Ͽ� �� ��ȯó�� ���̰� ����)
    /// �ε� ���� �̿��Ͽ� �����ϰ��� �ϴ� ���������� �ʿ��� ���ҽ� �ε峪 �ʱ�ȭ �۾����� ó��
    /// </summary>
    /// <param name="loadCoroutine">�� ��ȯ�� �ʿ��� �۾�</param>
    /// <param name="loadComplete">�� ��ȯ�� �Ϸ��� �� ������ �۾�</param>
    public void OnAdditiveLoadingScene(IEnumerator loadCoroutine = null, Action loadComplete = null)
    {
        StartCoroutine(WaitForLoad());

        IEnumerator WaitForLoad()
        {
            loadProgress = 0;

            // �������� �����͸� ���
            Clear();

            var asyncOper = SceneManager.LoadSceneAsync(Define.SceneType.Loading.ToString(), LoadSceneMode.Additive);

            #region �ε��� ������� ó��
            while (!asyncOper.isDone)
            {
                loadProgress = asyncOper.progress;
                yield return null;
            }


            loadProgress = 1f;
            #endregion


            #region �������� ��ȯ �� �ʿ��� �۾�
            if (loadCoroutine != null)
                yield return StartCoroutine(loadCoroutine);
            #endregion

            #region �������� ��ȯ �Ϸ� �� ������ �۾�
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
