using UnityEngine;

namespace AIStateMachine.Animal.Lion
{
    public class LionSM : AnimalStateMachine
    {
        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer, randomIdleAnim));
            States.Add(AnimalState.Chase, new ChasingState(this, player, agent));

            CurrentState = States[firstState];
        }
    }
}