using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class GameManager : MonoBehaviour
{

    //// Game
    IGame game;
    //// GameRenderer
    public Drawer drawer;
    //// UI
    public InputHandler inputHandler;

    // Constants


    // Use this for initialization
    void Start()
    {
        mDebug.Log("Starting GameManager...");
        game = new Game(this);
        drawer.game = game;
        inputHandler.game = game;
    }

   

    // Update is called once per frame
    void Update()
    {
        drawer.DrawMap();
        drawer.InitCamera();
        bool needsUiUpdate = inputHandler.HandleUserInput();
        //if (game.turnNumber == 0)
        //{
        //    game.turnNumber++;
        //    drawer.DrawInit();
        //}
        //drawer.Animate();
        //if (NeedsUpdate())
        //{
        //    drawer.DrawTakeTurn();
        //}
    }

    bool NeedsUpdate()
    {
        //bool updateRealtime = game.RealTimeTurn();
        //bool updateClick = uihandler.HandleMouse();
        //bool updateKey = uihandler.HandleKeys();
        //return updateRealtime || updateClick || updateKey;
        return true;
    }


    // UI
    public void Button_TakeTurn()
    {
        //game.TakeTurn();
        //drawer.DrawTakeTurn();
    }
    public void Button_TogglePause()
    {
        //game.paused = !game.paused;
        //drawer.UpdateButtons();
    }
    public void Button_ToggleSelectedAction()
    {
        //game.ToggleAction();
        //drawer.UpdateButtons();
    }

}
