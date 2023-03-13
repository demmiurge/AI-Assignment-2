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
       //FSM of normal roomba behaviour
        FiniteStateMachine Roomba = ScriptableObject.CreateInstance<FSM_Roomba>();

        //Go to the charging station
        State GoToStation = new State("Go To Station",
            () => { m_GoToTarget.enabled = true;
                m_Station = m_RoombaBlackboard.NearestStation();
                m_GoToTarget.target = m_Station;
            }, 
            () => { },
            () => { m_GoToTarget.enabled = false;}  
        );

        //Charging
        State Charging = new State("Chargning",
            () => { },
            () => { m_RoombaBlackboard.Recharge(Time.deltaTime);},
            () => { }
        );

        //If the charge is low
        Transition lowCharge = new Transition("Low Charge",
            () => { return m_RoombaBlackboard.currentCharge <= m_RoombaBlackboard.minCharge;}, 
            () => { }  
        );

        //If it's charged
        Transition Charged = new Transition("High Charge",
            () => { return m_RoombaBlackboard.currentCharge >= m_RoombaBlackboard.maxCharge; },
            () => { }
        );

        //If the station was reached
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
