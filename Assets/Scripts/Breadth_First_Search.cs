using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breadth_First_Search
{
    public static bool Clear(GameObject gameObject)
    {
        //return Physics.Raycast(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));

        RaycastHit hit;
        var ray = new Ray(gameObject.transform.position, gameObject.transform.TransformDirection(Vector3.up));
        if (Physics.Raycast(ray, out hit, 25))
        {
            Debug.Log(hit.collider.name);
            return true;
        }
        return false;
    }
}
