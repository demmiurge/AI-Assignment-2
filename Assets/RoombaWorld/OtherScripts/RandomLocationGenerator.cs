using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class RandomLocationGenerator  {

    private static List<GraphNode> allNodes;
    private static List<GameObject> patrolPoints;
    
    static RandomLocationGenerator ()
    {
        // get all the nodes in the gridgraph and save the walkable ones in allNodes list.
        allNodes = new List<GraphNode>();
        GridGraph gg = AstarPath.active.data.gridGraph;
        gg.GetNodes(nod => { if (nod.Walkable) allNodes.Add(nod); });

        // get all the patrol points
        patrolPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("PATROLPOINT"));
    } 

    public static Vector3 RandomWalkableLocation ()
    {
        GraphNode node = allNodes[Random.Range(0, allNodes.Count)];
        // return its position as a vector 3
        return (Vector3)node.position;
    }

    public static Vector3 RandomPatrolLocation ()
    {
        return patrolPoints[Random.Range(0, patrolPoints.Count)].transform.position;
    }

}
