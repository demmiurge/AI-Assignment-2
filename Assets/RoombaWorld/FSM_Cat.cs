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

        State ReachMouse = new State("Reach Mouse",
          () => {
              m_GoToTarget.enabled = true;
              m_GoToTarget.target = m_CatBlackboard.roomba;
              m_Context.maxAcceleration *= 1.5f;
              m_Context.maxSpeed *= 1.5f;
          },
          () => { },
          () => {
              m_GoToTarget.enabled = false;
              m_Context.maxAcceleration /= 1.5f;
              m_Context.maxSpeed /= 1.5f;
          }
      );

        Transition locationReached = new Transition("Location Reached",
           () => m_GoToTarget.routeTerminated(),
           () => { }
       );

        Transition TimeOut = new Transition("Time Out",
            () => { return m_CatBlackboard.m_CurrentTime >= m_CatBlackboard.m_RoombaRestTime; },
            () => { }
        );

            
        AddStates(CatRoomba, ReachMouse);

        AddTransition(ReachMouse, locationReached, ReachMouse);
        AddTransition(ReachMouse, TimeOut, CatRoomba);
         
        //initialState = ... 


    }
}
