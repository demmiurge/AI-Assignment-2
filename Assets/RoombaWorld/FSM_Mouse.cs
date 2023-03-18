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
        temp = new GameObject();
        temp.transform.position = RandomLocationGenerator.RandomWalkableLocation();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        //Go to a location to poo
        State Wander = new State("Wander",
            () => { m_GoToTarget.enabled = true;
                m_GoToTarget.target = temp;
            },
           () => { },
           () => { m_GoToTarget.enabled = false; Destroy(temp);}
       );

        //Basically poo
        State Poo = new State("Poo",
            () => { GameObject clone = Instantiate(m_MouseBlackboard.pooPrefab);
                clone.transform.position = gameObject.transform.position;
                m_ElapsedTime = 0;
            },
            () => { m_ElapsedTime += Time.deltaTime;},
            () =>  { }
        );

        //Leave the flat
        State Exit = new State("Exit",
            () => { m_GoToTarget.enabled = true;
                m_GoToTarget.target = m_MouseBlackboard.RandomExitPoint();
            },
            () => { },
            () =>  { }
        );

        //Flee roomba and leave the flat
        State Flee = new State("Flee",
            () => {
                m_GoToTarget.enabled = true;
                m_GoToTarget.target = m_MouseBlackboard.NearestExitPoint();
                m_Context.maxSpeed *= 2;
                m_Context.maxAcceleration *= 4;
                this.GetComponent<SpriteRenderer>().color = Color.green;
            },
            () => { },
            () => { }
        );

        //Destroy itself
        State Empty = new State("Empty",
            () => { Destroy(gameObject); },
            () => { },
            () => { }
        );

        //If location was reached
        Transition locationReached = new Transition("Location Reached",
            () => m_GoToTarget.routeTerminated(),
            () => { }
        );

        //If roomba was detected
        Transition roombaDetected = new Transition("Roomba Detected",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_MouseBlackboard.roomba) <= m_MouseBlackboard.roombaDetectionRadius; },
            () => { }
        );

        //If the cat was detected
        Transition catDetected = new Transition("Cat Detected",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_MouseBlackboard.cat) <= m_MouseBlackboard.catDetectionRadius; },
            () => { }
        );

        //Poo time
        Transition TimeOut = new Transition("Time Out",
            () => { return m_ElapsedTime >= m_MouseBlackboard.m_PooTime; },
            () => { }
        );


        AddStates(Wander, Poo, Exit, Flee, Empty);

        AddTransition(Wander, locationReached, Poo);
        AddTransition(Wander, roombaDetected, Flee);
        AddTransition(Wander, catDetected, Flee);
        AddTransition(Poo, roombaDetected, Flee);
        AddTransition(Poo, TimeOut, Exit);
        AddTransition(Exit, locationReached, Empty);
        AddTransition(Exit, roombaDetected, Flee);
        AddTransition(Exit, catDetected, Flee);
        AddTransition(Flee, locationReached, Empty);

        initialState = Wander;
    }
}
