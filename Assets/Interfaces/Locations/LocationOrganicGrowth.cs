using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationOrganicGrowth : ILocation
{
    float resourceValue;
    bool isLand;
    float maxResource = 1f;
    float resourcePerTurn = 0.0001f;
    float resourcePow = 0.99f;//0.9999f;//0.99999f;
    bool isVisible = true;
    IGame game;
    public LocationOrganicGrowth(IGame _game, bool _isLand, float _startingResourceValue)
    {
        game = _game;
        isLand = _isLand;
        resourceValue = _startingResourceValue;
    }
    public void TakeTurn()
    {
        if (resourceValue < maxResource)
        {
            //resourceValue += resourcePerTurn;
            resourceValue = Mathf.Pow(resourceValue, resourcePow);
        }
        else if (resourceValue > maxResource)
        {
            resourceValue = maxResource;
        }
    }
    public Color GetColor()
    {
        if (isVisible)
        {
            if (isLand)
            {
                return Colors.OceanBlue;
            }
            else
            {
                mDebug.Log("resourceValue = " + resourceValue, false);
                //float x = Mathf.Round(resourceValue * 10) / 10f;
                float x = resourceValue;
                return Colors.GetColorLerp(x / maxResource, Color.white, Color.black);
            }
        }
        else
        {
            return Color.black;
        }
    }

    public void Click()
    {
        isVisible = !isVisible;
    }
}
