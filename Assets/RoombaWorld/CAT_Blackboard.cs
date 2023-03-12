using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAT_Blackboard : MonoBehaviour
{
    public float roombaDetectionRadius = 50f;
    public float roombaReachRadius = 3f;
    public GameObject roomba;
    public float m_CurrentTime = 0f;
    public float m_RoombaTime = 7f;
    public float m_RoombaRestTime = 15f;
    // Start is called before the first frame update
    void Awake()
    {
        roomba = GameObject.FindGameObjectWithTag("ROOMBA");
    }

    void Update()
    {
        m_CurrentTime += Time.deltaTime;
    }
}
