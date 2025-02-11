using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject _textContainer;
    [Space]
    [SerializeField] private TextMeshProUGUI _primaryText;
    [SerializeField] private TextMeshProUGUI _backgroundText;

    public void EnablePrompt(bool enabled)
    {
        if (_primaryText == null) return;
        if (_backgroundText == null) return;

        _textContainer.SetActive(enabled);
    }
    public void SetPromptText(string promptText)
    {
        if (_primaryText == null) return;
        if (_backgroundText == null) return;

        _primaryText.text = promptText;
        _backgroundText.text = promptText;
    }
}
