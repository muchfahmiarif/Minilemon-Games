using UnityEngine;
using UnityEngine.AI;

namespace AIStateMachine.Animal
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AnimalStateMachine : StateMachine<AnimalState>
    {
        [SerializeField] protected AnimalState firstState;

        [SerializeField] protected Transform player;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected Animator[] animators;

        [SerializeField] protected float idleTimer;
        [SerializeField] protected string[] randomIdleAnim;

        public bool playerInSight { get; set; }

        protected virtual void AssignComponent()
        {
            if (player == null)
                player = GameObject.FindWithTag("Player").transform;

            if (agent == null)
                agent = GetComponent<NavMeshAgent>();

            if (animators.Length == 0 || animators == null)
            {
                if(GetComponent<Animator>() != null)
                {
                    animators = new Animator[1];
                    animators[0] = GetComponent<Animator>();
                }
                else
                {
                    animators = new Animator[transform.childCount];

                    for (int i = 0; i < transform.childCount; i++)
                    {
                        animators[i] = transform.GetChild(i).GetComponent<Animator>();
                    }
                }
            }
        }

        public void PlayAnimation(string stateName)
        {
            if (stateName == null)
            {
                Debug.LogError("No Animations to play, probably your state string is null");
                return;
            }

            foreach (var item in animators)
            {
                item.Play(stateName);
            }
        }

        public void PlayRandomAnimations(string[] stateNames)
        {
            if(stateNames == null)
            {
                Debug.LogError("No Animations to play, probably your Array is null");
                return;
            }

            int x = Random.Range(0, stateNames.Length);
            foreach (var item in animators)
            {
                item.Play(stateNames[x]);
            }
        }
    }
}