using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;




public class MarinusGame: IGame
{
    public TileShape tileShape { get; set; }
    public IMap map { get; set; }
    public GameManager gameManager;
    public int tick { get; set; }

    public IKeyHandler _keyHandler;
    public ExodusBiomeSystem _biomeSystem { get; set; }
    public ILocationColor _locationColor { get; set; }

    protected int dim;
    protected bool wrapEastWest = false;
    protected bool wrapNorthSouth = false;
    protected float percentSea = 0.5f;
    protected float percentRiver = 0.20f;
    
    public MarinusGame(GameManager _gameManager)
    {
        Initialize(_gameManager);
        map = new MarinusMap(this, gameManager.gameType, dim, dim, wrapEastWest, wrapNorthSouth, percentSea, percentRiver);
    }
    public void Initialize(GameManager _gameManager)
    {
        gameManager = _gameManager;
        dim = gameManager.dim;
        tick = 0;
    }

    public bool HandleClick(Pos pos, bool leftClick, bool rightClick)
    {
        map.lands[pos].Click();
        return true;
    }

    public void TakeTurn()
    {
        tick++;
        foreach (ILocation loc in map.lands.Values)
        {
            loc.TakeTurn();
        }
    }

    public bool HandleKeys(List<KeyCode> keysHeld, List<KeyCode> keysUp, List<KeyCode> keysDown)
    {
        return _keyHandler.HandleKeys(keysHeld, keysUp, keysDown);
    }
}



