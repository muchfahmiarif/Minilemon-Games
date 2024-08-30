using UnityEngine;

namespace AIStateMachine.Animal.Clownfish
{
    public class ClownfishSM : AnimalStateMachine
    {
        [SerializeField] private int radius;

        protected override void RegisterState()
        {
            AssignComponent();
            
            States.Add(AnimalState.Idle, new IdleState(this, player));
            States.Add(AnimalState.Patrol, new PatrolState(this, player, agent, radius));
            States.Add(AnimalState.Runaway, new RunawayState(this, player, agent));

            CurrentState = States[firstState];
        }
    }
}