using UnityEngine;

namespace AIStateMachine.Animal.Hedgehog
{
    public class Hedgehog : AnimalStateMachine
    {
        [SerializeField] private int radius;

        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player));
            States.Add(AnimalState.Patrol, new PatrolState(this, player, agent, radius));

            CurrentState = States[firstState];
        }
    }
}