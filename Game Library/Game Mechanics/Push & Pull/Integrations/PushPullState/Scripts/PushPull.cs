using MalbersAnimations.Scriptables;
using MalbersAnimations.Utilities;
using System.Collections;
using UnityEngine;

namespace MalbersAnimations.Controller
{
    //[CreateAssetMenu(menuName = "Malbers Animations/Push and Pull", fileName = "PushPull", order = 4001)]
    public class PushPull : State
    {
        public override string StateName => "Push and Pull";
        public override string StateIDName => "PushPull";

        [Header("Detection Settings")]

        [Tooltip("Tag used to identify push and pull objects (Unity Tag-System). [One Tag system is required]")]
        [SerializeField] string pushPullObjectTag;

        [Tooltip("Tag used to identify push and pull objects (Malbers Tag-System). [One Tag system is required]")]
        [SerializeField] Tag pushPullObjectACTag;

        [Tooltip("Radius for detecting push and pull objects")]
        [SerializeField] float detectionRadius = 0.3f;

        [Tooltip("Position offset for the raycast used for detection")]
        [SerializeField] Vector3 raycastPosition;

        [Tooltip("Length of the raycast used for detection")]
        [SerializeField] float rayLength = 1f;

        [Tooltip("The maximum number of RaycastHit objects that can be stored in the array when performing raycasts. Increasing this value may improve accuracy but can impact performance, especially in scenes with many colliders.")]
        [SerializeField] int maxRaycastHits = 10;

        [Tooltip("Speed of rotation when aligning to push and pull objects")]
        [SerializeField] float alignRotationSpeed = 50f;

        [Tooltip("Duration for character align rotation")]
        [SerializeField] float alignRotationDuration = 2f;

        [Header("Movement Settings")]

        [Tooltip("Movement speed of the character and the push pull object")]
        public float pushPullMoveSpeed = 1f;
        [Tooltip("Rotation speed of the character and the push pull object")]
        public float pushPullRotationSpeed = 20f;

        [Tooltip("Set a string value from an input that you would like to use to enable the rotation mode on the push-pull object.")]
        public StringReference enableRotateMode = new("Sprint");

        private bool validPushPullObject;
        private GameObject pushPullGameObject;
        private Rigidbody pushPullObjectRigidbody;
        private PushPullObject pushPullObject;
        private Transform pushPullObjectTransform;
        private int exitStatus = 1;
        private readonly float detectionDistance = 0.001f;

        public override void StatebyInput()
        {
            validPushPullObject = false;

            if (InputValue)
            {
                Activate();
            }
        }

        public override void ExtraInputs(IInputSource InputSource, bool connect)
        {
            if (connect)
            {
                InputSource.ConnectInput(enableRotateMode, EnableRotationMode);
            }
            else
            {
                InputSource.DisconnectInput(enableRotateMode, EnableRotationMode);
            }
        }

        public void EnableRotationMode(bool InputValue)
        {
            if (InputValue && pushPullObject != null)
            {
                pushPullObject.rotateMode = true;
            }
            else if (pushPullObject != null)
            {
                pushPullObject.rotateMode = false;
            }
        }

        public override void Activate()
        {
            CheckPushPullObject(); // Initial check for the Push/Pull Object

            if (validPushPullObject)
            {
                base.Activate();
                animal.UseCameraInput = false;
                pushPullObject.isPushed = true;
                animal.StartCoroutine(RotateToPushPullObjectCoroutine(pushPullObjectTransform));

                if (pushPullObject.freezePosition)
                {
                    pushPullObjectRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;//  | RigidbodyConstraints.FreezeRotationY
                }
            }
            else
            {
                Debugging("No PushPullObject tag found on a object");
            }
        }

        public override void EnterCoreAnimation()
        {
            base.EnterCoreAnimation();
        }

        public override void EnterTagAnimation()
        {
            base.EnterTagAnimation();
        }

