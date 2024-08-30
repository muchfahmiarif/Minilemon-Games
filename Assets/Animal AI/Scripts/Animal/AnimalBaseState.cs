using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal
{
    public abstract class AnimalBaseState : BaseState<AnimalState>
    {
        protected AnimalStateMachine stateMachine { get; private set; }
        protected Transform player { get; private set; }

        protected bool isJumping = false;
        protected Vector3 startPos;
        protected Vector3 endPos;
        protected float jumpHeight = .5f;
        protected float jumpDuration = 1f;
        protected float jumpTimer = 0.0f;

        public AnimalBaseState(AnimalStateMachine stateMachine, Transform player, AnimalState key) : base(key)
        {
            this.stateMachine = stateMachine;
            this.player = player;
        }

        public override void OnTriggerEnter(Collider other)
        {
            stateMachine.playerInSight = other.transform.Equals(player);
        }

        public override void OnTriggerExit(Collider other)
        {
            stateMachine.playerInSight = !other.transform.Equals(player);
        }

        protected void PatrolPattern(NavMeshAgent agent, float radius)
        {
            Vector3 random = Random.insideUnitSphere * radius;
            random += stateMachine.transform.position;

            if (NavMesh.SamplePosition(random, out NavMeshHit hit, radius, agent.areaMask))
                agent.SetDestination(hit.position);
        }

        protected void RunawayPattern(NavMeshAgent agent)
        {
            Vector3 bestPoint = Vector3.zero;

            int numChecks = 8;
            float maxDistance = -1f;
            float angleIncrement = 360f / numChecks;

            for (int i = 0; i < numChecks; i++)
            {
                float angle = i * angleIncrement;

                Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
                Vector3 potentialPosition = stateMachine.transform.position + direction * 5f;

                if (NavMesh.SamplePosition(potentialPosition, out NavMeshHit hit, 5f, agent.areaMask))
                {
                    float distanceFromPlayer = Vector3.Distance(hit.position, player.position);
                    if (distanceFromPlayer > maxDistance)
                    {
                        bestPoint = hit.position;
                        maxDistance = distanceFromPlayer;
                    }
                }
            }

            if (maxDistance > -1f)
            {
                agent.SetDestination(bestPoint);
            }
            else
            {
                Vector3 directionFromPlayer = stateMachine.transform.position - player.position;
                Vector3 runToPosition = stateMachine.transform.position + directionFromPlayer.normalized * 5f;

                NavMesh.SamplePosition(runToPosition, out NavMeshHit hit, 5f, agent.areaMask);

                agent.SetDestination(hit.position);
            }
        }
    }
}