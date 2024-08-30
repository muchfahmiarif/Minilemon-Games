using UnityEngine;

namespace AIStateMachine.Animal.Horse
{
    public class HorseSM : AnimalStateMachine
    {
        [SerializeField] private StarterAssets.StarterAssetsInputs input;

        protected override void RegisterState()
        {
            AssignComponent();

            States.Add(AnimalState.Idle, new IdleState(this, player, idleTimer, randomIdleAnim, input));
            States.Add(AnimalState.Runaway, new RunawayState(this, player, agent));

            CurrentState = States[firstState];
        }
    }
}