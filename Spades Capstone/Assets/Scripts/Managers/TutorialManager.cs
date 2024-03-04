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
    [SerializeField] DialogueSO tutorialPrompt;
    public bool triggerTutorial = false;

    private int tutorialDialogueIndex = 0;
    [SerializeField] DialogueSO[] tutorialPhaseDialogues;

    public IEnumerator TriggerTutorialPrompt()
    {
        // Play Dialogue
        DialogueManager.Instance.EnqueueDialogueSO(tutorialPrompt, true);
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
        if (wantTutorial == true) { GameManager.OnPhaseChanged += TutorialManagerOnPhaseChange; }
        tutorialPanel.SetActive(false);
    }

    public void EnqueueNextDialogue(bool triggerNow)
    {
        DialogueManager.Instance.EnqueueDialogueSO(tutorialPhaseDialogues[tutorialDialogueIndex], triggerNow);
        tutorialDialogueIndex++;
    }

    public void TutorialManagerOnPhaseChange(Phase phase)
    {
        switch (phase)
        {
            // These Dialogues Play after the Case Phase
            case Phase.AIDRAFT:
                // Play Draft Advice               
                if (tutorialDialogueIndex == 0) { EnqueueNextDialogue(false); }
                break;
            case Phase.AIBID:
                // Play Bid Advice
                if (tutorialDialogueIndex == 1) { EnqueueNextDialogue(false); }
                break;
            case Phase.AITURN:
                // Play Turn Advice
                if (tutorialDialogueIndex == 2) { EnqueueNextDialogue(false); }
                break;
            case Phase.ENDOFTRICK:
                // Explain end of trick & game loop
                if (tutorialDialogueIndex == 3) { EnqueueNextDialogue(false); }
                break;
            case Phase.SCORING:
                // Explain scoring (bags, penalties, etc)
                if (tutorialDialogueIndex == 4) { EnqueueNextDialogue(false); }
                break;
            default:
                break;
        }
    }

    public IEnumerator EndTutorial()
    {
        EnqueueNextDialogue(true);
        yield return new WaitUntil(() => (DialogueManager.Instance.IsDialogueActive() == false));

        GameManager.OnPhaseChanged -= TutorialManagerOnPhaseChange;
        GameManager.Instance.isInTutorial = false;
        DialogueManager.Instance.TurnOffPressSpace(false);

        ScoreManager.Instance.ResetTallyBoardScores();
        GameManager.Instance.ResetGM();

        // Reset Trick Lead
        GameManager.Instance.ResetInternalTrickLead(Character.DEATH);

        yield return new WaitForSeconds(8.2f);

        GameManager.Instance.ChangePhase(Phase.AIDRAFT);
    }

    public void UnsubscribeTutorialListeners()
    {
        GameManager.OnPhaseChanged -= TutorialManagerOnPhaseChange;
    }
}
