using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSpawner : MonoBehaviour
{
    public GameObject Mouse;
    public float interval = 25f;
    private GameObject[] points;

    private float elapsedTime = 0;

    void Awake()
    {
        points = GameObject.FindGameObjectsWithTag("EXIT");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Mouse == null)
            Debug.LogError("No Mouse prefab found");
    }

    // Update is called once per frame
    void Update()
    {
        //Spawn a mouse every 25 seconds
        if (elapsedTime >= interval)
        {
            GameObject mouseClone = Instantiate(Mouse);
            mouseClone.transform.position = GetRandomLocation().transform.position;

            elapsedTime = 0;
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }

    //Get a random spawning location
    private GameObject GetRandomLocation()
    {
        return points[Random.Range(0, points.Length)];
    }
}
