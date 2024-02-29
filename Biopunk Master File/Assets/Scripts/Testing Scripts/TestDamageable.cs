
using UnityEngine;

public class TestDamageable : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health = 10;
    public void Damage(int damageAmount)
    {
        _health -= damageAmount;
        Debug.Log(_health);
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
       
    }
}
