
using UnityEngine;
using System.Collections;

public class GenerateTerrain : MonoBehaviour
{
    public int depth = 20;

    public int width = 512;
    public int height = 512;

    public float scale = 20f;

    public float offsetX = 100f, offsetY = 100f;
    public bool[,] holes;

    public int xCoord, yCoord;

    public int xTile = 0, yTile = 0;

    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public TerrainManager terrainManager;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        terrainManager = GameObject.Find("Terrain Manager").GetComponent<TerrainManager>();

        width = (int)terrainManager.terrainSize;
        height = (int)terrainManager.terrainSize;

        xTile = (int)(transform.position.x / width);
        yTile = (int)(transform.position.z / height);

        offsetX = gameManager.offsetX + (yTile * scale); //The order of x and y is reversed since the terrain is rotated relative to the world (I think)
        offsetY = gameManager.offsetY + (xTile * scale);

        TerrainData terrainData = new TerrainData();
        terrainData.name = "TerrainData[" + (int)(transform.position.x / width) + "][" + (int)(transform.position.z / height) + "]";

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = terrainData;
        GetComponent<TerrainCollider>().terrainData = terrainData;

        terrain.terrainData = TerrainGenerator(terrain.terrainData);

        if (transform.position.x < 1024)
            terrain.terrainData.terrainLayers = terrainManager.grassLayer;
        else
            if (transform.position.x >= 1024 && transform.position.x < 4096)
            terrain.terrainData.terrainLayers = terrainManager.rockLayer;
        else
            terrain.terrainData.terrainLayers = terrainManager.sandLayer;

    }

    private void Update()
    {
        if(terrainManager.allTerrains.IndexOf(gameObject) > 45 && transform.position.x < PlayerManager.xLocation - 2000)
            gameObject.SetActive(false);
    }

    public TerrainData TerrainGenerator(TerrainData terrainData)
        {
            terrainData.heightmapResolution = width + 1;
            terrainData.size = new Vector3(width, depth, height);
            StartCoroutine(GenerateHeights());
            //terrainData.SetHeights(0, 0, GenerateHeights());

            return terrainData;
        }

        float CalculateHeight(int x, int y)
        {
            float xCoord = (float)x / width * scale + offsetX;
            float yCoord = (float)y / height * scale + offsetY;

            return Mathf.PerlinNoise(xCoord, yCoord);
        }

    IEnumerator GenerateHeights()
    {
        float[,] heights = new float[width + 1, height + 1];
        for (int x = 0; x <= width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }

            if (x % 32 == 31)
                yield return null;

        }

        GetComponent<Terrain>().terrainData.SetHeights(0, 0, heights);
        GetComponent<Terrain>().enabled = true;
    }
}
