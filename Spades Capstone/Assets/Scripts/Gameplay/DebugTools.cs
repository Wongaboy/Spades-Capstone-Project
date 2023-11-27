using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    #region "Class Variables/Data"

    [SerializeReference]
    List<Card> fillerPlayerCards;
    [SerializeReference]
    List<Card> fillerAICards;

    #endregion

    #region "Start/Update"
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    // Fills Player's Hand with standin Cards given by "fillerPlayerCards"
    void FillPlayerHand()
    {
        foreach (Card c in fillerPlayerCards)
        {
            PlayerManager.Instance.DraftCard(c);
        }
    }

    // Fills AI's Hand with standin Cards given by "fillerAICards"
    void FillAIHand()
    {
        foreach (Card c in fillerPlayerCards)
        {
            AIManager.Instance.DraftCard(c);
        }
    }
}
