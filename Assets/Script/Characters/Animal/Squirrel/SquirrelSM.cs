using UnityEngine;

using AIStateMachine.Animal.Hare;

namespace AIStateMachine.Animal.Squirrel
{
    public class SquirrelSM : AnimalStateMachine
    {
        [SerializeField] private int radius;

        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer, randomIdleAnim));
            States.Add(AnimalState.Patrol, new PatrolState(this, player, agent, radius));
            States.Add(AnimalState.Runaway, new RunawayState(this, player, agent));

            CurrentState = States[firstState];
        }
    }
}