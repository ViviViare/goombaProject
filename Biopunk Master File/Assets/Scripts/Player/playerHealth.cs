/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 08/02/24

// This script handles player health, death and other associated variables, such as armour plating.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerHealth : MonoBehaviour, IDamageable
{
    [Header("Health and Armour")]
    [SerializeField] private int _playerHealth = 100;
    [SerializeField] private int _playerMaxHealth = 100;
    [SerializeField] private int _playerArmourStacks = 0;

    // Checks constantly to both see if a player's health is at 0 (which triggers the DeathSequence() method) and to clamp the player's health to their _playerMaxHealth variable.
    // This clamping also prevents the player's health from dropping below 0; both of these should hopefully prevent unexpected issues from popping up (such as integer over/underflows)
    void Update()
    {
        if (_playerHealth <= 0)
        {
            DeathSequence();
        }

        _playerHealth = Mathf.Clamp(_playerHealth, 0, _playerMaxHealth);
    }

    // Uses the Unity Scene Manager to reload the current scene upon the player's health reaching zero.
    // Likely will be updated to instead bring up a death screen when the required UI elements are implemented.
    private void DeathSequence()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // This method is from the IDamageable interface. When called by another script, it will reduce the _playerHealth variable by whatever damageAmount is passed into the method.
    public void Damage(int damageAmount)
    {
        _playerHealth -= damageAmount;
    }
}
