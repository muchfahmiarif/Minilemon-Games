using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PresentAnimator))]
[CanEditMultipleObjects]
public class PresentAnimatorEditor : Editor
{
    Rect rect;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("FXRoot"));

        drawUILine();

        EditorGUILayout.LabelField("Idle");
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IdleAnim"), new GUIContent("Animation"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IdleFX"), new GUIContent("FX"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsPreLoadIdleFX"), new GUIContent("Preload FX"), true);
        EditorGUILayout.Slider(serializedObject.FindProperty("IdleFXDelay"), 0f, 5f, new GUIContent("FX Delay"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsIdleFXLoop"), new GUIContent("FX Loop"), true);

        drawUILine();

        SerializedProperty enableMouseOver = serializedObject.FindProperty("EnableMouseOver");
        enableMouseOver.boolValue = EditorGUILayout.BeginToggleGroup("Mouse Over", enableMouseOver.boolValue);
        if (enableMouseOver.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MouseOverAnim"), new GUIContent("Animation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MouseOverFX"), new GUIContent("FX"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsPreLoadMouseOverFX"), new GUIContent("Preload FX"), true);
            EditorGUILayout.Slider(serializedObject.FindProperty("MouseOverFXDelay"), 0f, 5f, new GUIContent("FX Delay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsMouseOverFXLoop"), new GUIContent("FX Loop"), true);
            EditorGUILayout.Slider(serializedObject.FindProperty("MouseOverFadeOut"), 0f, 5f, new GUIContent("Fade-Out"));
        }
        EditorGUILayout.EndToggleGroup();

        drawUILine();

        SerializedProperty enableClick = serializedObject.FindProperty("EnableClick");
        enableClick.boolValue = EditorGUILayout.BeginToggleGroup("Click", enableClick.boolValue);
        if (enableClick.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ClickedAnim"), new GUIContent("Animation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ClickedFX"), new GUIContent("FX"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsPreLoadClickedFX"), new GUIContent("Preload FX"), true);
            EditorGUILayout.Slider(serializedObject.FindProperty("ClickedFXDelay"), 0f, 5f, new GUIContent("FX Delay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsClickedFXLoop"), new GUIContent("FX Loop"), true);
        }
        EditorGUILayout.EndToggleGroup();

        drawUILine();

        SerializedProperty enableCloseBack = serializedObject.FindProperty("EnableCloseBack");
        enableCloseBack.boolValue = EditorGUILayout.BeginToggleGroup("Close Back", enableCloseBack.boolValue);
        if (enableCloseBack.boolValue)
        {
            SerializedProperty clickToCloseBack = serializedObject.FindProperty("ClickToCloseBack");
            SerializedProperty autoCloseBack = serializedObject.FindProperty("AutoCloseBack");
            clickToCloseBack.boolValue = EditorGUILayout.Toggle("Click To Close", !autoCloseBack.boolValue);
            autoCloseBack.boolValue = EditorGUILayout.Toggle("Auto Close", !clickToCloseBack.boolValue);
            EditorGUILayout.Slider(serializedObject.FindProperty("CloseBackDelay"), 0f, 5f, new GUIContent("Auto Close Delay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("CloseBackFX"), new GUIContent("FX"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsPreLoadCloseBackFX"), new GUIContent("Preload FX"), true);
            EditorGUILayout.Slider(serializedObject.FindProperty("CloseBackFXDelay"), 0f, 5f, new GUIContent("FX Delay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IsCloseBackFXLoop"), new GUIContent("FX Loop"), true);
        }
        EditorGUILayout.EndToggleGroup();

        drawUILine();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("ContainedItems"), new GUIContent("Contained Items"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsPreloadContainedItems"), new GUIContent("Preload Contained Items"), true);

        serializedObject.ApplyModifiedProperties();
    }

    private void drawUILine()
    {
        EditorGUILayout.Space();
        rect = EditorGUILayout.BeginHorizontal();
        Handles.color = Color.gray;
        Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
}