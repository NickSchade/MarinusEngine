using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyHandler
{
    bool HandleKeys(List<KeyCode> keysHeld, List<KeyCode> keysUp, List<KeyCode> keysDown);
}


