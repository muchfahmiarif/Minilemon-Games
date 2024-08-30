using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Clownfish
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
            stateMachine.PlayAnimation("Swim");
            agent.speed = 6;
            RunawayPattern(agent);
        }

        public override void ExitState()
        {
            agent.ResetPath();
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