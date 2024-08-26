#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace MalbersAnimations.Utilities
{

    [CustomEditor(typeof(PushPullObject))]

    public class PushPullObjectEditor : Editor
    {
        SerializedProperty debug;
        SerializedProperty player;
        SerializedProperty rotateMode;
        SerializedProperty useKinematicState;
        SerializedProperty delaySetKinematic;
        SerializedProperty raycastDistance;
        SerializedProperty movingPlatformTag;
        SerializedProperty movingPlatformACTag;
        SerializedProperty movementThreshold;
        SerializedProperty OnObjectMove;
        SerializedProperty OnObjectStop;
        SerializedProperty OnLanding;
        SerializedProperty moveBackwardsBlocked;
        SerializedProperty moveForwardsBlocked;
        SerializedProperty moveLeftBlocked;
        SerializedProperty moveRightBlocked;
        SerializedProperty freezePosition;
        SerializedProperty groundThreshold;
        SerializedProperty slopeThresholdAngle;
        SerializedProperty positionOnLanding;
        SerializedProperty positionBottom;
        SerializedProperty positionBottomOffset;
        SerializedProperty playerTag;
        SerializedProperty playerACTag;
        SerializedProperty positionCollision;
        SerializedProperty useRigidbodyVelocity;
        SerializedProperty rigidbodyVelocity;
        SerializedProperty OnCollision;
        SerializedProperty restrictMovement;

        private bool detectionFoldout;
        private bool eventSectionFoldout;
        private bool movementBlockSettingsFoldout;

        void OnEnable()
        {
            freezePosition = serializedObject.FindProperty("freezePosition");
            debug = serializedObject.FindProperty("debug");
            player = serializedObject.FindProperty("player");
            rotateMode = serializedObject.FindProperty("rotateMode");
            useKinematicState = serializedObject.FindProperty("useKinematicState");
            delaySetKinematic = serializedObject.FindProperty("delaySetKinematic");
            raycastDistance = serializedObject.FindProperty("raycastDistance");
            movingPlatformTag = serializedObject.FindProperty("movingPlatformTag");
            movingPlatformACTag = serializedObject.FindProperty("movingPlatformACTag");
            movementThreshold = serializedObject.FindProperty("movementThreshold");
            OnObjectMove = serializedObject.FindProperty("OnObjectMove");
            OnObjectStop = serializedObject.FindProperty("OnObjectStop");
            OnLanding = serializedObject.FindProperty("OnLanding");
            moveBackwardsBlocked = serializedObject.FindProperty("moveBackwardsBlocked");
            moveForwardsBlocked = serializedObject.FindProperty("moveForwardsBlocked");
            moveLeftBlocked = serializedObject.FindProperty("moveLeftBlocked");
            moveRightBlocked = serializedObject.FindProperty("moveRightBlocked");
            groundThreshold = serializedObject.FindProperty("groundThreshold");
            slopeThresholdAngle = serializedObject.FindProperty("slopeThresholdAngle");
            positionOnLanding = serializedObject.FindProperty("positionOnLanding");
            positionBottom = serializedObject.FindProperty("positionBottom");
            positionBottomOffset = serializedObject.FindProperty("positionBottomOffset");
            playerTag = serializedObject.FindProperty("playerTag");
            playerACTag = serializedObject.FindProperty("playerACTag");
            positionCollision = serializedObject.FindProperty("positionCollision");
            useRigidbodyVelocity = serializedObject.FindProperty("useRigidbodyVelocity");
            rigidbodyVelocity = serializedObject.FindProperty("rigidbodyVelocity");
            OnCollision = serializedObject.FindProperty("OnCollision");
            restrictMovement = serializedObject.FindProperty("restrictMovement");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(debug);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(player, new GUIContent("Player*"));
            EditorGUILayout.PropertyField(playerTag, new GUIContent("Player Tag*"));
            EditorGUILayout.PropertyField(playerACTag, new GUIContent("Player AC Tag*"));
            EditorGUILayout.PropertyField(rotateMode);
            EditorGUILayout.PropertyField(freezePosition);
            EditorGUILayout.PropertyField(useKinematicState);

            if (useKinematicState.boolValue)
            {
                EditorGUILayout.PropertyField(delaySetKinematic);
            }


            EditorGUILayout.Space();

            detectionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(detectionFoldout, "Detection Settings");
            if (detectionFoldout)
            {
                EditorGUILayout.PropertyField(raycastDistance);
                EditorGUILayout.PropertyField(movingPlatformTag);
                EditorGUILayout.PropertyField(movingPlatformACTag);
                EditorGUILayout.PropertyField(groundThreshold);
                EditorGUILayout.PropertyField(slopeThresholdAngle);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            eventSectionFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(eventSectionFoldout, "Event Section");
            if (eventSectionFoldout)
            {
                EditorGUILayout.PropertyField(positionCollision);
                EditorGUILayout.PropertyField(positionOnLanding);
                EditorGUILayout.PropertyField(positionBottom);
                EditorGUILayout.PropertyField(positionBottomOffset);
                EditorGUILayout.PropertyField(movementThreshold);
                EditorGUILayout.PropertyField(useRigidbodyVelocity);

                if (useRigidbodyVelocity.boolValue)
                {
                    EditorGUILayout.PropertyField(rigidbodyVelocity);
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(OnObjectMove);
                EditorGUILayout.PropertyField(OnObjectStop);
                EditorGUILayout.PropertyField(OnLanding);
                EditorGUILayout.PropertyField(OnCollision);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            movementBlockSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(movementBlockSettingsFoldout, "Movement Block Settings");
            if (movementBlockSettingsFoldout)
            {
                EditorGUILayout.PropertyField(restrictMovement);
                if (restrictMovement.boolValue)
                {
                    EditorGUILayout.PropertyField(moveBackwardsBlocked);
                    EditorGUILayout.PropertyField(moveForwardsBlocked);
                    EditorGUILayout.PropertyField(moveLeftBlocked);
                    EditorGUILayout.PropertyField(moveRightBlocked);
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(moveBackwardsBlocked);
                    moveBackwardsBlocked.boolValue = false;
                    EditorGUILayout.PropertyField(moveForwardsBlocked);
                    moveForwardsBlocked.boolValue = false;
                    EditorGUILayout.PropertyField(moveLeftBlocked);
                    moveLeftBlocked.boolValue = false;
                    EditorGUILayout.PropertyField(moveRightBlocked);
                    moveRightBlocked.boolValue = false;
                    EditorGUI.EndDisabledGroup();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif