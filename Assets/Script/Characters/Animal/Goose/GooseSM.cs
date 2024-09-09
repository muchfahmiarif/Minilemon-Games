using UnityEngine;

namespace AIStateMachine.Animal.Goose
{
    public class GooseSM : AnimalStateMachine
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