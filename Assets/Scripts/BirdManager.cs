using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    public GameObject bird;
    public PlayerManager player;

    public List<GameObject> birdInstances;

    float spawnInterval = .5f; // Seconds between bird spawns
    float nextSpawn = 0;


    void Update()
    {
        if(Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnInterval;
            birdInstances.Add(Instantiate(bird, new Vector3(Random.Range(player.transform.position.x + 1000, player.transform.position.x + 1400), 
                Random.Range(250, player.transform.position.y + 600), Random.Range(player.transform.position.z + 300, player.transform.position.z - 800)), Quaternion.identity));
            birdInstances[birdInstances.Count - 1].transform.parent = this.transform;
            var random = Random.Range(0, 2);
            if(random == 0)
                birdInstances[birdInstances.Count - 1].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, Random.Range( 2000, 4000) * Time.deltaTime);
            else
            {
                birdInstances[birdInstances.Count - 1].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -Random.Range(2000, 4000) * Time.deltaTime);
                birdInstances[birdInstances.Count - 1].transform.eulerAngles = new Vector3(0, 0, 180);
            }

            birdInstances[birdInstances.Count - 1].name = "Bird" + birdInstances.Count;

        }
    }
}
