using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuManager : MonoBehaviour
{
    #region "Singleton"
    private static SettingsMenuManager _instance;

    public static SettingsMenuManager Instance { get { return _instance; } }

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

    #region "Class Variables"
    // Settings Menu Panel GameObject
    [SerializeField] GameObject settingsMenuPanel;

    private bool isSettingMenuActive = false;

    [SerializeField] private string introSceneName;

    private List<Card> cardsToToggle = new List<Card>();
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsMenu(!isSettingMenuActive);
        }
    }

    // Turn On/Off Settings Menu UI Overlay; Changes to opposite of current state
    // Prevents Card interact by turning off interaction, but turn on after menu closes
    void ToggleSettingsMenu(bool newState)
    {
        settingsMenuPanel.SetActive(newState);
        isSettingMenuActive = newState;
      
        DialogueManager.Instance.SetDialogueInteractable(!newState);

        if (newState == true)
        {
            cardsToToggle = GameManager.Instance.GetInteractableCards();
            foreach (Card card in cardsToToggle) { card.SetInteractable(false); }
        }
        else
        {
            foreach (Card card in cardsToToggle) { card.SetInteractable(true); }
            cardsToToggle.Clear();
        }

    }

    // Get bool -> active state of Setting Menu UI Overlay
    public bool GetMenuActiveState()
    {
        return isSettingMenuActive;
    }

    // Restart Game Scene; Called by UI Button
    public void MenuRestartGame()
    {
        SceneManager.LoadScene(introSceneName);
    }

    // Close Game Application; Called by UI Button
    public void MenuQuitGame()
    {
        Application.Quit();
    }
}
