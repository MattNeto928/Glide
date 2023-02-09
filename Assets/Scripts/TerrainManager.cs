using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public PlayerManager player;
    public GameObject terrain;
    public CloudManager cloudManager;

    public float terrainSize;
    public int tileX, tileZ;

    public List<GameObject> allTerrains;

    public TerrainData terrainDataGrass;
    public TerrainLayer[] grassLayer;
    public TerrainLayer[] rockLayer;
    public TerrainLayer[] sandLayer;
    public TerrainLayer[] dirtLayer;
    
    private void Start()
    {
        terrainSize = terrain.GetComponent<GenerateTerrain>().width; // Getting terrainSize from terrain prefab
        cloudManager.initialSpawn();
        allTerrains = new List<GameObject>();

    }

    private void Update()
    {
        StartCoroutine(instantiateTerrain());

    }

    // Coroutine for instantiating terrain to reduce lag
    IEnumerator instantiateTerrain()
    {
        for (int x = -1; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (!hasTerrain(player.xTile + x, player.yTile + y))
                {
                    allTerrains.Add(Instantiate(terrain, new Vector3(terrainSize * (x + player.xTile),
                        -60, terrainSize * (y + player.yTile)), Quaternion.identity)); //Instantiating terriain/adding to list
                    allTerrains[allTerrains.Count - 1].name = "Terrain[" + (player.xTile + x) + "][" + (player.yTile + y) + "]"; //Changing name of terrain to coordinate
                    allTerrains[allTerrains.Count - 1].transform.parent = gameObject.transform; //Setting terrain object to child of Terrain Manager

                }
                if (y % 2 == 0)
                    yield return null;
            }
        }

    }

    public void resetTiles()
    {
        while (allTerrains.Count > 45)
        {
            Destroy(allTerrains[allTerrains.Count - 1]);
            allTerrains.RemoveAt(allTerrains.Count - 1);

        }
    }

    bool hasTerrain(int x, int y)
    {

        foreach (GameObject g in allTerrains)
        {
            if (g.name.Equals("Terrain[" + x + "][" + y + "]"))
                return true;
        }

        return false;
    }

}