using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CheatInstance", menuName = "ScriptableObjects/CheatInstance")]
public class CheatSO : ScriptableObject
{
    public string cheatName;
    public bool canUse = true;
    public DialogueSO cheatDialogue;
}
