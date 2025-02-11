using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugController : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!Application.isEditor)
            {
                Application.Quit();
            }
        }
    }
}