        public override void OnStateMove(float deltatime)
        {
            if (InCoreAnimation && animal.CheckIfGrounded() && pushPullObject.CanPush())
            {
                CheckPushPullObjectRay(); // Check if the object is befor the player
                if (validPushPullObject)
                {
                    CheckBlocker(); // Check for any blockers that may hinder movement
                    PushPullEvent(); // Possible events triggerd on the Object script
                    if (pushPullObject.rotateMode)
                    {
                        RotatePushPullObject();
                    }
                    else
                    {
                        MovePushPullObject();
                    }
                }
                else
                {
                    AllowExit();
                    Debugging("No PushPullObject tag before the player");
                }
            }
            else
            {
                AllowExit();
                Debugging("The player is not Grounded or not in CoreAnimation");
            }
        }

        private void CheckPushPullObject() // Used for the initial check
        {
            Vector3 sphereOrigin = transform.position + raycastPosition;
            Vector3 sphereDirection = transform.forward;

            RaycastHit[] hits = new RaycastHit[maxRaycastHits];
            int hitCount = Physics.SphereCastNonAlloc(sphereOrigin, detectionRadius, sphereDirection, hits, detectionDistance);

            Debug.DrawRay(sphereOrigin, sphereDirection * detectionRadius, Color.green);

            validPushPullObject = false;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = hits[i];

                if ((!string.IsNullOrEmpty(pushPullObjectTag) && hit.collider.CompareTag(pushPullObjectTag))
                    || hit.collider.HasMalbersTag(pushPullObjectACTag))
                {
                    validPushPullObject = true;
                    pushPullObjectRigidbody = hit.collider.GetComponent<Rigidbody>();
                    pushPullGameObject = hit.collider.gameObject;
                    pushPullObjectTransform = hit.collider.transform;
                    pushPullObject = pushPullGameObject.GetComponent<PushPullObject>();
                    break;
                }
            }
        }

        private void CheckPushPullObjectRay() // Checks constantly for the object
        {
            Vector3 origin = transform.position + raycastPosition;
            Debug.DrawRay(origin, transform.forward * rayLength, Color.yellow);

            RaycastHit[] hits = new RaycastHit[maxRaycastHits];
            int numHits = Physics.RaycastNonAlloc(origin, transform.forward, hits, rayLength);

            bool isPushPullObjectDetected = false;

            for (int i = 0; i < numHits; i++)
            {
                RaycastHit hit = hits[i];
                if ((!string.IsNullOrEmpty(pushPullObjectTag) && hit.collider.CompareTag(pushPullObjectTag)) || hit.collider.HasMalbersTag(pushPullObjectACTag))
                {
                    isPushPullObjectDetected = true;
                    break;
                }
            }
            validPushPullObject = isPushPullObjectDetected;
        }

        private IEnumerator RotateToPushPullObjectCoroutine(Transform surfaceNormal)
        {
            Vector3 directionToTarget = surfaceNormal.position - transform.position;
            directionToTarget.y = 0f;

            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);
                float elapsedTime = 0f;

