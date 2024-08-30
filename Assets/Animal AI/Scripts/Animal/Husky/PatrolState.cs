using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Husky
{
    public class PatrolState : AnimalBaseState
    {
        private NavMeshAgent agent;

        public PatrolState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent) : base(stateMachine, player, AnimalState.Patrol)
        {
            this.agent = agent;

        }

        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }

        public override AnimalState GetNextState()
        {
            throw new System.NotImplementedException();
        }

        public override void OnTriggerStay(Collider other)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            throw new System.NotImplementedException();
        }
    }
}