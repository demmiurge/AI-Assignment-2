using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Cat", menuName = "Finite State Machines/FSM_Cat", order = 1)]
public class FSM_Cat : FiniteStateMachine
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
        FiniteStateMachine CatRoomba = ScriptableObject.CreateInstance<FSM_CatRoomba>();

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

        Transition locationReached = new Transition("Location Reached",
           () => m_GoToTarget.routeTerminated(),
           () => { }
       );

        Transition TimeOut = new Transition("Time Out",
            () => { return m_CatBlackboard.m_CurrentTime >= m_CatBlackboard.m_RoombaRestTime; },
            () => { }
        );

            
        AddStates(CatRoomba, Wander);

        AddTransition(Wander, locationReached, Wander);
        AddTransition(Wander, TimeOut, CatRoomba);
         
        //initialState = ... 


    }
}
