using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnroom_Items : MonoBehaviour
{
    [SerializeField] private GameObject _spawnPoint;

    [SerializeField] public GameObject _itemToSpawn1;
    [SerializeField] private GameObject _itemToSpawn2;

    [SerializeField] private GameObject _doorHandler;

    [SerializeField] private bool _itemGrabbed = false;

    // Start is called before the first frame update
    void Start()
    {
        _doorHandler.GetComponent<RoomStatus>().ToggleDoors();

        int itemToSpawn = Random.Range(0, 10);

        if(itemToSpawn <= 5)
        {
            ObjectPooler.Spawn(_itemToSpawn1, _spawnPoint.transform.position, Quaternion.identity);
        }
        else if(itemToSpawn > 5)
        {
            ObjectPooler.Spawn(_itemToSpawn2, _spawnPoint.transform.position, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalVariables.__startingDoorsOpened == true) return;

        if(GlobalVariables._startingItemGrabbed == true)
        {
            GlobalVariables.__startingDoorsOpened = true;
            _itemGrabbed = true;
            _doorHandler.GetComponent<RoomStatus>().ToggleDoors();
        }
    }
}
