using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationMapGenPainted : LocationAbstract, ILocation
{
    public Dictionary<string, float> qualities;
    public LocationMapGenPainted(Dictionary<string, float> _qualities)
    {
        qualities = _qualities;
    }
    public override Color GetColor()
    {
        string Category = "Regions";
        float value = qualities[Category];
        Color c = Colors.GetColorLerp(value, Color.blue, Color.green);
        if (Category == "Regions")
        {
            if (value == 0f)
            {
                c = Colors.DarkBlue;
            }
            else
            {
                c = Colors.staticColors[(int)value];
            }
        }
        mDebug.Log(Category + " value is " + value + " and color is " + c);
        return c;

    }
}

