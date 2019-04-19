using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    public IGame game;
	
    public bool HandleUserInput()
    {
        ScrollHandler.HandleScroll();
        return HandleClick();
    }
    void HandleScroll()
    {
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            Camera.main.GetComponent<Camera>().orthographicSize -= 1;
        }
        else if (d < 0f)
        {
            Camera.main.GetComponent<Camera>().orthographicSize += 1;
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
    bool HandleClick()
    {
        bool updateClick = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            foreach (RaycastHit hit in hits)
            {
                Clickable clicked = hit.transform.gameObject.GetComponentInParent<Clickable>();

                if (clicked != null)
                {
                    mDebug.Log("Clicked " + clicked.pos.gridLoc.key(), false);
                    updateClick = game.HandleClick(clicked.pos, Input.GetMouseButtonUp(0), Input.GetMouseButtonUp(1));
                }
                // Debug.Log("Hovering " + clicked.pos.gridLoc.key());
            }
        }
        return updateClick;
    }
}
