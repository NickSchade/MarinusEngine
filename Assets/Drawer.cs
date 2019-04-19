using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour {

    public GameObject pfTileSquare;

    private GameObject pfTile;

    Dictionary<Pos, GameObject> goMap;

    public IGame game;

    bool initCamera;

    float tileSpread = 1.0f;

	// Use this for initialization
	void Start () {
        initCamera = false;
        goMap = new Dictionary<Pos, GameObject>();
        pfTile = pfTileSquare;
	}

    public void DrawMap()
    {
        if (game.map.pathMap != null)
        {
            foreach (string k in game.map.pathMap.Keys)
            {
                Pos p = game.map.pathMap[k];
                if (!goMap.ContainsKey(p))
                {
                    goMap[p] = InstantiateGo(pfTile, p.mapLoc, game.map.lands[game.map.pathMap[p.mapLoc.key()]].GetColor());
                    goMap[p].GetComponentInChildren<Clickable>().setPos(game.map.pathMap[k]);
                }
            }
        }
    }

    GameObject InstantiateGo(GameObject pf, Loc l, Color c)
    {
        Vector3 pos = new Vector3(l.x() * tileSpread, l.z(), l.y() * tileSpread);
        GameObject go = Instantiate(pf, pos, Quaternion.identity);
        go.GetComponentInChildren<Renderer>().material.color = c;
        return go;
    }

    public void InitCamera()
    {
        if (!initCamera)
        {
            mDebug.Log("Initializing Camera...", false);
            initCamera = true;
            int xd = game.map.xDim;
            int yd = game.map.yDim;
            int d = Mathf.Min(xd, yd);

            Loc MidLoc = new Loc(d / 2 - 1, d / 2 - 1);
            mDebug.Log("Mid Loc is " + MidLoc.key(), false);
            Vector3 p = goMap[game.map.pathMap[MidLoc.key()]].transform.position;
            Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
            Camera.main.orthographic = true;
            if (game.tileShape == TileShape.SQUARE)
            {
                Camera.main.transform.position = new Vector3(p.x + tileSpread, 1, p.z + tileSpread);
                Camera.main.orthographicSize = tileSpread * d / 1.75f;
            }
            else if (game.tileShape == TileShape.HEX)
            {
                Camera.main.transform.position = new Vector3(p.x * 3 / 2, 1, p.z * 3 / 2);
                Camera.main.orthographicSize = (3 / 2) * d / 1.75f;
            }
        }
        
    }



    // Update is called once per frame
    void Update () {
		
	}
}
