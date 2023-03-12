using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_CatRoomba", menuName = "Finite State Machines/FSM_CatRoomba", order = 1)]
public class FSM_CatRoomba : FiniteStateMachine
{
    private CAT_Blackboard m_CatBlackboard;
    private GoToTarget m_GoToTarget;
    private SteeringContext m_Context;
    private float m_ElapsedTime = 0;
    private GameObject temp;

    public override void OnEnter()
    {
        m_CatBlackboard = GetComponent<CAT_Blackboard>();
        m_GoToTarget = GetComponent<GoToTarget>();
        m_Context = GetComponent<SteeringContext>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State Wander = new State("Wander",
            () => {
                temp = new GameObject();
                m_GoToTarget.enabled = true;
                temp.transform.position = RandomLocationGenerator.RandomWalkableLocation();
                m_GoToTarget.target = temp;
            },
           () => { },
           () => { m_GoToTarget.enabled = false; Destroy(temp); }
       );

        State ReachRoomba = new State("Reach Roomba",
           () => {
               m_GoToTarget.enabled = true;
               m_GoToTarget.target = m_CatBlackboard.roomba;
               m_Context.maxAcceleration *= 1.5f;
               m_Context.maxSpeed *= 1.5f;
           },
           () => { },
           () => { m_GoToTarget.enabled = false;
               m_Context.maxAcceleration /= 1.5f;
               m_Context.maxSpeed /= 1.5f;
           }
       );

        State RideRoomba = new State("Ride Roomba",
           () => { gameObject.transform.parent = m_CatBlackboard.roomba.transform; m_ElapsedTime = 0; m_CatBlackboard.m_CurrentTime = 0f; },
           () => { m_ElapsedTime += Time.deltaTime; },
           () => { gameObject.transform.parent = null; }
       );

        Transition roombaReached = new Transition("Roomba Reached",
          () => { return SensingUtils.DistanceToTarget(gameObject, m_CatBlackboard.roomba) <= m_CatBlackboard.roombaReachRadius; },
          () => { }
      );

        Transition locationReached = new Transition("Location Reached",
           () => m_GoToTarget.routeTerminated(),
           () => { }
       );

        Transition roombaDetected = new Transition("Roomba Detected",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_CatBlackboard.roomba) <= m_CatBlackboard.roombaDetectionRadius 
                && m_CatBlackboard.m_CurrentTime >= m_CatBlackboard.m_RoombaRestTime; },
            () => { }
        );

        Transition TimeOut = new Transition("Time Out",
            () => { return m_ElapsedTime >= m_CatBlackboard.m_RoombaTime; },
            () => { }
        );


            
        AddStates(Wander, ReachRoomba, RideRoomba);

        AddTransition(Wander, locationReached, Wander);

        AddTransition(Wander, roombaDetected, ReachRoomba);
        AddTransition(ReachRoomba, roombaReached, RideRoomba);
        AddTransition(RideRoomba, TimeOut, Wander);


        initialState = Wander;

    }
}
