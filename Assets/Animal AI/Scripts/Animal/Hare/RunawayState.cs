using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Hare
{
    public class RunawayState : AnimalBaseState
    {
        private NavMeshAgent agent;

        public RunawayState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent) : base(stateMachine, player, AnimalState.Runaway)
        {
            this.agent = agent;
        }

        public override void EnterState()
        {
            RunawayPath();
        }

        public override void ExitState()
        {
            agent.ResetPath();
        }

        public override AnimalState GetNextState()
        {
            if (IsReachedPoint())
                return AnimalState.Idle;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
            if (agent.isOnOffMeshLink && !isJumping)
                StartJump();

            if (isJumping)
                HandleJump();
        }

        private void StartJump()
        {
            isJumping = true;
            startPos = agent.transform.position;
            endPos = agent.currentOffMeshLinkData.endPos;

            agent.isStopped = true;

            stateMachine.PlayAnimation("Jump");

            jumpTimer = 0;
        }

        private void HandleJump()
        {
            jumpTimer += Time.deltaTime / jumpDuration;
            float heightOffset = System.MathF.Sin(jumpTimer * System.MathF.PI) * jumpHeight;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, jumpTimer) + Vector3.up * heightOffset;

            Quaternion targetRotation = Quaternion.LookRotation(endPos - agent.transform.position);
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, targetRotation, jumpTimer * 8);

            agent.transform.position = currentPos;

            if (jumpTimer >= 1)
            {
                isJumping = false;
                agent.CompleteOffMeshLink();
                agent.Warp(agent.transform.position);
                agent.isStopped = false;
                stateMachine.PlayAnimation("Run");
            }
        }

        private void RunawayPath()
        {
            RunawayPattern(agent);

            stateMachine.PlayAnimation("Run");
            agent.speed = 5;
        }

        private bool IsReachedPoint()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                return true;

            return false;
        }
    }
}