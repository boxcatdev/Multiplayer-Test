using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Action<int, int> OnClickedOnGridPosition = delegate { };

    private void Awake()
    {
        #region Singleton
        Instance = this;

        /*if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this) Destroy(gameObject);
        }*/
        #endregion
    }
    public void ClickedOnGridPosition(int x, int y)
    {
        Debug.Log("Clicked on " + x + ", " + y);

        OnClickedOnGridPosition?.Invoke(x, y);
    }
}
