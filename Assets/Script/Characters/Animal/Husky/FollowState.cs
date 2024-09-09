using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Husky
{
    public class FollowState : AnimalBaseState
    {
        private NavMeshAgent agent;

        public FollowState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent) : base(stateMachine, player, AnimalState.Follow)
        {
            this.agent = agent;
        }

        public override void EnterState()
        {
            stateMachine.PlayAnimation("Run");
        }

        public override void ExitState()
        {
            agent.ResetPath();
        }

        public override AnimalState GetNextState()
        {
            if (stateMachine.playerInSight)
                return AnimalState.Idle;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
            if (!stateMachine.playerInSight)
                FollowPath();
        }

        private void FollowPath()
        {
            agent.SetDestination(player.position);
        }
    }
}