using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] public AudioClip _caveExploreMusic;
    [SerializeField] public AudioClip _caveCombatMusic;

    [SerializeField] public AudioClip _labExploreMusic;
    [SerializeField] public AudioClip _labCombatMusic;


    [SerializeField] public AudioSource _primarySource;
    [SerializeField] public AudioSource _secondarySource;

    [SerializeField] public int _currentlyActiveSource;

    [SerializeField] public List<AudioSource> _audioSources;

    private void Start()
    {
        _primarySource.clip = _caveExploreMusic;
        _secondarySource.clip = _caveCombatMusic;

        _primarySource.Play(0);
        _secondarySource.Play(0);

        _primarySource.volume = 100;
        _secondarySource.volume = 0;

        _currentlyActiveSource = 0;
    }


    public void FadeToSecondary()
    {
        StartCoroutine(StartFade(_audioSources[_currentlyActiveSource], 1, 0));
        if(_currentlyActiveSource == 0)
        {
            _currentlyActiveSource = 1;
        }
        else if(_currentlyActiveSource == 1)
        {
            _currentlyActiveSource = 0;
        }
        StartCoroutine(StartFade(_audioSources[_currentlyActiveSource], 1, 100));
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
