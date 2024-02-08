using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    #region "Singleton"
    private static TutorialManager _instance;

    public static TutorialManager Instance { get { return _instance; } }

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

    [SerializeReference] GameObject tutorialPanel;
    [SerializeField] DialogueSO tutorialDialogue;
    private bool triggerTutorial = false;

    private int tutorialDialogueIndex = 0;
    [SerializeField] DialogueSO[] tutorialPrompts;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnPhaseChanged += TutorialManagerOnPhaseChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator TriggerTutorialPrompt()
    {
        // Play Dialogue
        DialogueManager.Instance.EnqueueDialogueSO(tutorialDialogue, true);
        // Wait to activate panel until Dialogue is over
        yield return new WaitUntil(() => (DialogueManager.Instance.IsDialogueActive() == false));

        tutorialPanel.SetActive(true);
    }

    public bool IsTutorialPromptActive()
    {
        return tutorialPanel.activeSelf;
    }

    public bool IsTutorialWanted()
    {
        return triggerTutorial;
    }

    public void SetTriggerTutorial(bool wantTutorial)
    {
        triggerTutorial = wantTutorial;
        tutorialPanel.SetActive(false);
    }

    public void StartTutorial()
    {

    }

    public void TutorialManagerOnPhaseChange(Phase phase)
    {
        switch (phase)
        {
            case Phase.AIDRAFT:
                // Play Draft Advice
                break;
            case Phase.AIBID:
                // Play Bid Advice
                break;
            case Phase.AITURN:
                // Play Turn Advice
                break;
            case Phase.ENDOFTRICK:
                // Explain end of trick & game loop
                break;
            case Phase.SCORING:
                // Explain scoring (bags, penalties, etc)
                // Then end tutorial and resume normal game
                break;
            case Phase.DIALOGUERESOLVE:
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(phase), phase, null);
        }
    }
}
