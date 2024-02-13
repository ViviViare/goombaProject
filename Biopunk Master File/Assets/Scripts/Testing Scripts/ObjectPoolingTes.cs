using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPoolingTes : MonoBehaviour
{

    private PCInputs _playerInput;

    [SerializeField] GameObject testobject;
    [SerializeField] GameObject testobject2;


    // Start is called before the first frame update
    void Start()
    {
        _playerInput = new PCInputs();
        _playerInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            ObjectPooler.Spawn(testobject, this.gameObject.transform.position, Quaternion.identity);
        }
    }
}
