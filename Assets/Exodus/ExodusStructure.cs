using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExodusStructure : IStructure
{
    ExodusLocation _location;
    public ExodusStructure(ExodusLocation location)
    {
        _location = location;
        Debug.Log("Made New Structure");
    }
}

