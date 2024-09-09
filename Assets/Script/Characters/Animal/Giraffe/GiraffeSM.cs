using UnityEngine;

using AIStateMachine.Animal.Hare;

namespace AIStateMachine.Animal.Giraffe
{
    public class GiraffeSM : AnimalStateMachine
    {
        [SerializeField] private int radius;

        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer, randomIdleAnim));
            States.Add(AnimalState.Patrol, new PatrolState(this, player, agent, radius));

            CurrentState = States[firstState];
        }
    }
}