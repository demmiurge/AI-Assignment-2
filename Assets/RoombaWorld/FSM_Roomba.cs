using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Roomba", menuName = "Finite State Machines/FSM_Roomba", order = 1)]
public class FSM_Roomba : FiniteStateMachine
{
    private ROOMBA_Blackboard m_RoombaBlackboard;
    private GoToTarget m_GoToTarget;
    private SteeringContext m_Context;
    private float m_ElapsedTime = 0;
    private GameObject m_Poo;
    private GameObject m_Dust;
    private GameObject m_OtherPoo;
    private GameObject m_OtherDust;
    private GameObject m_Patrol;

    public override void OnEnter()
    {
        m_RoombaBlackboard = GetComponent<ROOMBA_Blackboard>();  
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

        State Patrol = new State("Patrolling",
            () =>
            {
                m_Patrol = new GameObject();
                m_GoToTarget.enabled = true;
                m_Patrol.transform.position = RandomLocationGenerator.RandomPatrolLocation();
                m_GoToTarget.target = m_Patrol;
            }, 
            () => { }, 
            () => { m_GoToTarget.enabled = false; Destroy(m_Patrol); }   
        );

        State ReachPoo = new State("Reach Poo",
            () => {
                m_GoToTarget.enabled = true;
                m_GoToTarget.target = m_Poo;
                m_Context.maxSpeed *= 1.3f;
                m_Context.maxAcceleration *= 2.6f;
                m_ElapsedTime = 0;
            },
            () => { },
            () => {
                m_GoToTarget.enabled = false;
                m_Context.maxSpeed /= 1.3f;
                m_Context.maxAcceleration /= 2.6f;
            }
        );

        State CleanPoo = new State("Clean Poo",
            () => { m_ElapsedTime = 0; },
            () => { m_ElapsedTime += Time.deltaTime;},
            () => { Destroy(m_Poo); }
        );

        State ReachDust = new State("Reach Dust",
            () => {
                m_GoToTarget.enabled = true;
                m_GoToTarget.target = m_Dust;
                m_ElapsedTime = 0;
            },
            () => { },
            () =>  { m_GoToTarget.enabled = false; }
        );

        State CleanDust= new State("Clean Dust",
            () => { m_ElapsedTime = 0; },
            () => { m_ElapsedTime += Time.deltaTime; },
            () => { Destroy(m_Dust); }
        );

        Transition locationReached = new Transition("Location Reached",
            () => m_GoToTarget.routeTerminated(),
            () => { }
        );

        Transition pooDetected = new Transition("Poo Detected",
            () =>
            {
                m_Poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", m_RoombaBlackboard.pooDetectionRadius);
                return m_Poo != null;
            }, 
            () => { } 
        );

        Transition dustDetected = new Transition("Dust Detected",
            () =>
            {
                m_Dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", m_RoombaBlackboard.dustDetectionRadius);
                return m_Dust != null;
            },
            () => { }
        );

        Transition pooReached = new Transition("Poo Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, m_Poo) <= m_RoombaBlackboard.pooReachedRadius;
            },
            () => { }
        );

        Transition dustReached = new Transition("Dust Reached",
            () =>
            {
                return SensingUtils.DistanceToTarget(gameObject, m_Dust) <= m_RoombaBlackboard.dustReachedRadius;
            },
            () => { }
        );

        Transition dustDetectedRemember = new Transition("Dust Detected Remember",
            () =>
            {
                m_Dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", m_RoombaBlackboard.dustDetectionRadius);
                return m_Dust != null;
            },
            () => { m_RoombaBlackboard.AddToMemory(m_Dust);}
        );

        Transition pooDetectedRemember = new Transition("Dust Detected Remember",
            () =>
            {
                m_Poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", m_RoombaBlackboard.pooDetectionRadius);
                return m_Poo != null;
            },
            () => { m_RoombaBlackboard.AddToMemory(m_Dust); }
        );

        Transition pooCloser = new Transition("Poo Closer",
            () =>
            {
                m_OtherPoo = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", m_RoombaBlackboard.dustDetectionRadius);
                return m_OtherPoo != null && SensingUtils.DistanceToTarget(gameObject, m_OtherPoo) <
                    SensingUtils.DistanceToTarget(gameObject, m_Poo);
            },
            () => { m_Poo = m_OtherPoo;}
        );

        Transition Cleaned = new Transition("Cleaned",
            () => { return m_ElapsedTime >= m_RoombaBlackboard.cleanTime;},
            () => { }
        );

        Transition Remembered = new Transition("Remembered",
            () => { return m_RoombaBlackboard.memory.Count > 0; },
            () => { }
        );

            
        AddStates(Patrol, CleanPoo, CleanDust, ReachPoo, ReachDust);

        AddTransition(Patrol, locationReached, Patrol);
        AddTransition(Patrol, pooDetected, ReachPoo);
        AddTransition(CleanPoo, pooCloser, ReachPoo);
        AddTransition(ReachPoo, dustDetectedRemember, ReachPoo);
        AddTransition(ReachPoo, pooReached, CleanPoo);
        AddTransition(CleanPoo, Remembered, ReachDust);
        AddTransition(CleanPoo, Cleaned, Patrol);

        AddTransition(Patrol, dustDetected, ReachDust);
        AddTransition(ReachDust, pooDetectedRemember, ReachPoo);
        AddTransition(ReachDust, dustReached, CleanDust);
        AddTransition(CleanDust, Cleaned, Patrol);

        initialState = Patrol;


    }
}
