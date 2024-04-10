using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CotagrasAI : MonoBehaviour
{
    #region Variables
    [Header("Data")]
    [ShowOnly] public bool _aiActivated;
    private GameObject _player;
    
    [Header("References")]
    private CharacterController _controller;
    [SerializeField] private Transform _leapTargetPoint;
    [SerializeField] private Transform _leapHighPoint;
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private float _raycastLength;
    [Space]

    [Header("Health Stats")]
    [ShowOnly] public int _health;
    [SerializeField] private int _maxHealth;
    [Space]

    [Header("Damage Stats")]
    [SerializeField] private int _chargeDamage; 
    [SerializeField] private int _leapingDamage;
    [SerializeField] private int _thrashDamage;
    [SerializeField] private int _maxChargeBounces;
    [SerializeField] private int _maxLeaps;
    [Space]
    
    [Header("Damage Ranges")]
    [SerializeField] private float _chargeDmgRadius; 
    [SerializeField] private float _leapDmgRadius; 
    [SerializeField] private float _thrashDmgRadius;
    [Space]

    [Header("Speed Stats")]
    [SerializeField] private float _wanderingSpeed;
    [SerializeField] private float _chargeSpeed;
    [SerializeField] private float _leapingSpeed, _leapingGravity;
    
    [Header("Tuning")]
    [SerializeField] private float _minLeapDistance;
    [SerializeField] private float _maxLeapDistance;
    [SerializeField] private float _leapHighPointDistance;
    private int _directLeapChance;

    [Header("Textures")]
    private Material _mainTexture;
    private bool _isShiny;    
    [SerializeField] private Material _shinyTexture;
    [SerializeField] private GameObject _enemyModel;

    #endregion

    private void Start()
    {
        SetupCotagras();
        //ActivateAI(); // Temp call, delete after.
        //StartCoroutine(CheckForWalls());
        //LeapTowards();
    }

    private void Update()
    {
        ChargeForward();
    }

    private void SetupCotagras()
    {
        _player = GlobalVariables._player;
        
        _health = _maxHealth;

        _aiActivated = false;

        _controller = GetComponent<CharacterController>();

        Shiny();
    }

    public void ActivateAI()
    {
        _aiActivated = true;
    }

    #region Charge Attack
    private void ChargeForward()
    {
        CheckForWalls();

        Vector3 moveDirection = (Vector3.forward * _chargeSpeed) * Time.deltaTime;
        _controller.Move(moveDirection);
    }

    private void CheckForWalls()
    {
        RaycastHit hit;
        if (Physics.Raycast(_raycastOrigin.position, Vector3.forward, out hit, _raycastLength ) )
        {
            // If it cannot find a wall continue the loop
            if (!hit.collider.gameObject.CompareTag("Wall")) return;
            transform.rotation = Quaternion.Euler(0, -transform.rotation.y, 0);

        }
        
    }

    #endregion

    #region Leap Attack
    
    private void LeapTowards()
    {
        Vector3 target = FindLeapTarget();

        Vector3 startPoint = transform.position;
        Debug.Log(startPoint);
        
        Vector3 midpointOfTarget = (transform.position + target) / 2;
        Vector3 highPoint = midpointOfTarget + Vector3.up * _leapHighPointDistance;

        Debug.Log($"origin: {startPoint} | high: {highPoint} |  target:  {target}");
        StartCoroutine(AnimateLeap(startPoint, highPoint, target));
    }

    
    private Vector3 FindLeapTarget()
    {
        Vector3 target = _player.transform.position;

        int chanceToHitPlayer = Random.Range(_directLeapChance, 100);
        // Chance to hit the player dead on where they currently stand
        if (chanceToHitPlayer >= 100)
        {
            _directLeapChance = 10;
            return target;
        }
        else
        {
            // Increase chance to hit player each jump
            _directLeapChance += Mathf.FloorToInt(chanceToHitPlayer);
        }

        float newX;
        float newZ;

        do 
        {
            newX = Random.Range(-_maxLeapDistance, _maxLeapDistance);
            newZ = Random.Range(-_maxLeapDistance, _maxLeapDistance);
            target.x = newX;
            target.z = newZ;
        }
        while (!TargetIsValid(target));

        return target;


    }
    
    private bool TargetIsValid(Vector3 target)
    {
        if (Vector3.Distance(target, transform.position) < _minLeapDistance ) return false;

        RaycastHit hit;
        if (Physics.Raycast(target + (Vector3.up * 2), Vector3.down, out hit, Mathf.Infinity) )
        {
            return true;
        }
        return false;
    }


    private IEnumerator AnimateLeap(Vector3 startPoint, Vector3 highestPoint, Vector3 endPoint)
    {
        float startTime = Time.time;

        while (Time.time < startTime + _leapingSpeed)
        {
            float fracComplete = (Time.time - startTime) / _leapingSpeed;
            transform.position = CalculateSlerp(startPoint, highestPoint, endPoint, fracComplete);
            yield return null;
        }

        transform.position = endPoint;
        

    }

    private Vector3 CalculateSlerp(Vector3 startPoint, Vector3 highestPoint, Vector3 endPoint, float time)
    {
        Vector3 startToHigh = (highestPoint - startPoint).normalized;
        Vector3 startToEnd = (endPoint - startPoint).normalized;
        
        Quaternion fromTo = Quaternion.FromToRotation(startToHigh, startToEnd);
        Quaternion slerpRotation = Quaternion.Slerp(Quaternion.identity, fromTo, time);

        Vector3 direction = slerpRotation * startToHigh;

        float distance = Vector3.Distance(startPoint, endPoint);
        float height = Vector3.Distance(highestPoint, startPoint);
        float arcHeight = height * (1 - 4 * (time - 0.5f) * (time - 0.5f));

        Vector3 arcPoint = startPoint + direction * distance + Vector3.up * arcHeight;
        
        Debug.Log(startPoint + direction * distance + Vector3.up * arcHeight);
        return arcPoint;
    }

    #endregion

    #region Thrash Attack


    #endregion

    private void Shiny()
    {
        // Decide if this enemy should be a shiny
        int shinyChance = Random.Range(0, 10);
        Material textureToUse = _mainTexture;
        if (shinyChance <= 0 && _shinyTexture != null) textureToUse = _shinyTexture;

        // If this enemy is not to be shiny and was not shiny already, stop running the code
        if (textureToUse == _mainTexture && !_isShiny) return;

        // Change the material on the enemy to be either shiny or revert from shiny back to normal
        foreach (Renderer child in _enemyModel.GetComponentsInChildren<Renderer>() )
        {
            child.material = _shinyTexture;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _chargeDmgRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _leapDmgRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _thrashDmgRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _minLeapDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxLeapDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(_raycastOrigin.position, _raycastOrigin.position + (Vector3.forward * _raycastLength));

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * _leapHighPointDistance));
    }
}
