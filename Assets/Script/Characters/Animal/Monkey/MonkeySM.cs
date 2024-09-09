using UnityEngine;

namespace AIStateMachine.Animal.Monkey
{
    public class MonkeySM : AnimalStateMachine
    {
        // [SerializeField] private int radius;

        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer, randomIdleAnim));
            States.Add(AnimalState.Follow, new FollowState(this, player, agent));

            CurrentState = States[firstState];
        }
    }
}