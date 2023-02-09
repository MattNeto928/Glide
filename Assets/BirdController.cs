using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    bool jetpackOn = false;
    bool changed = false;
    public GameObject jetpack;
    public ParticleSystem smokeParticle, flamesParticle;
    public float speed;

    Rigidbody rb;

    void Start()
    {
        jetpack.SetActive(false);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(jetpackOn && !changed)
        {
            jetpack.SetActive(true);
            changed = true;
        }
    }

    public void turnOnJetpack()
    {
        jetpackOn = true;
        Debug.Log("BIRD ON");
        rb.AddForce(new Vector3(0, speed, 0), ForceMode.Impulse);
        transform.eulerAngles = new Vector3(-90, 0, 0);
        smokeParticle.Play();
        flamesParticle.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            //other.gameObject.GetComponent<PlayerManager>().emptyJetpack();
            Debug.Log("PLAYER HIT");
        }
    }

}
