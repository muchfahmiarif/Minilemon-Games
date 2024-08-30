using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal.Hare
{
    public class PatrolState : AnimalBaseState
    {
        private NavMeshAgent agent;

        private int radius;

        private Quaternion startRotation, normalRotation;

        public PatrolState(AnimalStateMachine stateMachine, Transform player, NavMeshAgent agent, int radius) : base(stateMachine, player, AnimalState.Patrol)
        {
            this.agent = agent;
            this.radius = radius;
        }

        public override void EnterState()
        {
            PatrolPath();
        }

        public override void ExitState()
        {
            agent.ResetPath();
        }

        public override AnimalState GetNextState()
        {
            if (IsReachedPoint())
                return AnimalState.Idle;
            if (stateMachine.playerInSight)
                return AnimalState.Runaway;

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

        private void PatrolPath()
        {
            PatrolPattern(agent, radius);

            stateMachine.PlayAnimation("Walk");
            agent.speed = 1.5f;
        }

        private bool IsReachedPoint()
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                return true;

            return false;
        }
    }
}