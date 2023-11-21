using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueInstance", menuName = "ScriptableObjects/DialogueInstance")]
public class DialogueSO : ScriptableObject
{
    public string dialogueName;
    [TextArea(5, 10)]
    public List<string> dialogueTexts;
}
