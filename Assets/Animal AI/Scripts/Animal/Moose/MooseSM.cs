using UnityEngine;
using AIStateMachine.Animal.Goose;

namespace AIStateMachine.Animal.Moose
{
    public class MooseSM : AnimalStateMachine
    {
        [SerializeField] private string[] randomRunAnim;

        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer, randomIdleAnim));
            States.Add(AnimalState.Runaway, new RunawayState(this, player, agent, randomRunAnim));

            CurrentState = States[firstState];
        }
    }
}