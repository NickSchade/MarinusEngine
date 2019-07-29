using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMap
{
    Dictionary<string, Pos> pathMap { get; set; }
    Dictionary<Pos, ILocation> lands { get; set; }
    Dictionary<Pos, IStructure> settlements { get; set; }
    int xDim { get; set; }
    int yDim { get; set; }
    bool wrapEastWest { get; set; }
    bool wrapNorthSouth { get; set; }
}
