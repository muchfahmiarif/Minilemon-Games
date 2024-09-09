using UnityEngine;

namespace AIStateMachine.Animal.Hare
{
    public class IdleState : AnimalBaseState
    {
        private float idleTimer, idleTimeInterval;
        private string[] randomIdleAnim;
        private bool goPatrol;

        public IdleState(AnimalStateMachine stateMachine, Transform player, float idleTimer, string[] randomAnim) : base(stateMachine, player, AnimalState.Idle)
        {
            this.idleTimer = idleTimer;
            randomIdleAnim = randomAnim;
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
            if (stateMachine.playerInSight)
                return AnimalState.Runaway;

            goPatrol = false;

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
                    stateMachine.PlayRandomAnimations(randomIdleAnim);
                    break;
                default:
                    break;
            }
        }
    }
}