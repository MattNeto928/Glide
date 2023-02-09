using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    Terrain terrain;

    ParticleSystem particles;
    public Transform particleTransform;

    public bool moving = false;

    public float speed = 150;
    public float amount = 1.25f;
    public float riseSpeed = 350;

    public float terrainHeight;

    public bool terrainFound = false;

    public float spawnHeight; 

    private void Awake()
    {
        terrainHeight = -1000;
    }

    private void Start()
    { 
        gameObject.name = gameObject.tag + "[" + (int)(transform.position.x / 512) + "][" + (int)(transform.position.z / 512) + "]";
        if (gameObject.tag.Equals("Tree"))
            spawnHeight = -255;
        else
            if (gameObject.tag.Equals("Rock"))
            spawnHeight = -400;

        particles = GetComponent<ParticleSystem>();

        if (gameObject.tag.Equals("Tree"))
            spawnHeight = -255;
        else
            if (gameObject.tag.Equals("Rock"))
            spawnHeight = -450;

        transform.parent = GameObject.Find("Obstacles").transform;

        transform.position = new Vector3(transform.position.x, spawnHeight, transform.position.z); //Starts initial y position 255 units below the height of the terrain at (x, z) of THIS
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    private void Update()
    {
        getTerrain();

        if (transform.position.y < terrainHeight - 65)
        {
            transform.position += new Vector3(Mathf.Sin(Time.time * speed) * amount * Time.deltaTime, riseSpeed * Time.deltaTime, Mathf.Sin(Time.time * speed) * amount * Time.deltaTime);
        }
        else
        {
            if (particles)
            {
                var main = particles.main;
                main.loop = false;
            }
        }

        if (transform.position.x < PlayerManager.xLocation - 200)
            gameObject.SetActive(false);

    }

    void getTerrain()
    {
        if (GameObject.Find("Terrain[" + (int)(transform.position.x / 512) + "][" + (int)(transform.position.z / 512) + "]") && !terrainFound)
        {
            terrain = GameObject.Find("Terrain[" + (int)(transform.position.x / 512) + "][" + (int)(transform.position.z / 512) + "]").GetComponent<Terrain>();
            particles = GetComponent<ParticleSystem>();

            if (terrain.terrainData)
            {
                terrainHeight = terrain.SampleHeight(transform.position);
                transform.position = new Vector3(transform.position.x, -255 - terrainHeight, transform.position.z); //Starts initial y position 255 units below the height of the terrain at (x, z) of THIS

                try
                {
                    particleTransform.position = new Vector3(transform.position.x, terrainHeight, transform.position.z);
                }
                catch
                {

                }
                terrainFound = true;
            }
        }


    }
}