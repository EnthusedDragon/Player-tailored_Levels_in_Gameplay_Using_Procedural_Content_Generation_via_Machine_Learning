using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    bool setup = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!setup)
            Setup();
    }

    void Setup()
    {
        var parent = GameObject.Find("Maze");

        // First find a center for your bounds.
        Vector3 center = Vector3.zero;

        foreach (Transform child in parent.transform)
        {
            center += child.GetComponent<Renderer>().bounds.center;
        }
        center /= parent.transform.childCount; //center is average center of children

        //Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
        Bounds bounds = new Bounds(center, Vector3.zero);

        foreach (Transform child in parent.transform)
        {
            bounds.Encapsulate(child.GetComponent<Renderer>().bounds);
        }

        var camHeight = Mathf.Max(bounds.size.x, bounds.size.z);
        
        transform.position = new Vector3((bounds.size.x / 2)-2.5f, camHeight * 1.5f, (bounds.size.z / 2) - 2.5f);

        setup = true;
    }
}
