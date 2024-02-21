using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [SerializeField] DialogueSO goodEndingDialogue;
    [SerializeField] DialogueSO badEndingDialogue;

    [SerializeReference] GameObject goodEndingCamera;
    [SerializeReference] GameObject badEndingCamera;
    [SerializeReference] GameObject mainCamera;

    [SerializeReference] GameObject goodTimeline;
    [SerializeReference] GameObject badTimeline;

    #region "Singleton"
    private static EndingManager _instance;

    public static EndingManager Instance { get { return _instance; } }

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

    public IEnumerator TriggerEndCutscene(Character winner)
    {
        switch (winner)
        {
            case Character.PLAYER:
                DialogueManager.Instance.EnqueueDialogueSO(goodEndingDialogue, true);
                yield return new WaitUntil(() => (DialogueManager.Instance.IsDialogueActive() == false));
                goodEndingCamera.SetActive(true);
                mainCamera.SetActive(false);
                goodTimeline.SetActive(true);
                yield return new WaitForSeconds(4.5f);
                FadeScript.Instance.FadeOutWhite();
                break;

            case Character.DEATH:
                DialogueManager.Instance.EnqueueDialogueSO(badEndingDialogue, true);
                yield return new WaitUntil(() => (DialogueManager.Instance.IsDialogueActive() == false));
                badEndingCamera.SetActive(true);
                mainCamera.SetActive(false);
                badTimeline.SetActive(true);
                yield return new WaitForSeconds(4.5f);
                FadeScript.Instance.FadeOutBlack();
                break;

            default:
                Debug.Log("No Cutscene exists for that winner");
                break;
        }        
    }
}
