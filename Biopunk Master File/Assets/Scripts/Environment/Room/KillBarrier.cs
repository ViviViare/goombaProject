using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBarrier : MonoBehaviour
{

    // Very basic kill barrier, that damages whatever touches it for 99999 damage.
    // Created to mitigate the Nautt splitting issue, as sometimes the enemies Nautt spawned when he splits would fall underneath the level, soft-locking the player.
    // Now, instead of falling through the void forever and preventing the player from clearing a room, they will simply be killed by this killbarrier, allowing the player to continue.

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<IDamageable>() != null)
        {
            other.gameObject.GetComponent<IDamageable>().Damage(99999);
        }
    }
}
