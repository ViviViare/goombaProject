using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class activePickup : MonoBehaviour
{
    // When placed onto an active item pickup, defines what the active actually is (based on its index within a list of active items) and what the item's maximum charge is.

    [SerializeField] public int _activeIndex;
    [SerializeField] public int _activeMaxCharge;
    [SerializeField] public Sprite _sprite;
}
