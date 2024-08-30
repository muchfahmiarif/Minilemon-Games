using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Goose
{
    public class RunawayState : AnimalBaseState
    {
        private NavMeshAgent agent;

        private string[] randomAnim;

        public RunawayState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent, string[] randomAnim) : base(stateMachine, player, AnimalState.Runaway)
        {
            this.agent = agent;
            this.randomAnim = randomAnim;
        }

        public override void EnterState()
        {
            RunawayPath();
        }

        public override void UpdateState()
        {
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
        private void RunawayPath()
        {
            RunawayPattern(agent);

            stateMachine.PlayRandomAnimations(randomAnim);
        }

        private bool IsReachedPoint()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                return true;

            return false;
        }
    }
}