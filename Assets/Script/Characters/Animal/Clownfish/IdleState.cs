using UnityEngine;

namespace AIStateMachine.Animal.Clownfish
{
    public class IdleState : AnimalBaseState
    {
        private bool goPatrol;

        public IdleState(AnimalStateMachine stateMachine, Transform player) : base(stateMachine, player, AnimalState.Idle)
        {
        }

        public override void EnterState()
        {
            stateMachine.PlayAnimation("Swim");
        }

        public override void ExitState()
        {
            goPatrol = false;
        }

        public override AnimalState GetNextState()
        {
            if (stateMachine.playerInSight)
                return AnimalState.Runaway;

            if (goPatrol)
                return AnimalState.Patrol;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
            goPatrol = !stateMachine.playerInSight;
        }
    }
}