using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSpawner : MonoBehaviour
{
    public GameObject Mouse;
    public float interval = 55f;
    public GameObject points;

    private float elapsedTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(Mouse == null)
            Debug.LogError("No Mouse prefab found");
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsedTime >= interval)
        {
            GameObject mouseClone = Instantiate(Mouse);
            //mouseClone.transform.position = 
        }
    }
}
