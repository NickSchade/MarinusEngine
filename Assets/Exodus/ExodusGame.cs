using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExodusGame : MarinusGame, IGame
{
    public ExodusLocationColor _colorLocation;
    public ExodusGame(GameManager gameManager) : base(gameManager)
    {
        _colorLocation = new ExodusLocationColor(this);
        _locationColor = new ExodusLocationColor(this);
        _biomeSystem = new ExodusBiomeSystem(ExodusSettings.numberOfBiomes);
        _keyHandler = new ExodusKeyHandler(this);
    }
}
