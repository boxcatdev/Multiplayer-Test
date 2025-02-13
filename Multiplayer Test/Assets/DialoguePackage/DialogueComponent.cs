using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueComponent : MonoBehaviour
{
    [Header("Dialogue Display ")]
    [Tooltip("The gameobject that will be set active and inactive.")]
    [SerializeField] GameObject _dialogueBox;
    [Tooltip("The text field that will display the header text.")]
    [SerializeField] TextMeshProUGUI _headerText;
    [Tooltip("The text field that will display the body text.")]
    [SerializeField] TextMeshProUGUI _bodyText;
    [Space]
    [Tooltip("The text that will be displayed in the dialogue box.")]
    [SerializeField] Dialogue _dialogue;

    private Queue<string> sentences;

    private void Start()
    {
        sentences = new Queue<string>();
        _dialogueBox.SetActive(false);
    }

    private void ChangeText()
    {
        if (_headerText.text != _dialogue._speakerName)
            _headerText.text = _dialogue._speakerName;
    }
    public void StartDialogue()
    {
        Debug.Log("Start of conversation...");

        //unhide dialogue box and update text
        if (_dialogueBox.activeInHierarchy == false)
            _dialogueBox.SetActive(true);

        //update header
        ChangeText();

        //update sentences
        sentences.Clear();

        foreach (var sentence in _dialogue._sentences)
        {
            sentences.Enqueue(sentence);
        }

        //start queueing
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        //display dialogue
        _bodyText.text = sentence;

    }
    public void EndDialogue()
    {
        Debug.Log("End of conversation...");
        _dialogueBox.SetActive(false);
    }
}
