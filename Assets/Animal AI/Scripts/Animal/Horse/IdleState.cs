using UnityEngine;
using StarterAssets;

namespace AIStateMachine.Animal.Horse
{
    public class IdleState : AnimalBaseState
    {
        private StarterAssetsInputs input;

        private bool playerRunInSight;
        private float idleTimer, idleTimeInterval;
        private string[] randomAnim;

        public IdleState(HorseSM stateMachine, Transform player, float idleTimer, string[] idleAnim, StarterAssetsInputs input) : base(stateMachine, player, AnimalState.Idle)
        {
            this.input = input;
            this.idleTimer = idleTimer;
            randomAnim = idleAnim;
        }

        public override void EnterState()
        {
            stateMachine.PlayAnimation("Idle_A");
            playerRunInSight = false;
        }

        public override void ExitState()
        {
            stateMachine.PlayAnimation("Clicked");
        }

        public override AnimalState GetNextState()
        {
            if (playerRunInSight)
                return AnimalState.Runaway;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
            playerRunInSight = input.sprint || input.jump;
        }

        public override void UpdateState()
        {
            idleTimeInterval += Time.deltaTime;
            if (idleTimeInterval < idleTimer) return;

            stateMachine.PlayRandomAnimations(randomAnim);

            idleTimeInterval = 0;
        }
    }
}