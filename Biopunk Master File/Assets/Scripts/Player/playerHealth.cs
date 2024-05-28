/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 08/02/24

// This script handles player health, death and other associated variables, such as armour plating.

// Edits since script completion:
// 19/03/24: Re-updated health script to implement immunity timers.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class playerHealth : MonoBehaviour, IDamageable
{
    [Header("Health and Armour")]
    [SerializeField] public int _playerHealth = 100;
    [SerializeField] public int _playerMaxHealth = 100;

    [SerializeField] public int _playerMaxArmourStacks = 3;
    [SerializeField] public int _playerArmourStacks = 0;

    [SerializeField] public bool _canBeDamaged = true;
    [SerializeField] public float _immunityTime = 1.5f;

    [SerializeField] public GameObject _playerHealthBar;
    [SerializeField] public GameObject _playerHealthText;

    [SerializeField] public GameObject _playerArmourBar;
    [SerializeField] public GameObject _playerArmourText;

    [SerializeField] public AudioClip _deathSound;

    void Start()
    {
        _playerHealthBar = GameObject.FindGameObjectWithTag("Healthbar");
        _playerHealthBar.GetComponent<Slider>().maxValue = _playerMaxHealth;
        _playerHealthBar.GetComponent<Slider>().value = _playerHealth;
         
        //_playerHealthText = GameObject.FindGameObjectWithTag("Healthtext");
       // _playerHealthText.GetComponent<TextMeshProUGUI>().text = ("Health: " + _playerHealth + "/" + _playerMaxHealth);

        _playerArmourBar = GameObject.FindGameObjectWithTag("Armourbar");
        _playerArmourBar.GetComponent<Slider>().maxValue = _playerMaxArmourStacks;
        _playerArmourBar.GetComponent<Slider>().value = _playerArmourStacks;
    }

    // Checks constantly to both see if a player's health is at 0 (which triggers the DeathSequence() method) and to clamp the player's health to their _playerMaxHealth variable.
    // This clamping also prevents the player's health from dropping below 0; both of these should hopefully prevent unexpected issues from popping up (such as integer over/underflows)
    void Update()
    {
        _playerHealth = Mathf.Clamp(_playerHealth, 0, _playerMaxHealth);
        _playerArmourStacks = Mathf.Clamp(_playerArmourStacks, 0, _playerMaxArmourStacks);
    }

    // Uses the Unity Scene Manager to reload the current scene upon the player's health reaching zero.
    // Likely will be updated to instead bring up a death screen when the required UI elements are implemented.
    private void DeathSequence()
    {
        MusicManager musicman = GameObject.FindGameObjectWithTag("Musicmanager").GetComponent<MusicManager>();
        musicman.DeathFade();
        AudioSource.PlayClipAtPoint(_deathSound, this.transform.position);
        this.gameObject.GetComponent<pauseMenu>().OpenDeathMenu();
        ObjectPooler.Clear();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // This method is from the IDamageable interface. When called by another script, it will reduce the _playerHealth variable by whatever damageAmount is passed into the method.
    public void Damage(int damageAmount)
    {
        if (_canBeDamaged == false) return;
        if(_playerArmourStacks > 0)
        {
            StartCoroutine(ImmunityTimer());
            _playerArmourStacks--;
            _playerArmourBar.GetComponent<Slider>().value = _playerArmourStacks;
        }
        else
        {
            StartCoroutine(ImmunityTimer());
            _playerHealth -= damageAmount;
            _playerHealthBar.GetComponent<Slider>().value = _playerHealth;
            //_playerHealthText.GetComponent<TextMeshProUGUI>().text = ("Health: " + _playerHealth + "/" + _playerMaxHealth);
            if (_playerHealth <= 0)
            {
                DeathSequence();
            }
        }
    }

    // Serves to prevent the player from taking damage if they have just recently taken damage (so that the player does not get overwhelmed and killed instantly)
    IEnumerator ImmunityTimer()
    {
        _canBeDamaged = false;
        yield return new WaitForSeconds(_immunityTime);
        _canBeDamaged = true;
    }
}
