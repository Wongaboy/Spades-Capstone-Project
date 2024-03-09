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
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsMenu(!isSettingMenuActive);
        }
    }

    void ToggleSettingsMenu(bool newState)
    {
        settingsMenuPanel.SetActive(newState);
        isSettingMenuActive = newState;
      
        DialogueManager.Instance.SetDialogueInteractable(!newState);
        PlayerManager.Instance.SetPlayerCardInteractable(!newState);       
    }

    public bool GetMenuActiveState()
    {
        return isSettingMenuActive;
    }
    public void MenuRestartGame()
    {
        SceneManager.LoadScene(introSceneName);
    }

    public void MenuQuitGame()
    {
        Application.Quit();
    }
}
