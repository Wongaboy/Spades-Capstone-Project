using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueInstance", menuName = "ScriptableObjects/DialogueInstance")]
public class DialogueSO : ScriptableObject
{
    public string dialogueTitle;
    public string dialogueName;
    [TextArea(10, 20)]
    public List<string> dialogueTexts;
}
