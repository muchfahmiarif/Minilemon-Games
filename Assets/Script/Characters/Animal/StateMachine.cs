using System;
using System.Collections.Generic;

using UnityEngine;

namespace AIStateMachine
{
    public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
    {
        protected Dictionary<EState, BaseState<EState>> States { get; set; } = new();

        protected BaseState<EState> CurrentState { get; set; }

        protected bool IsTransitioningState { get; set; }

        private void Awake()
        {
            RegisterState();
        }

        private void Start()
        {
            CurrentState.EnterState();
        }

        private void Update()
        {
            EState nextStateKey = CurrentState.GetNextState();

            if (!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
            {
                CurrentState.UpdateState();
            }
            else if (!IsTransitioningState)
            {
                TransitionToState(nextStateKey);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            CurrentState.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            CurrentState.OnTriggerStay(other);
        }

        private void OnTriggerExit(Collider other)
        {
            CurrentState.OnTriggerExit(other);
        }

        protected abstract void RegisterState();

        private void TransitionToState(EState stateKey)
        {
            IsTransitioningState = true;
            CurrentState.ExitState();
            CurrentState = States[stateKey];
            CurrentState.EnterState();
            IsTransitioningState = false;
        }
    }
}