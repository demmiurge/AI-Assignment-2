using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Mouse", menuName = "Finite State Machines/FSM_Mouse", order = 1)]
public class FSM_Mouse : FiniteStateMachine
{
    private MOUSE_Blackboard m_MouseBlackboard;
    private GoToTarget m_GoToTarget;
    private SteeringContext m_Context;
    private float m_ElapsedTime = 0;
    private GameObject temp;

    public override void OnEnter()
    {
        m_MouseBlackboard = GetComponent<MOUSE_Blackboard>();
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
            () => { m_GoToTarget.enabled = true;
                temp = new GameObject();
                temp.transform.position = RandomLocationGenerator.RandomWalkableLocation();
                m_GoToTarget.target = temp;
                Debug.Break();
            },
           () => { },
            () => { m_GoToTarget.enabled = false; Destroy(temp);}
            );
        State Poo = new State("Poo",
            () => { GameObject clone = Instantiate(m_MouseBlackboard.pooPrefab);
                clone.transform.position = gameObject.transform.position;
                m_ElapsedTime = 0;
            },
            () => { m_ElapsedTime += Time.deltaTime;},
            () =>  { }
        );

        State Exit = new State("Exit",
            () => { m_GoToTarget.enabled = true;
                m_GoToTarget.target = m_MouseBlackboard.RandomExitPoint();
            },
            () => { },
            () =>  { Destroy(gameObject); }
        );

        State Flee = new State("Flee",
            () => {
                m_GoToTarget.enabled = true;
                m_GoToTarget.target = m_MouseBlackboard.NearestExitPoint();
                m_Context.maxSpeed *= 2;
                m_Context.maxAcceleration *= 4;
            },
            () => { },
            () => { Destroy(gameObject); }
        );

        State Empty = new State("Empty",
            () => { },
            () => { },
            () => { }
        );

        Transition locationReached = new Transition("Location Reached",
            () => m_GoToTarget.routeTerminated(),
            () => { }
        );

        Transition roombaDetected = new Transition("Roomba Detected",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_MouseBlackboard.roomba) <= m_MouseBlackboard.roombaDetectionRadius; },
            () => { }
        );

        Transition TimeOut = new Transition("Time Out",
            () => { return m_ElapsedTime >= m_MouseBlackboard.m_PooTime; },
            () => { }
        );


        AddStates(Wander, Poo, Exit, Flee, Empty);

        AddTransition(Wander, locationReached, Poo);
        AddTransition(Wander, roombaDetected, Flee);
        AddTransition(Poo, roombaDetected, Flee);
        AddTransition(Poo, TimeOut, Exit);
        AddTransition(Exit, locationReached, Empty);

        initialState = Wander;
    }
}
