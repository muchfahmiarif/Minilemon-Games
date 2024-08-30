using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Horse
{
    public class RunawayState : AnimalBaseState
    {
        private NavMeshAgent agent;

        public RunawayState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent) : base(stateMachine, player, AnimalState.Runaway)
        {
            this.agent = agent;
        }

        public override void EnterState()
        {
            RunawayPath();
        }

        public override void ExitState()
        {
        }

        public override AnimalState GetNextState()
        {
            if (IsReachedPoint())
                return AnimalState.Idle;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
        }

        private void RunawayPath()
        {
            RunawayPattern(agent);

            stateMachine.PlayAnimation("Run");
        }

        private bool IsReachedPoint()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                return true;

            return false;
        }
    }
}