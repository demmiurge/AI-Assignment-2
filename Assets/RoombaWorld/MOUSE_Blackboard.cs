﻿using UnityEngine;
using Steerings;
using Pathfinding;

public class MOUSE_Blackboard : MonoBehaviour
{
    private GameObject[] exitPoints;
    public GameObject pooPrefab;
    public float roombaDetectionRadius = 50;
    public float catDetectionRadius = 60;
    public GameObject roomba;
    public GameObject cat;
    public float m_PooTime = 0.5f;

    void Awake()
    {
        // let's get all the exit&entry points
        exitPoints = GameObject.FindGameObjectsWithTag("EXIT");
        pooPrefab = Resources.Load<GameObject>("POO");
        roomba = GameObject.FindGameObjectWithTag("ROOMBA");
        cat = GameObject.FindGameObjectWithTag("CAT");
    }

    public GameObject RandomExitPoint()
    {
        return exitPoints[Random.Range(0,exitPoints.Length)];   
    }

    public GameObject NearestExitPoint ()
    {
        GameObject nearest = exitPoints[0];
        float best = SensingUtils.DistanceToTarget(gameObject, nearest);
        float current;
        // process all exit points. Retain the nearest
        for (int i=1; i<exitPoints.Length; i++)
        {
            current = SensingUtils.DistanceToTarget(gameObject, exitPoints[i]);
            if (current<best)
            {
                best = current;
                nearest = exitPoints[i];
            }
        }

        return nearest;
    }



}
