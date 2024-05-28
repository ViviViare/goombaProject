using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MedipakScript : MonoBehaviour
{
    [SerializeField] public int _medkitHeal = 30;

    // Used to detect when a player uses their active item
    private void OnEnable()
    {
        playerActiveItem._activeAction += UseActive;
    }
    private void OnDisable()
    {
        playerActiveItem._activeAction -= UseActive;
    }

    // Heals the player up to (but not over) their max health upon use, and updates the player's health UI accordingly.
    private void UseActive()
    {
        playerHealth playhealth = GlobalVariables._player.GetComponent<playerHealth>();
        playhealth._playerHealth += _medkitHeal;
        playhealth._playerHealthBar.GetComponent<Slider>().value = playhealth._playerHealth;
        playhealth._playerHealthText.GetComponent<TextMeshProUGUI>().text = ("Health: " + playhealth._playerHealth + "/" + playhealth._playerMaxHealth);

    }
}
