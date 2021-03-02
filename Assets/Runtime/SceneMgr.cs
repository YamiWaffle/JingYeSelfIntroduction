using JingYe.Common;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JingYe.SelfIntro
{
    public class SceneMgr : Singleton<SceneMgr>
    {
        public bool IsDuringChangeScene { get; private set; }

        public Task ChangeScene(string sceneName)
        {
            if (m_ChangeSceneCoroutine != null)
            {
                Debug.LogWarning($"SceneMgr:: ChangeScene(): ChangeScene Twice, skipped {sceneName}.");
                return m_ChangeSceneTaskCompletionSource.Task;
            }

            m_ChangeSceneTaskCompletionSource = new TaskCompletionSource<bool>();
            m_ChangeSceneCoroutine = StartCoroutine(_ChangeScene(sceneName, () => {
                m_ChangeSceneTaskCompletionSource.SetResult(true);
                m_ChangeSceneTaskCompletionSource = null;
            }));
            return m_ChangeSceneTaskCompletionSource.Task;
        }

        public Task UnloadScene(string sceneName)
        {
            var tcs = new TaskCompletionSource<bool>();

            StartCoroutine(UnloadSceneCoroutine(sceneName, () =>
            {
                tcs.SetResult(true);
            }));

            return tcs.Task;
        }

        private IEnumerator _ChangeScene(string sceneName, Action callback = null)
        {
            if (IsDuringChangeScene == true)
            {
                Debug.LogWarning($"SceneMgr:: _ChangeScene(): ChangeScene Twice, skipped {sceneName}.");
                yield break;
            }

            // Raise flag
            IsDuringChangeScene = true;

            // Invoke 'BeforeChangeScene' event
            WorldEventSystem.OnBeforeChangeScene?.Invoke(sceneName);
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!asyncOperation.isDone)
            {
                // Invoke 'UpdateChangeSceneProgress' event
                WorldEventSystem.OnUpdateChangeSceneProgress?.Invoke(sceneName, asyncOperation.progress);
                if (asyncOperation.progress >= 0.9f)
                {
                    WorldEventSystem.OnUpdateChangeSceneProgress?.Invoke(sceneName, 1f);
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            // Falling flag
            IsDuringChangeScene = false;

            // Clear task
            m_ChangeSceneCoroutine = null;

            callback?.Invoke();

            // Invoke 'AfterChangeScene' event
            WorldEventSystem.OnAfterChangeScene?.Invoke(sceneName);
        }

        private IEnumerator UnloadSceneCoroutine(string sceneName, Action callback)
        {
            WorldEventSystem.OnBeforeChangeScene?.Invoke(sceneName);

            yield return SceneManager.UnloadSceneAsync(sceneName);
            yield return new WaitForSeconds(3f);
            callback();

            WorldEventSystem.OnAfterChangeScene?.Invoke("");
        }

        private void OnDisable()
        {
            if (m_ChangeSceneCoroutine != null)
            {
                StopCoroutine(m_ChangeSceneCoroutine);
                m_ChangeSceneCoroutine = null;
                m_ChangeSceneTaskCompletionSource = null;
            }
        }

        private Coroutine m_ChangeSceneCoroutine;
        private TaskCompletionSource<bool> m_ChangeSceneTaskCompletionSource;
    } // END class
} // END namespace
