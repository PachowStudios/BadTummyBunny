using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace PachowStudios.BadTummyBunny
{
  [CustomEditor(typeof(ScaleWidthCamera))]
  public class ScaleWidthCameraEditor : Editor
  {
    private AnimBool showWorldSpaceUI;

    private ScaleWidthCamera Target => (ScaleWidthCamera)target;

    private void OnEnable() => this.showWorldSpaceUI = new AnimBool(Target.useWorldSpaceUI);

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      EditorGUILayout.LabelField("Current FOV", Target.CurrentFOV.ToString());
      EditorGUILayout.Space();

      Target.CurrentFOV = Target.defaultFOV = EditorGUILayout.IntField("Default FOV", Target.defaultFOV);

      this.showWorldSpaceUI.target = EditorGUILayout.Toggle("Use World Space UI", this.showWorldSpaceUI.target);
      Target.useWorldSpaceUI = this.showWorldSpaceUI.value;

      if (EditorGUILayout.BeginFadeGroup(this.showWorldSpaceUI.faded))
      {
        EditorGUI.indentLevel++;

        Target.worldSpaceUI = (RectTransform)EditorGUILayout.ObjectField("World Space UI", Target.worldSpaceUI, typeof(RectTransform), true);

        if (Target.worldSpaceUI == null)
          EditorGUILayout.HelpBox("No world space UI selected!", MessageType.Error);

        EditorGUI.indentLevel--;
      }

      EditorGUILayout.EndFadeGroup();

      if (GUI.changed)
        EditorUtility.SetDirty(Target);

      serializedObject.ApplyModifiedProperties();
      Repaint();
    }
  }
}
