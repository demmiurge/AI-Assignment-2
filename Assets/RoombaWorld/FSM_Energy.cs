using System;
using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Energy", menuName = "Finite State Machines/FSM_Energy", order = 1)]
public class FSM_Energy : FiniteStateMachine
{
    private ROOMBA_Blackboard m_RoombaBlackboard;
    private GoToTarget m_GoToTarget;
    private float m_ElapsedTime = 0;
    private GameObject m_Station;

    public override void OnEnter()
    {
        m_RoombaBlackboard = GetComponent<ROOMBA_Blackboard>();
        m_GoToTarget = GetComponent<GoToTarget>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------*/

        FiniteStateMachine Roomba = ScriptableObject.CreateInstance<FSM_Roomba>();

        State GoToStation = new State("Go To Station",
            () => { m_GoToTarget.enabled = true;
                m_Station = m_RoombaBlackboard.NearestStation();
                m_GoToTarget.target = m_Station;
            }, 
            () => { },
            () => { m_GoToTarget.enabled = false;}  
        );

        State Charging = new State("Chargning",
            () => { },
            () => { m_RoombaBlackboard.Recharge(Time.deltaTime);},
            () => { }
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------*/

        Transition lowCharge = new Transition("Low Charge",
            () => { return m_RoombaBlackboard.currentCharge <= m_RoombaBlackboard.minCharge;}, 
            () => { }  
        );

        Transition Charged = new Transition("High Charge",
            () => { return m_RoombaBlackboard.currentCharge >= m_RoombaBlackboard.maxCharge; },
            () => { }
        );

        Transition locationReached = new Transition("Location Reached",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_Station) <= m_RoombaBlackboard.chargingStationReachedRadius; },
            () => { }
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------*/

        AddStates(Roomba, GoToStation, Charging);

        AddTransition(Roomba, lowCharge, GoToStation);
        AddTransition(GoToStation, locationReached, Charging);
        AddTransition(Charging, Charged, Roomba);


        /* STAGE 4: set the initial state*/

        initialState = Roomba;

    }
}
