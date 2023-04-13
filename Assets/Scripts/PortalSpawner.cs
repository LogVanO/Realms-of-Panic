using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    public GameObject portal;
    public double spawnRate = 90;
    private float timer = 0;
    public System.Random rnd = new System.Random();
    public AudioSource PortalSound;

    // Start is called before the first frame update
    void Start()
    {
        // hey
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            spawnPortal();
            timer = 0;
        }
    }

    void spawnPortal()
    {
        PortalSound.Play();
        int x = rnd.Next(1, 3);
        if (x == 2)
        {
            x = -1;
        }
        Instantiate(portal, new Vector3((transform.position.x + (8 * x)), transform.position.y, transform.position.z), transform.rotation);
    }
}