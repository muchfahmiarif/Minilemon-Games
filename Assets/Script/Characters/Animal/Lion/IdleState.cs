using UnityEngine;

namespace AIStateMachine.Animal.Lion
{
    public class IdleState : AnimalBaseState
    {
        private float idleTimer, idleTimeInterval;
        private string[] randomIdleAnim;

        public IdleState(AnimalStateMachine stateMachine, Transform player, float idleTimer, string[] animations) : base(stateMachine, player, AnimalState.Idle)
        {
            this.idleTimer = idleTimer;
            randomIdleAnim = animations;
        }

        public override void EnterState()
        {
            stateMachine.PlayRandomAnimations(randomIdleAnim);
        }

        public override void ExitState()
        {
        }

        public override AnimalState GetNextState()
        {
            if(stateMachine.playerInSight)
                return AnimalState.Chase;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
            idleTimeInterval += Time.deltaTime;
            if (idleTimeInterval < idleTimer) return;

            stateMachine.PlayRandomAnimations(randomIdleAnim);

            idleTimeInterval = 0;
        }
    }
}