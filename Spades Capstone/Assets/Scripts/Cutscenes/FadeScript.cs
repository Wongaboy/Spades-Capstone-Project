using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScript : MonoBehaviour
{
    public Animator animator;
    public string introSceneName;

    #region "Singleton"
    private static FadeScript _instance;

    public static FadeScript Instance { get { return _instance; } }

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

    public void FadeOutWhite()
    {
        animator.SetTrigger("WhiteFadeOut");
    }

    public void FadeInWhite()
    {
        animator.SetTrigger("WhiteFadeIn");
    }

    public void FadeOutBlack()
    {
        animator.SetTrigger("BlackFadeOut");
    }

    public void FadeInBlack()
    {
        animator.SetTrigger("BlackFadeIn");
    }

    public void OnBlackFadeComplete()
    {
        SceneManager.LoadScene(introSceneName);        
    }

    public void OnWhiteFadeComplete()
    {
        SceneManager.LoadScene(introSceneName);        
    }
}
