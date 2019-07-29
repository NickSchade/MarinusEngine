using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExodusGame : MarinusGame, IGame
{
    public ExodusLocationColor _colorLocation;
    public ExodusGame(GameManager gameManager)
    {
        Initialize(gameManager);    
        map = new MarinusMap(this, gameManager.gameType, dim, dim, wrapEastWest, wrapNorthSouth, percentSea, percentRiver);
        _biomeSystem = new ExodusBiomeSystem(ExodusSettings.numberOfBiomes);
        _colorLocation = new ExodusLocationColor(this._biomeSystem);
        _locationColor = new ExodusLocationColor(this._biomeSystem);
        _keyHandler = new ExodusKeyHandler(this);
    }
}
