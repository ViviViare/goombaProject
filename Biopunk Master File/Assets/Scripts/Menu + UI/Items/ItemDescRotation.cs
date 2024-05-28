/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 27/04/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  ItemDescRotation.cs
//
//  Simple script to make a UI element always face the main camera
//  
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
*/
using UnityEngine;

public class ItemDescRotation : MonoBehaviour
{
    public void Update()
    {
        Camera camera = Camera.main;
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }
}