                while (elapsedTime < alignRotationDuration)
                {
                    elapsedTime += Time.deltaTime * alignRotationSpeed;
                    float progress = Mathf.Clamp01(elapsedTime / alignRotationDuration);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, progress);
                    yield return null;
                }
            }
        }

        private void RotatePushPullObject()
        {
            transform.RotateAround(pushPullGameObject.transform.position, Vector3.up, -animal.MovementAxis.x * pushPullRotationSpeed * Time.deltaTime);

            Vector3 forwardMovement = -animal.MovementAxis.z * pushPullMoveSpeed * Time.deltaTime * pushPullGameObject.transform.forward;

            pushPullGameObject.transform.position += forwardMovement;
            transform.position += forwardMovement;

            Vector3 directionToPlayer = transform.position - pushPullGameObject.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            pushPullGameObject.transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);

            Vector3 directionToCube = pushPullGameObject.transform.position - transform.position;
            Quaternion targetRotationCube = Quaternion.LookRotation(directionToCube, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, targetRotationCube.eulerAngles.y, 0f);
        }

        private void MovePushPullObject()
        {
            Vector3 directionToCube = pushPullGameObject.transform.position - transform.position;
            directionToCube.y = 0f;
            Quaternion targetRotationCube = Quaternion.LookRotation(directionToCube, Vector3.up);
            transform.rotation = Quaternion.Euler(0f, targetRotationCube.eulerAngles.y, 0f);

            Vector3 forwardDirection = transform.forward;
            forwardDirection.y = 0f;

            Vector3 movement = pushPullMoveSpeed * Time.deltaTime * ((forwardDirection * animal.MovementAxis.z) + (transform.right * animal.MovementAxis.x)).normalized;
            animal.transform.position += movement;

            pushPullObjectTransform.position += movement;
        }

        public override void StateExitByInput()
        {
            animal.LockMovement = true; // Otherwise the cube can be moved in the exit animation
            SetExitStatus(exitStatus);
        }


        public void CheckBlocker() // Checks for collisions that block movement and adjusts movement accordingly
        {
            if (pushPullObject.rotateMode)
            {
                animal.MovementAxis.z = (pushPullObject.moveLeftBlocked || pushPullObject.moveRightBlocked) ? Mathf.Clamp(animal.MovementAxis.z, -1, 0) : animal.MovementAxis.z;

                animal.MovementAxis.x = pushPullObject.moveLeftBlocked ? Mathf.Clamp(animal.MovementAxis.x, -1, 0) : animal.MovementAxis.x;

                animal.MovementAxis.x = pushPullObject.moveRightBlocked ? Mathf.Clamp(animal.MovementAxis.x, 0, 1) : animal.MovementAxis.x;

                animal.MovementAxis.z = pushPullObject.moveBackwardsBlocked ? Mathf.Clamp(animal.MovementAxis.z, 0, 1) : animal.MovementAxis.z;

                animal.MovementAxis.z = pushPullObject.moveForwardsBlocked ? Mathf.Clamp(animal.MovementAxis.z, -1, 0) : animal.MovementAxis.z;
            }
            else
            {
                animal.MovementAxis.x = pushPullObject.moveLeftBlocked ? Mathf.Clamp(animal.MovementAxis.x, 0, 1): animal.MovementAxis.x;

                animal.MovementAxis.x = pushPullObject.moveRightBlocked ? Mathf.Clamp(animal.MovementAxis.x, -1, 0): animal.MovementAxis.x;

                animal.MovementAxis.z = pushPullObject.moveBackwardsBlocked ? Mathf.Clamp(animal.MovementAxis.z, 0, 1): animal.MovementAxis.z;

                animal.MovementAxis.z = pushPullObject.moveForwardsBlocked ? Mathf.Clamp(animal.MovementAxis.z, -1, 0): animal.MovementAxis.z;
            }
            Debugging(pushPullObject.GetBlockMovement());
        }

        public void PushPullEvent() //Triggering events on the PushPullObject script
        {
            if (pushPullObject.isPushed)
            {
                if (pushPullObject.useKinematicState)
                {
                    pushPullObjectRigidbody.isKinematic = false;
                }

                if (Mathf.Abs(animal.MovementAxis.x) > pushPullObject.movementThreshold || Mathf.Abs(animal.MovementAxis.z) > pushPullObject.movementThreshold)
                {
                    pushPullObject.OnObjectMove.Invoke();
                }
                else
                {
                    pushPullObject.OnObjectStop.Invoke();
                }
            }
        }

        public override void ResetStateValues()
        {
            if (pushPullObject != null)
            {
                pushPullObject.GroundCheck();
                pushPullObject.isPushed = false;
                pushPullObject.OnObjectStop.Invoke();
                pushPullObject.RestoreConstraints();
                if (pushPullObject.restrictMovement)
                {
                    pushPullObject.RestoreInitialMovementBlocks();
                }
            }
            validPushPullObject = false;
            pushPullGameObject = null;
            pushPullObject = null;
            pushPullObjectRigidbody = null;
            pushPullObjectTransform = null;
        }

        public override void RestoreAnimalOnExit()
        {
            animal.LockMovement = false;
            animal.ResetCameraInput();
        }
    }
}
