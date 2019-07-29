using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class GameManager : MonoBehaviour
{

    //// Game
    IGame game;
    public GameType gameType;
    //// GameRenderer
    public Drawer drawer;
    //// UI
    public InputHandler inputHandler;

    // Constants
    public int dim;

    // Use this for initialization
    void Start()
    {
        //gameType = GameType.MapGen;
        mDebug.Log("Starting GameManager with GameType " + gameType.ToString() +"...");
        //game = new Game(this);
        game = new ExodusGame(this);
        drawer.game = game;
        inputHandler.SetGame(game);
    }

   

    // Update is called once per frame
    void Update()
    {
        if (game == null)
        {
            mDebug.Log("Game is null");
        }
        game.TakeTurn();
        drawer.DrawMap();
        drawer.InitCamera();
        bool needsUiUpdate = inputHandler.HandleUserInput();
        
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
