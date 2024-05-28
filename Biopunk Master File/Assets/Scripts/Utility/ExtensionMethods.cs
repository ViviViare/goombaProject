/*  Class created by: Leviathan Vi Amare / ViviViare
//  Creation date: 18/04/24
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  ExtensionMethods.cs
//
//  An extender to unity's default method functionality for specific use cases
//  
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
//  -=-=-=-=-=-=-=-=-=-=-=-=-=-
*/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods 
{
    // Custom extension method to be used to exclusively change the alpha of a graphic
    public static T ChangeAlpha<T>(this T g, float newAlpha)
        where T : Graphic
    {
        var colour = g.color;
        colour.a = newAlpha;
        g.color = colour;
        return g;
    }


    // Gets all children of a parent of a specific type
    public static List<T> GetChildrenOfType<T>(this Transform parent)
    where T : Component
    {
        List<T> children = new List<T>();
        
        foreach (Transform child in parent)
        {
            T component = child.GetComponent<T>();
            if (component == null) continue;

            children.Add(component);
        }
        return children;
    }   
}
