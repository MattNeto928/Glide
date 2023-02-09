using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    public TerrainManager terrain;

    public PlayerManager player;
    public List<GameObject> rings; // Ring objects in the world

    public List<GameObject> ringTypes; // Different variations of ring Prefabs

    void Update()
    {
        checkRings();
    }

    void checkRings()
    {
        foreach (GameObject ring in rings)
        {
            

            if (player.transform.position.x * 4 < 1024)
                return;
            else
                try
                {
                    if (ring.name.Equals("Ring[" + (player.xTile + 4) + "]"))
                        return;
                }
                catch
                {

                }

            if(ring != null)
            if (ring.transform.position.x < PlayerManager.xLocation - 300)
            {
                Destroy(ring);
            }
        }


        createRings(player.xTile + 4);

    }

    void createRings(int spawnTile)
    {
        rings.Add(Instantiate(ringTypes[Random.Range(0, ringTypes.Count)], new Vector3(spawnTile * terrain.terrainSize, Random.Range(100, 600),
            Random.Range(player.transform.position.z - 800, player.transform.position.z + 800)), Quaternion.Euler(0, 90, 0)));
        rings[rings.Count - 1].name = "Ring[" + spawnTile + "]";
        rings[rings.Count - 1].transform.parent = transform;
    }
    
    public void destroyAllRings()
    {
        var count = rings.Count;
        for(int i = 0; i < count; i++)
        {
            Destroy(rings[i]);
        }
        
        rings = new List<GameObject>();
    }
}
