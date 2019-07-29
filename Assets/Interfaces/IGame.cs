using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGame
{
    TileShape tileShape { get; set; }
    IMap map { get; set; }
    int tick { get; set; }

    ILocationColor _locationColor { get; set; }
    ExodusBiomeSystem _biomeSystem { get; set; }

    bool HandleClick(Pos pos, bool leftClick, bool rightClick);
    bool HandleKeys(List<KeyCode> keysHeld, List<KeyCode> keysUp, List<KeyCode> keysDown);
    void TakeTurn();
}