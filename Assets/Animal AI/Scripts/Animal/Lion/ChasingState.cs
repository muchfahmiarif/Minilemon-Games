using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Lion
{
    public class ChasingState : AnimalBaseState
    {
        private NavMeshAgent agent;

        public ChasingState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent) : base(stateMachine, player, AnimalState.Chase)
        {
            this.agent = agent;
        }

        public override void EnterState()
        {
            stateMachine.PlayAnimation("Walk");
        }

        public override void ExitState()
        {
        }

        public override AnimalState GetNextState()
        {
            if (stateMachine.playerInSight == false)
                return AnimalState.Idle;

            return StateKey;
        }

        public override void OnTriggerEnter(Collider other)
        {
        }

        public override void OnTriggerStay(Collider other)
        {
            //if (stateMachine.playerInSight == false) return;

            agent.SetDestination(player.position);
        }

        public override void UpdateState()
        {
        }
    }
}