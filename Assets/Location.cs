using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocation
{
    Color GetColor();
    void TakeTurn();
    void Click();
}


public class LocationManualBiome : ILocation
{
    public Dictionary<string, float> qualities;
    public Color color;
    Pos pos;

    float _numCats;

    public LocationManualBiome(Pos _pos, Dictionary<string, float> _qualities, Color[] colorList, float numCats)
    {
        pos = _pos;
        qualities = _qualities;
        _numCats = numCats;

        SetColor(colorList);
    }

    void SetColor(Color[] colorList)
    {
        //SetColorManual1();
        //SetColorManual2(colorList);
        SetColorAutomatic1(colorList, _numCats);
        //SetColorAutomatic2(colorList, 2f);

    }

    void SetColorAutomatic1(Color[] colorList, float numCats)
    {
        bool _debugThis = false;
        if (qualities["Land"] == 0f)
        {
            color = Color.black;
        }
        else
        {
            color = Color.white;
            float water = (qualities["WaterFlux"] + qualities["Rain"]) / 2f;
            float temperature = qualities["Temperature"];
            mDebug.Log("Water(" + water + ") & Temperature(" + temperature + ")", _debugThis);
            for (float w = 0; w < numCats; w++)
            {
                for (float t = 0; t < numCats; t++)
                {
                    int k = (int)(w * numCats + t + 1f);
                    float wk = w / numCats;
                    float tk = t / numCats;
                    mDebug.Log("Water(" + water + ") > " + wk + " && Temperature(" + temperature + ") > " + tk + " ? ", _debugThis);
                    if (water >= wk && temperature >= tk)
                    {
                        color = colorList[k];
                        mDebug.Log("Yes, k = " + k.ToString(), _debugThis);
                    }
                    else
                    {
                        mDebug.Log("No", _debugThis);
                    }
                }
            }
        }
    }
    void SetColorAutomatic2(Color[] colorList, float numCats)
    {
        if (qualities["Land"] == 0f)
        {
            color = Color.black;
        }
        else
        {
            color = Color.white;
            float water = (qualities["WaterFlux"] + qualities["Rain"]) / 2f;
            float temperature = qualities["Temperature"];
            float longitude = pos.gridLoc.x() / (float)pos.game.map.xDim;
            float latitude = pos.gridLoc.y() / (float)pos.game.map.yDim;
            mDebug.Log("Water(" + water + ") & Temperature(" + temperature + ")", false);

            int kk = 0;
            for (float w = 0; w < numCats; w++)
            {
                for (float t = 0; t < numCats; t++)
                {
                    for (float lat = 0; lat < numCats; lat++)
                    {
                        for (float lng = 0; lng < numCats; lng++)
                        {
                            kk++;
                            float wkk = w / numCats;
                            float tkk = t / numCats;
                            float latk = lat / numCats;
                            float lngk = lng / numCats;
                            if (water >= wkk && temperature >= tkk && latitude >= latk && longitude >= lngk)
                            {
                                color = colorList[kk];
                            }
                        }
                    }
                }
            }
        }
    }

    void SetColorManual1()
    {
        float water = qualities["WaterFlux"];
        float temperature = qualities["Temperature"];

        if (qualities["Land"] == 0f)
        {
            color = Colors.OceanBlue;
        }
        else
        {
            if (water > 0.5f)
            {
                if (temperature > 0.5f)
                {
                    color = Color.green;
                }
                else
                {
                    color = Color.blue;
                }
            }
            else
            {
                if (temperature > 0.5f)
                {
                    color = Color.red;
                }
                else
                {
                    color = Color.yellow;
                }
            }
        }
    }
    void SetColorManual2(Color[] colorList)
    {
        //float water = qualities["WaterFlux"];
        float water = (qualities["WaterFlux"] + qualities["Rain"]) / 2f;
        float temperature = qualities["Temperature"];

        if (qualities["Land"] == 0f)
        {
            //color = Colors.OceanBlue;
            color = Color.black;
        }
        else
        {
            if (water < 0.25f)
            {
                if (temperature < 0.25f)
                {
                    color = colorList[1];
                }
                else if (temperature < 0.5f)
                {
                    color = colorList[2];
                }
                else if (temperature < 0.75f)
                {
                    color = colorList[3];
                }
                else
                {
                    color = colorList[4];
                }
            }
            else if (water < 0.5f)
            {
                if (temperature < 0.25f)
                {
                    color = colorList[5];
                }
                else if (temperature < 0.5f)
                {
                    color = colorList[6];
                }
                else if (temperature < 0.75f)
                {
                    color = colorList[7];
                }
                else
                {
                    color = colorList[8];
                }
            }
            else if (water < 0.75f)
            {
                if (temperature < 0.25f)
                {
                    color = colorList[9];
                }
                else if (temperature < 0.5f)
                {
                    color = colorList[10];
                }
                else if (temperature < 0.75f)
                {
                    color = colorList[11];
                }
                else
                {
                    color = colorList[12];
                }
            }
            else
            {
                if (temperature < 0.25f)
                {
                    color = colorList[13];
                }
                else if (temperature < 0.5f)
                {
                    color = colorList[14];
                }
                else if (temperature < 0.75f)
                {
                    color = colorList[15];
                }
                else
                {
                    color = colorList[16];
                }
            }
        }
    }
    public Color GetColor()
    {
        return color;

    }
    public void TakeTurn()
    {

    }
    public void Click()
    {
        mDebug.Log("Color=" + color.ToString() + ";Temperature=" + qualities["Temperature"] + ";Rain=" + qualities["Rain"] + ";WaterFlux=" + qualities["WaterFlux"]);
    }
}

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

public abstract class LocationMapGen : ILocation
{
    public abstract Color GetColor();
    public void TakeTurn()
    {

    }
    public void Click()
    {

    }
}

public class LocationMapGenPainted : LocationMapGen, ILocation
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

public class LocationMapGenBiomeColor : LocationMapGen, ILocation
{
    Color color;
    public LocationMapGenBiomeColor(Color c)
    {
        color = c;
    }
    public override Color GetColor()
    {
        return color;

    }

}
