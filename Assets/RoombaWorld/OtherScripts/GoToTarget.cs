using Pathfinding;
using Steerings;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(PathFollowing))]
public class GoToTarget : MonoBehaviour
{
    public GameObject target;    // the required destination

    private Seeker seeker;
    private PathFollowing pathFollowingSteering;
    private Path currentPath;

    private GameObject lastTarget = null;
    
    void Start()
    {
        seeker = GetComponent<Seeker>();
        pathFollowingSteering = GetComponent<PathFollowing>();
        pathFollowingSteering.enabled = false; // can't be enabled until a path is available
    }

    

    void Update()
    {
        if (target == null) return;
        if (target!=lastTarget)
        {
            lastTarget = target;
            pathFollowingSteering.enabled = false;
            // start the path computation process
            currentPath = null;
            seeker.StartPath(this.transform.position, target.transform.position, OnPathComplete);
            return;
        }
        if (currentPath != null &&
            pathFollowingSteering.currentWaypointIndex == currentPath.vectorPath.Count)
        {
            target = null; 
        }

    }

    public void OnPathComplete(Path p)
    {
        // this is a "callback" method. if this method is called, a path has been computed and "stored" in p
        currentPath = p;

        // feed the path to the path-following steering
        pathFollowingSteering.path = currentPath;
        pathFollowingSteering.currentWaypointIndex = 0;
        pathFollowingSteering.enabled = true;
    }


    public bool routeTerminated()
    {
        // this method should only be invoked if the behaviour has been enabled
        // and a target has been set (first set target, then enable) 
        return target == null;
    }

    private void OnEnable()
    {
        lastTarget = null;
    }

    private void OnDisable()
    {
        pathFollowingSteering.enabled = false;
    }
}
