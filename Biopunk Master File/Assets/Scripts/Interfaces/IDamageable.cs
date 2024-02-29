/*
// Interface created by Mateusz Korcipa / Forkguy13
// Creation date: 10/02/24

// This interface reduces repetion of damage methods and variables; instead of having to repeat a damage method for every damageable object,
// a damageable object will simply include this interface within its script.

// Edits since script completion:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(int damageAmount);
}
