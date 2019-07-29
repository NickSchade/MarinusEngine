using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExodusKeyHandler : IKeyHandler
{
    ExodusGame _game;
    public ExodusKeyHandler(ExodusGame game)
    {
        _game = game;
    }
    public bool HandleKeys(List<KeyCode> keysHeld, List<KeyCode> keysUp, List<KeyCode> keysDown)
    {
        bool updateNeeded = false;
        foreach (KeyCode keyPressed in keysUp)
        {
            updateNeeded = updateNeeded || HandleKeyUp(keyPressed);
        }
        return updateNeeded;
    }
    public bool HandleKeyUp(KeyCode keyPressed)
    {
        bool updateNeeded = false;
        if (keyPressed == KeyCode.Alpha1)
        {
            _game._biomeSystem.RotateView();
            updateNeeded = true;
        }
        if (keyPressed == KeyCode.Alpha2)
        {
            _game._biomeSystem.RotateBiome();
            updateNeeded = true;
        }
        return updateNeeded;
    }
}
