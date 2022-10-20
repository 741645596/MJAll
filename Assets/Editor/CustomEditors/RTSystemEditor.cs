// @Author: tanjinhua
// @Date: 2021/11/23  16:45


using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RTSystem), true)]
public class RTSystemEditor : Editor
{
    SerializedProperty _useDynamicTargetTexture;
    SerializedProperty _dynamicTargetTextureSize;
    SerializedProperty _targetTexture;
    SerializedProperty _layerIndex;

    SerializedProperty _cameraSettings;
    SerializedProperty _position;
    SerializedProperty _euler;
    SerializedProperty _backgroundColor;
    SerializedProperty _projectionType;
    SerializedProperty _orthographicSize;
    SerializedProperty _fieldOfView;
    SerializedProperty _near;
    SerializedProperty _far;
    SerializedProperty _lookTarget;
    bool _cameraSettingFold = true;



    private void OnEnable()
    {
        _useDynamicTargetTexture = serializedObject.FindProperty("_useDynamicTargetTexture");
        _dynamicTargetTextureSize = serializedObject.FindProperty("_dynamicTargetTextureSize");
        _targetTexture = serializedObject.FindProperty("_targetTexture");
        _layerIndex = serializedObject.FindProperty("_layerIndex");

        _cameraSettings = serializedObject.FindProperty("_cameraSettings");
        _position = _cameraSettings.FindPropertyRelative("_position");
        _euler = _cameraSettings.FindPropertyRelative("_euler");
        _backgroundColor = _cameraSettings.FindPropertyRelative("_backgroundColor");
        _projectionType = _cameraSettings.FindPropertyRelative("_projectionType");
        _orthographicSize = _cameraSettings.FindPropertyRelative("_orthographicSize");
        _fieldOfView = _cameraSettings.FindPropertyRelative("_fieldOfView");
        _near = _cameraSettings.FindPropertyRelative("_near");
        _far = _cameraSettings.FindPropertyRelative("_far");
        _lookTarget = _cameraSettings.FindPropertyRelative("_lookTarget");


    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDynamicTargetTextureField();

        DrawDynamicTargetTextureSizeField();

        DrawTargetTextureField();
        
        DrawLayerIndexField();

        DrawCameraSettingsField();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDynamicTargetTextureField()
    {
        EditorGUILayout.PropertyField(_useDynamicTargetTexture, new GUIContent("Use Dynamic Texture", "是否使用动态RenderTexture"));
    }

    private void DrawDynamicTargetTextureSizeField()
    {
        if (!_useDynamicTargetTexture.boolValue)
        {
            return;
        }

        EditorGUILayout.PropertyField(_dynamicTargetTextureSize, new GUIContent("Target Texture Size", "动态RenderTexture的尺寸"));
        var v = _dynamicTargetTextureSize.vector2IntValue;
        _dynamicTargetTextureSize.vector2IntValue = new Vector2Int(Mathf.Max(2, v.x), Mathf.Max(2, v.y));
    }

    private void DrawTargetTextureField()
    {
        if (_useDynamicTargetTexture.boolValue)
        {
            return;
        }
        EditorGUILayout.PropertyField(_targetTexture);
    }

    private void DrawLayerIndexField()
    {
        EditorGUILayout.PropertyField(_layerIndex);
    }

    private void DrawCameraSettingsField()
    {
        _cameraSettingFold = EditorGUILayout.Foldout(_cameraSettingFold, new GUIContent("Camera Settings", "摄像机设置项"), true, EditorStyles.foldout);

        if (_cameraSettingFold)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_position, new GUIContent("Local Position"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_euler, new GUIContent("Local Euler Angles"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_backgroundColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_projectionType);
            EditorGUILayout.EndHorizontal();

            if (_projectionType.enumValueIndex == 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_orthographicSize);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_fieldOfView);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_near);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(_far);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(_lookTarget);
            EditorGUILayout.EndHorizontal();
        }
    }
}
