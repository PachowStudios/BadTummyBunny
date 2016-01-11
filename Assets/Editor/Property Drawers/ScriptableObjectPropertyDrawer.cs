using System;
using System.IO;
using System.Linq;
using System.Linq.Extensions;
using UnityEditor;
using UnityEngine;

namespace PachowStudios.BadTummyBunny.Editor
{
  [CustomPropertyDrawer(typeof(ScriptableObject), true)]
  public class ScriptableObjectPropertyDrawer : PropertyDrawer
  {
    private SerializedProperty Property { get; set; }

    private Type[] PossibleTypes
    {
      get
      {
        return fieldInfo.FieldType == typeof(ScriptableObject)
          ? new [] { typeof(ScriptableObject) }
          : AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(fieldInfo.FieldType.IsAssignableFrom)
            .ToArray();
      }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      Property = property;
      EditorGUI.PropertyField(position, property, label);

      var clickEvent = Event.current;

      if (clickEvent.type == EventType.MouseDown
          && clickEvent.button == 1
          && position.Contains(clickEvent.mousePosition))
      {
        clickEvent.Use();
        EditorUtility.DisplayCustomMenu(
          new Rect(clickEvent.mousePosition, Vector2.zero),
          PossibleTypes.Select(t => new GUIContent($"Create {t.Name} Asset")).ToArray(),
          -1, OnCreateAssetClick, null);
      }
    }

    private void OnCreateAssetClick(object userData, string[] options, int selected)
    {
      if (selected >= 0)
        CreateAsset(PossibleTypes[selected]);
    }

    private void CreateAsset(Type type)
    {
      var fileName = type.ToString().Split('.').Last();
      var asset = ScriptableObject.CreateInstance(type);
      var path = AssetDatabase.GetAssetPath(Selection.activeObject);

      path = path.IsNullOrEmpty()
        ? "Assets/Resources"
        : Path.GetDirectoryName(path);

      path = AssetDatabase.GenerateUniqueAssetPath($"{path}/{fileName}.asset");
      AssetDatabase.CreateAsset(asset, path);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      EditorUtility.FocusProjectWindow();
      Selection.activeObject = asset;
    }
  }
}