/*
// Class created by Mateusz Korcipa / Forkguy13
// Creation date: 08/02/24

// This script keeps track of and allows modification of the player's various attributes.
// Player attributes should be self-explanitory, but weapon stats such as _playerDamage are *not* the damage a weapon will do.
// This will be defined in a weapon's script itself, as a base damage value. The _playerDamage value found here is a *modifier* that will multiply a weapon's base damage value.
// This allows us to create items that can boost a player's damage universally, no matter what weapons they equip.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerStats : MonoBehaviour
{
    [Header("Weapon Stats & Multipliers")]
    [SerializeField] private float _playerDamage= 1f;
    [SerializeField] private float _playerAttackSpeed = 1f;

    [Header("Player Stats")]
    [SerializeField] public float _playerSpeed = 1f;

}
