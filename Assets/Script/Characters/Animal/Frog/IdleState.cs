using UnityEngine;

namespace AIStateMachine.Animal.Frog
{
    public class IdleState : AnimalBaseState
    {
        private bool goPatrol;
        private float idleTimer, idleTimeInterval;

        public IdleState(AnimalStateMachine stateMachine, Transform player, float idleTimer) : base(stateMachine, player, AnimalState.Idle)
        {
            this.idleTimer = idleTimer;
        }

        public override void EnterState()
        {
            goPatrol = false;
            stateMachine.PlayAnimation("Idle_A");
        }

        public override void ExitState()
        {
        }

        public override AnimalState GetNextState()
        {
            if (goPatrol)
                return AnimalState.Patrol;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
            idleTimeInterval += Time.deltaTime;
            if (idleTimeInterval < idleTimer) return;

            int x = Random.Range(0, 2);
            RandomIdleState(x);

            idleTimeInterval = 0;
        }

        private void RandomIdleState(int x)
        {
            switch (x)
            {
                case 0:
                    goPatrol = true;
                    break;
                case 1:
                    stateMachine.PlayAnimation("Idle_A");
                    break;
                default:
                    break;
            }
        }
    }
}