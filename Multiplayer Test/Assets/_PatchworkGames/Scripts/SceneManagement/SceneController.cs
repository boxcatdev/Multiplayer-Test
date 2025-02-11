using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PatchworkGames
{
    public class SceneController : MonoBehaviour
    {
        public static void LoadScene(string sceneName)
        {
            Time.timeScale = 1;
            Loader.Load(sceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
