using UnityEngine;

namespace AIStateMachine.Animal.Frog
{
    public class FrogSM : AnimalStateMachine
    {
        [SerializeField] private int radius;

        protected override void RegisterState()
        {
            AssignComponent();
            
            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer));
            States.Add(AnimalState.Patrol, new PatrolState(this, player, agent, radius));

            CurrentState = States[firstState];
        }
    }
}