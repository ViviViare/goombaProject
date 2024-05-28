using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 24/02/24

// Handles music and the switching of music.

// Edits since script completion:
// 05/03/24: Cut down script bloat by a lot, also making the script more modular.
*/
public class MusicManager : MonoBehaviour
{
    [SerializeField] public AudioClip _caveExploreMusic;
    [SerializeField] public AudioClip _caveCombatMusic;

    [SerializeField] public AudioClip _labExploreMusic;
    [SerializeField] public AudioClip _labCombatMusic;

    [SerializeField] public float _fadeDuration = 3f;

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

        _primarySource.volume = 70;
        _secondarySource.volume = 0;

        _currentlyActiveSource = 0;
    }

    // When called, it will crossfade between the currently active music track and the "secondary" music track.
    // Allows us to have combat music that fades in when entering a room populated with enemies.
    public void FadeToSecondary()
    {
        StartCoroutine(StartFade(_audioSources[_currentlyActiveSource], _fadeDuration, 0));
        if(_currentlyActiveSource == 0)
        {
            _currentlyActiveSource = 1;
        }
        else if(_currentlyActiveSource == 1)
        {
            _currentlyActiveSource = 0;
        }
        StartCoroutine(StartFade(_audioSources[_currentlyActiveSource], _fadeDuration, 70));
    }

    // Cuts off the music when the player dies
    public void DeathFade()
    {
        _primarySource.enabled = false;
        _secondarySource.enabled = false;
    }

    // Below method is there to allow a smooth crossfade instead of abrupt transition between our two music tracks
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
