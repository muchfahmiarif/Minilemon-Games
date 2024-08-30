using UnityEngine;

namespace AIStateMachine.Animal.Husky
{
    public class HuskySM : AnimalStateMachine
    {
        [SerializeField] private bool isTamed;

        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer, randomIdleAnim));
            States.Add(AnimalState.Follow, new FollowState(this, player, agent));

            CurrentState = States[firstState];
        }
    }
}