using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    #region "Singleton"
    private static SoundFXManager _instance;

    public static SoundFXManager Instance { get { return _instance; } }

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
    // SoundFXObject Prefab reference
    [SerializeField] private AudioSource soundFXObject;

    // Card Play/Draw SFX clip
    [SerializeField] private AudioClip cardPlayDrawClip;
    #endregion

    // One Time SFX clips, (Do not use for SFX that persist, OR are meant to stop when condition met)
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawn in GameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign Audioclip
        audioSource.clip = audioClip;

        //Assign Volume
        audioSource.volume = volume;

        //Play Sound
        audioSource.Play();

        //Get Clip Length
        float clipLength = audioSource.clip.length;

        //Destroy Clip object after done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    // Plays Specifically the CardSFX only
    public void PlayCardSFX(Transform spawnTransform, float volume)
    {
        //Spawn in GameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign Audioclip
        audioSource.clip = cardPlayDrawClip;

        //Assign Volume
        audioSource.volume = volume;

        //Play Sound
        audioSource.Play();

        //Get Clip Length
        float clipLength = audioSource.clip.length;

        //Destroy Clip object after done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource PlayLoopingSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawn in GameObject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        //Assign Audioclip
        audioSource.clip = audioClip;

        //Assign Volume
        audioSource.volume = volume;

        //Make SFX loop
        audioSource.loop = true;

        //Play Sound
        audioSource.Play();

        //Get Clip Length
        //float clipLength = audioSource.clip.length;

        //Destroy Clip object after done playing
        //Destroy(audioSource.gameObject, clipLength);

        return audioSource;
    }
}
