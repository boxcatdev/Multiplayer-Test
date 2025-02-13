using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkConnectUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
                HideUI();
            });
        }
        if (clientButton != null)
        {
            clientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
                HideUI();
            });
        }
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
    }
}
