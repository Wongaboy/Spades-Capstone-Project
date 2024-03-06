using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
