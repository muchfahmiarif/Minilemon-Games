using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Utilities
{
    [RequireComponent(typeof(Rigidbody))]
    public class PushPullObject : MonoBehaviour
    {
        [Tooltip("Enable debug mode to receive console reports.")]
        [SerializeField] bool debug;

        [Tooltip("[Requiered] Reference to the player's transform variable.")]
        [SerializeField] TransformVar player;

        [Tooltip("Distance of the raycast used to detect ground, and moving platforms.")]
        [SerializeField] float raycastDistance = 1.3f;

        [Tooltip("Tag used to identify moving platforms (Unity Tag-System).")]
        [SerializeField] string movingPlatformTag;

        [Tooltip("Tag used to identify moving platforms (Malbers Tag-System).")]
        [SerializeField] Tag movingPlatformACTag;

        [Tooltip("Tag used to identify the Player (Unity Tag-System). [One Tag system is required]")]
        [SerializeField] string playerTag;

        [Tooltip("Tag used to identify the Player (Malbers Tag-System). [One Tag system is required]")]
        [SerializeField] Tag playerACTag;

        [Tooltip("Movement threshold value. Events trigger based on this threshold.")]
        public float movementThreshold;

        [Tooltip("Vertical distance threshold to determine when the object is considered grounded.")]
        [SerializeField] float groundThreshold = 0.1f;

        [Tooltip("Angle limit to distinguish flat ground from sloped surfaces.")]
        [SerializeField] float slopeThresholdAngle = 45.0f;

        [Tooltip("Represents the initial collision position when the object lands.")]
        [SerializeField] Transform positionOnLanding;

        [Tooltip("Denotes a position located underneath the object, suitable for particle effects beneath the object.")]
        [SerializeField] Transform positionBottom;

        [Tooltip("Allows adjustment of the vertical offset for the bottom position as needed.")]
        [SerializeField] float positionBottomOffset;

        [Tooltip("Represents the initial collision position when the object collides.")]
        [SerializeField] Transform positionCollision;


        [Tooltip("When enabled, triggers the 'OnMoving' event if the object's velocity exceeds the specified threshold.")]
        public bool useRigidbodyVelocity;

        [Tooltip("Specifies the velocity threshold for triggering the 'OnMoving' event.")]
        [SerializeField] float rigidbodyVelocity;

        [Tooltip("Enable rotation mode for the object.")]
        public bool rotateMode;

        [Tooltip("Toggle to make the object unaffected by external forces like explosions.")]
        public bool useKinematicState;

        [Tooltip("Prevent the push-pull object from sliding down slopes while in push-pull state.")]
        public bool freezePosition;

        [Tooltip("Delay in seconds before the collider stops registering as grounded and applies kinematic settings if grounded.")]
        [SerializeField] float delaySetKinematic = 0.2f;

        [Tooltip("Event triggered when the animal input is above the movement threshold.")]
        public UnityEvent OnObjectMove;

        [Tooltip("Event triggered when the animal input is below the movement threshold.")]
        public UnityEvent OnObjectStop;

        [Tooltip("Event triggered when the object becomes grounded.")]
        public UnityEvent OnLanding;

        [Tooltip("Event triggered when the object collides.")]
        public UnityEvent OnCollision;

        [Tooltip("Can be used to restrict movement. For example, in a 2D scenario, you can block the unused movements.")]
        public bool restrictMovement;


        /// <summary>
        /// Flag indicating whether the object is currently being pushed.
        /// </summary>
        [HideInInspector] public bool isPushed = false;

        private readonly List<ContactPoint> lastCollisionContacts = new();
        private Transform initialParent;
        private GameObject platform;
        private Rigidbody rigidBody;
        private RigidbodyConstraints savedConstraints;
        private bool wasInAir = true;

        /// <summary>
        /// Flag to detect if the collider detects Obstacles
        /// </summary>
        public bool moveBackwardsBlocked;
        /// <summary>
        /// Flag to detect if the collider detects Obstacles
        /// </summary>
        public bool moveForwardsBlocked;
        /// <summary>
        /// Flag to detect if the collider detects Obstacles
        /// </summary>
        public bool moveLeftBlocked;
        /// <summary>
        /// Flag to detect if the collider detects Obstacles
        /// </summary>
        public bool moveRightBlocked;

        private bool initialMoveBackwardsBlocked;
        private bool initialMoveForwardsBlocked;
        private bool initialMoveLeftBlocked;
        private bool initialMoveRightBlocked;

        private void Start()
        {
            initialParent = gameObject.transform.parent;
            rigidBody = GetComponent<Rigidbody>();
            SaveConstraints();
            SaveInitialMovementBlocks();
        }

        private void SaveConstraints()
        {
            savedConstraints = rigidBody.constraints;
        }

        public void RestoreConstraints()
        {
            rigidBody.constraints = savedConstraints;
        }

        private void SaveInitialMovementBlocks()
        {
            initialMoveBackwardsBlocked = moveBackwardsBlocked;
            initialMoveForwardsBlocked = moveForwardsBlocked;
            initialMoveLeftBlocked = moveLeftBlocked;
            initialMoveRightBlocked = moveRightBlocked;
        }

        public void RestoreInitialMovementBlocks()
        {
            moveBackwardsBlocked = initialMoveBackwardsBlocked;
            moveForwardsBlocked = initialMoveForwardsBlocked;
            moveLeftBlocked = initialMoveLeftBlocked;
            moveRightBlocked = initialMoveRightBlocked;
        }

        IEnumerator SetRigidbodyKinematicAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // If the Object is still in motion then restart the process
            if (rigidBody.velocity.magnitude < 0.05f)
            {
                rigidBody.isKinematic = true;
            }
            else
            {
                StartCoroutine(SetRigidbodyKinematicAfterDelay(delay));
            }
        }

        public void GroundCheck()
        {
            if (useKinematicState && !isPushed)
            {
                StartCoroutine(SetRigidbodyKinematicAfterDelay(delaySetKinematic));
            }

            bool isInAir = !Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance);

            if ((!isInAir && wasInAir) || isPushed)
            {
                // Check if the gound is a moving platform
                if ((!string.IsNullOrEmpty(movingPlatformTag) && hit.collider.CompareTag(movingPlatformTag)) || hit.transform.HasMalbersTag(movingPlatformACTag))
                {
                    platform = hit.collider.gameObject;
                    transform.SetParent(hit.transform);
                }
                else
                {
                    transform.SetParent(initialParent);
                }

                // Check if the cube landed
                if (!isPushed)
                {
                    if (positionOnLanding != null)
                    {
                        positionOnLanding.position = hit.point;
                    }
                    OnLanding.Invoke();
                }
            }
            else if (isInAir)
            {
                transform.SetParent(initialParent);
            }
            wasInAir = isInAir;
        }

        /// <summary>
        /// Block the movement in a specific direction
        /// </summary>
        /// <param name="blockBackwards"></param>
        /// <param name="blockForwards"></param>
        /// <param name="blockLeft"></param>
        /// <param name="blockRight"></param>
        public void SetBlockMovement(bool blockBackwards, bool blockForwards, bool blockLeft, bool blockRight)
        {
            moveBackwardsBlocked = blockBackwards;
            moveForwardsBlocked = blockForwards;
            moveLeftBlocked = blockLeft;
            moveRightBlocked = blockRight;
        }

        /// <summary>
        /// Returns a string with infomation about block status
        /// </summary>
        /// <returns></returns>
        public string GetBlockMovement()
        {
            string backwardsColor = moveBackwardsBlocked ? "<color=red>true</color>" : "false";
            string forwardsColor = moveForwardsBlocked ? "<color=red>true</color>" : "false";
            string leftColor = moveLeftBlocked ? "<color=red>true</color>" : "false";
            string rightColor = moveRightBlocked ? "<color=red>true</color>" : "false";

            return $"Movementblocked => Backwards: {backwardsColor}, Forwards: {forwardsColor}, Left: {leftColor}, Right: {rightColor}";
        }

        /// <summary>
        /// Set the Rotation mode of the PushPull Object
        /// </summary>
        /// <param name="ShouldRotate"></param>
        public void SetRotateMode(bool ShouldRotate)
        {
            rotateMode = ShouldRotate;
        }

        /// <summary>
        /// Teleport the object to the given Transform
        /// </summary>
        /// <param name="targetPosition"></param>
        public void Teleport(Transform targetPosition)
        {
            if (targetPosition != null)
            {
                transform.position = targetPosition.position;

                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Determines if the object is pushable and pullable.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanPush()
        {
            // Provide your logic to enable the object to be pushed and pulled.
            return true;
        }


        private void OnCollisionEnter(Collision collision)
        {
            GroundCheck();

            foreach (ContactPoint contact in collision.contacts)
            {
                float heightDifference = contact.point.y - player.Value.position.y;
                
                if (heightDifference < groundThreshold)
                {
                    continue; // Don't collide with the ground
                }

                Vector3 contactNormal = contact.normal;

                float angle = Vector3.Angle(contactNormal, Vector3.up);

                if (angle < slopeThresholdAngle)
                {
                    continue; // Don't collide with the slope
                }

                if ((!string.IsNullOrEmpty(playerTag) && collision.transform.CompareTag(playerTag)) || collision.transform.HasMalbersTag(playerACTag))
                {
                    continue; // Don't collide with the player
                }

                if (positionCollision != null)
                {
                    positionCollision.position = contact.point;
                }

                OnCollision.Invoke();
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (useRigidbodyVelocity && (rigidbodyVelocity < rigidBody.velocity.magnitude) && !isPushed)
            {
                OnObjectMove.Invoke();
            }

            if (positionBottom != null)
            {
                positionBottom.position = transform.position + (Vector3.down * positionBottomOffset);
            }

            foreach (ContactPoint contact in collision.contacts)
            {
                float heightDifference = contact.point.y - player.Value.position.y;

                if (heightDifference < groundThreshold)
                {
                    continue; // Don't collide with the ground
                }

                Vector3 contactNormal = contact.normal;

                float angle = Vector3.Angle(contactNormal, Vector3.up);

                if (angle < slopeThresholdAngle)
                {
                    continue; // Don't collide with the slope
                }

                if ((!string.IsNullOrEmpty(playerTag) && collision.transform.CompareTag(playerTag)) || collision.transform.HasMalbersTag(playerACTag))
                {
                    continue; // Don't collide with the player
                }

                Vector3 cubeToContact = contact.point - transform.position;

                float forwardAngle = Vector3.Angle(player.Value.forward, cubeToContact);

                if (forwardAngle < 90.0f)
                {
                    moveForwardsBlocked = true;
                }
                else
                {
                    moveBackwardsBlocked = true;
                }

                Vector3 rightDir = player.Value.right;
                float dotRight = Vector3.Dot(rightDir, cubeToContact.normalized);

                if (dotRight > 0.5f)
                {
                    moveRightBlocked = true;
                }
                else if (dotRight < -0.5f)
                {
                    moveLeftBlocked = true;
                }
            }

            if (debug)
            {
                Debug.Log("<color=green>" + gameObject.name + "</color> Collides with: <color=red>" + collision.gameObject.name + "</color>",this);
                Debug.Log(GetBlockMovement(),this);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            moveForwardsBlocked = false;
            moveBackwardsBlocked = false;
            moveLeftBlocked = false;
            moveRightBlocked = false;
            lastCollisionContacts.Clear();

            if (restrictMovement)
            {
                RestoreInitialMovementBlocks();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = Color.red;

                foreach (ContactPoint contact in lastCollisionContacts)
                {
                    Gizmos.DrawSphere(contact.point, 1f);
                }

                Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * 1.5f));
                Gizmos.DrawSphere(transform.position + (Vector3.down * 1.5f), 0.1f);

                if (player.Value != null)
                {
                    Vector3 playerPosition = player.Value.position + Vector3.up;
                    Vector3 playerForward = player.Value.forward;
                    Vector3 lineEnd = playerPosition + (playerForward * 2.0f);
                    Gizmos.DrawLine(playerPosition, lineEnd);
                }
            }
        }
#endif
    }
}
