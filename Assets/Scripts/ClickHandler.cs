﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClickHandler
{
    public static bool HandleClick(IGame game)
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
