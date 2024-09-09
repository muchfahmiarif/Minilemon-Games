using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Monkey
{
    public class FollowState : AnimalBaseState
    {
        private NavMeshAgent agent;
        protected float jumpHeight = 2f;

        public FollowState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent) : base(stateMachine, player, AnimalState.Follow)
        {
            this.agent = agent;
        }

        public override void EnterState()
        {
            stateMachine.PlayAnimation("Run");
        }

        public override void ExitState()
        {
            agent.ResetPath();
        }

        public override AnimalState GetNextState()
        {
            if (stateMachine.playerInSight)
                return AnimalState.Idle;

            return StateKey;
        }

        public override void OnTriggerStay(Collider other)
        {
        }

        public override void UpdateState()
        {
            if (!stateMachine.playerInSight)
                FollowPath();
            if (agent.isOnOffMeshLink && !isJumping)
                StartJump();
            if (isJumping)
                HandleJump();
        }

        private void FollowPath()
        {
            agent.SetDestination(player.position);
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
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, targetRotation, jumpTimer * 5);

            agent.transform.position = currentPos;

            if(jumpTimer >= 1)
            {
                isJumping = false;
                agent.CompleteOffMeshLink();
                agent.isStopped = false;
                stateMachine.PlayAnimation("Run");
            }
        }
    }
}