using UnityEngine;

namespace AIStateMachine.Animal.Goose
{
    public class IdleState : AnimalBaseState
    {
        private float idleTimer, idleTimeInterval;
        private string[] randomIdleAnim;

        public IdleState(AnimalStateMachine stateMachine, Transform player, float idleTimer, string[] randomIdleAnim) : base(stateMachine, player, AnimalState.Idle)
        {
            this.idleTimer = idleTimer;
            this.randomIdleAnim = randomIdleAnim;
        }

        public override void EnterState()
        {
            stateMachine.PlayAnimation("Idle_A"); 
        }

        public override void UpdateState()
        {
            idleTimeInterval += Time.deltaTime;
            if (idleTimeInterval < idleTimer) return;

            stateMachine.PlayRandomAnimations(randomIdleAnim);

            idleTimeInterval = 0;
        }

        public override void ExitState()
        {
        }

        public override AnimalState GetNextState()
        {
            if (stateMachine.playerInSight)
                return AnimalState.Runaway;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }
    }
}