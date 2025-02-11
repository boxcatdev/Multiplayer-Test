using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PatchworkGames
{
    public static class Loader
    {
        private class LoadingMonobehavior : MonoBehaviour { }

        private static Action onLoaderCallback;
        private static AsyncOperation loadingAsyncOperation;
        public static void Load(string sceneName)
        {
            //subscribe to onLoaderCallback so it gets called first frame in loading scene
            onLoaderCallback = () =>
            {
                GameObject loadingGameObject = new GameObject("Loading Game Object");
                loadingGameObject.AddComponent<LoadingMonobehavior>().StartCoroutine(LoadSceneAsync(sceneName));
            };

            SceneManager.LoadScene("LoadingScene");
        }

        private static IEnumerator LoadSceneAsync(string sceneName)
        {
            yield return null;

            loadingAsyncOperation = SceneManager.LoadSceneAsync(sceneName);

            while (!loadingAsyncOperation.isDone)
            {
                yield return null;
            }
        }
        public static float GetLoadingProgress()
        {
            if (loadingAsyncOperation != null)
                return loadingAsyncOperation.progress;
            else
                return 0f;
        }
        public static void LoaderCallback()
        {
            if (onLoaderCallback != null)
            {
                onLoaderCallback();
                onLoaderCallback = null;
            }
        }
    }
}
