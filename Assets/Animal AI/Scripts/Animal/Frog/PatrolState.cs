using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Frog
{
    public class PatrolState : AnimalBaseState
    {
        private NavMeshAgent agent;

        private int radius;

        public PatrolState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent, int radius) : base(stateMachine, player, AnimalState.Patrol)
        {
            this.agent = agent;
            this.radius = radius;
        }

        public override void EnterState()
        {
            PatrolPattern(agent, radius);
            stateMachine.PlayAnimation("Jump");
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

        private bool IsReachedPoint()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                return true;

            return false;
        }
    }
}