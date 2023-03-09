using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustSpawner : MonoBehaviour
{
    public GameObject Dust;
    public float interval = 5f;
    private GameObject[] points;

    private float elapsedTime = 0;

    void Awake()
    {
        points = GameObject.FindGameObjectsWithTag("EXIT");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Dust == null)
            Debug.LogError("No Dust prefab found");
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsedTime >= interval)
        {
            GameObject dustClone = Instantiate(Dust);
            dustClone.transform.position = RandomLocationGenerator.RandomWalkableLocation();
            dustClone.GetComponent<SpriteRenderer>().color = Random.ColorHSV();

            elapsedTime = 0;
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
