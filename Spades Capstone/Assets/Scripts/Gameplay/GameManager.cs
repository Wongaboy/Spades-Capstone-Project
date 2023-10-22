using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region "Singleton"
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    [SerializeField] public Deck deck;
    [SerializeField] public DraftManager draftManager;
    public Phase currentPhase;
    public static event Action<Phase> OnPhaseChanged;
    public Character lead;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize game components
        lead = Character.DEATH; // Death always goes first for tutorial
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePhase(Phase newPhase)
    {
        currentPhase = newPhase;
        switch (newPhase)
        {
            case Phase.DRAFT:
                ShuffleDeck();
                break;
            case Phase.PLAYERTURN:
                break;
            case Phase.AITURN:

                break;
            case Phase.SCORING:
                // Clear hands
                // update scoreboard
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(newPhase), newPhase, null);
        }

        OnPhaseChanged?.Invoke(newPhase);
    }

    private void ShuffleDeck()
    {
        deck.Shuffle();
    }
}
public enum Phase { DRAFT, PLAYERTURN, AITURN, SCORING };
public enum Character { DEATH, PLAYER};
