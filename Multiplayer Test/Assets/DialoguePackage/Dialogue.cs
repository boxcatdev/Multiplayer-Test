using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("The text that will be displayed in the header of the dialogue box.")]
    public string _speakerName;

    [Tooltip("The text that will be displayed in the body of the dialogue box.")]
    [TextArea(3, 10)]
    public string[] _sentences;
}
