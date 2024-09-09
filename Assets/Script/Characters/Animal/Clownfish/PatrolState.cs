using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Clownfish
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
            PatrolPath();
        }

        public override void ExitState()
        {
            agent.ResetPath();
        }

        public override AnimalState GetNextState()
        {
            if (IsReachedPoint())
                return AnimalState.Idle;

            if (stateMachine.playerInSight)
                return AnimalState.Runaway;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {

        }

        private void PatrolPath()
        {
            PatrolPattern(agent, radius);

            stateMachine.PlayAnimation("Swim");
            //agent.speed = 2f;
        }

        private bool IsReachedPoint()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                return true;

            return false;
        }
    }
}