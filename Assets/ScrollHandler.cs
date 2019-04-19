using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScrollHandler  {

    public static void HandleScroll()
    {
        mDebug.Log("Handling Scroll...",false);
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d!= 0)
        {
            if (d > 0f)
            {
                Camera.main.GetComponent<Camera>().orthographicSize -= 1;
            }
            else if (d < 0f)
            {
                Camera.main.GetComponent<Camera>().orthographicSize += 1;
            }
        }
        
        if (Input.GetKey("w"))
        {
            Vector3 p = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(p.x, p.y, p.z + 1);
        }
        if (Input.GetKey("s"))
        {
            Vector3 p = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(p.x, p.y, p.z - 1);
        }
        if (Input.GetKey("a"))
        {
            Vector3 p = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(p.x - 1, p.y, p.z);
        }
        if (Input.GetKey("d"))
        {
            Vector3 p = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(p.x + 1, p.y, p.z);
        }
    }
}
