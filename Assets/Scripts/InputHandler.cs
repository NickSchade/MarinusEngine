using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class InputHandler : MonoBehaviour{

    public IGame _game { get; set; }
    public void SetGame(IGame game)
    {
        _game = game;
    }
    public bool HandleUserInput()
    {
        ScrollHandler.HandleScroll();
        bool updateClick = HandleClick();
        bool updateKeys = HandleKeys();
        return updateClick || updateKeys;
    }
    bool HandleClick()
    {
        bool updateClick = ClickHandler.HandleClick(_game);
        return updateClick;
    }
    bool HandleKeys()
    {
        bool updateKey = false;
        List<KeyCode> inputKeys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToList();
        //List<KeyCode> inputKeys = new List<KeyCode> { KeyCode.Alpha1 };
        List<KeyCode> keysHeld = new List<KeyCode>();
        List<KeyCode> keysUp = new List<KeyCode>();
        List<KeyCode> keysDown = new List<KeyCode>();
        foreach (KeyCode key in inputKeys)
        {
            if (Input.GetKey(key))
            {
                keysHeld.Add(key);
            }
            if (Input.GetKeyUp(key))
            {
                keysUp.Add(key);
            }
            if (Input.GetKeyDown(key))
            {
                keysDown.Add(key);
            }
        }
        _game.HandleKeys(keysHeld, keysUp, keysDown);

        return updateKey;
    }
}
