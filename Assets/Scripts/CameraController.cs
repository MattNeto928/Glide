using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager playerManager;
    Vector3 playerPos;

    private void Update()
    {
        playerPos = playerManager.transform.position;

        if(playerPos.x < 280)
        {
            transform.position = new Vector3(97.6f, 151.1f, 274.2f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(playerPos.x + 90, playerPos.y, playerPos.z) - transform.position), 10 * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(playerPos.x - 15, playerPos.y + 5, playerPos.z);
            transform.eulerAngles = new Vector3(5, 90, 0);
        }
    }
}
