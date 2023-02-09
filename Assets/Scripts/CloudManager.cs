using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    public GameObject[] cloudPrefabs;
    public List<GameObject> cloudInstances;
    public PlayerManager player;
    public TerrainManager terrainManager;

    public int cloudSpeed;

    int random;

    private void Update()
    {
        checkCloudsAroundPlayer();
        moveClouds();

        foreach(GameObject cloud in cloudInstances)
        {
            if (cloud.transform.parent != GameObject.Find("Clouds").transform)
                cloud.transform.parent = GameObject.Find("Clouds").transform;
        }
    }

    // Called from TerrainMangager
    public void initialSpawn()
    {
        for (float y = 0; y < 4; y++)
        {
            for (float x = -5; x < 8; x += 2)
            {
                for (float z = -8; z < 8; z += 2)
                {

                    random = Random.Range(0, cloudPrefabs.Length);

                    cloudInstances.Add(Instantiate(cloudPrefabs[random],
                        new Vector3((x * terrainManager.terrainSize) + Random.Range(-300, 300), 1000 + (y * 750) + Random.Range(-300, 300), (z * terrainManager.terrainSize) + Random.Range(-300, 300)), Quaternion.identity));
                    cloudInstances[cloudInstances.Count-1].name = "Cloud" + (int)(cloudInstances[cloudInstances.Count-1].transform.position.z/terrainManager.terrainSize);
                }
            }
        }
    }

    void checkCloudsAroundPlayer()
    {
        checkCloudAhead();
        checkCloudLeft();
        checkCloudRight();

        destroyPastClouds();
    }

    void checkCloudLeft()
    {
        foreach (GameObject cloud in cloudInstances)
        {
            if (cloud.name == "Cloud" + (player.yTile + 8) || cloud.name == "Cloud" + (player.yTile + 7))
                return;
        }
        for (int y = 0; y < 4; y++)
        {
            for (int x = -5; x < 8; x += 2)
            {
                random = Random.Range(0, cloudPrefabs.Length);

                cloudInstances.Add(Instantiate(cloudPrefabs[random],
                        new Vector3(((player.xTile + x) * terrainManager.terrainSize) + Random.Range(-300, 300), 1000 + (y * 750) + Random.Range(-300, 300), ((player.yTile + 8) * terrainManager.terrainSize) + Random.Range(-300, 300)), Quaternion.identity));
                cloudInstances[cloudInstances.Count - 1].name = "Cloud" + (int)(cloudInstances[cloudInstances.Count - 1].transform.position.z / terrainManager.terrainSize);
            }
        }
    }

    void checkCloudRight()
    {
        foreach (GameObject cloud in cloudInstances)
        {
            if (cloud.name == "Cloud" + (player.yTile - 8) || cloud.name == "Cloud" + (player.yTile - 7))
                return;
        }
        for (int y = 0; y < 4; y++)
        {
            for (int x = -5; x < 8; x += 2)
            {
                random = Random.Range(0, cloudPrefabs.Length);

                cloudInstances.Add(Instantiate(cloudPrefabs[random],
                        new Vector3(((player.xTile + x) * terrainManager.terrainSize) + Random.Range(-300, 300), 1000 + (y * 750) + Random.Range(-300, 300), ((player.yTile - 8) * terrainManager.terrainSize) + Random.Range(-300, 300)), Quaternion.identity));
                cloudInstances[cloudInstances.Count - 1].name = "Cloud" + (int)(cloudInstances[cloudInstances.Count - 1].transform.position.z / terrainManager.terrainSize);
            }
        }
    }

    void checkCloudAhead()
    {

        var totalCloudsAhead = 0;

        foreach (GameObject cloud in cloudInstances)
        {
            if ((cloud.transform.position.x > (player.xTile + 8) * terrainManager.terrainSize) || (cloud.transform.position.x > (player.xTile + 7) * terrainManager.terrainSize))
            {
                totalCloudsAhead++;
            }
            if (totalCloudsAhead >= 15)
                return;
        }

        for (int z = -8; z < 8; z += 2)
        {
            for (int y = 0; y < 4; y++)
            {
                random = Random.Range(0, cloudPrefabs.Length);

                cloudInstances.Add(Instantiate(cloudPrefabs[random],
                    new Vector3(((player.xTile + 8) * terrainManager.terrainSize) + Random.Range(-300, 300), 1000 + (y * 750) + Random.Range(-300, 300), ((z + player.yTile) * terrainManager.terrainSize) + Random.Range(-300, 300)), Quaternion.identity));
                cloudInstances[cloudInstances.Count - 1].name = "Cloud" + (int)(cloudInstances[cloudInstances.Count - 1].transform.position.z / terrainManager.terrainSize);
            }
        }
    }

    void destroyPastClouds()
    {

        for (int i = 0; i < cloudInstances.Count; i++)
        {
            if ((cloudInstances[i].transform.position.x < ((player.xTile - 5) * terrainManager.terrainSize))
                || cloudInstances[i].transform.position.z > (player.yTile + 10) * terrainManager.terrainSize
                || cloudInstances[i].transform.position.z < (player.yTile - 10) * terrainManager.terrainSize)
            {
                var temp = cloudInstances[i];
                cloudInstances.Remove(cloudInstances[i]);
                Destroy(temp);
            }
        }

    }

    public void destroyAllClouds()
    {
        while (cloudInstances.Count > 0)
        {
            var temp = cloudInstances[cloudInstances.Count - 1];
            cloudInstances.Remove(temp);
            Destroy(temp);
        }
    }
    void moveClouds()
    {
        foreach(GameObject cloud in cloudInstances)
        {
            cloud.transform.position += Vector3.back * cloudSpeed * Time.deltaTime;
            cloud.name = "Cloud" + (int)(cloud.transform.position.z / terrainManager.terrainSize);
        }
    }


}

