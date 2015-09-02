using UnityEditor;
using UnityEngine;

public static class EditorExtension
{
	public static int DrawBitMaskField(Rect rect, int mask, System.Type type, GUIContent label)
	{
		var itemNames = System.Enum.GetNames(type);
		var itemValues = System.Enum.GetValues(type) as int[];

		var val = mask;
		var maskVal = 0;
		for (var i = 0; i < itemValues.Length; i++)
		{
			if (itemValues[i] != 0)
			{
				if ((val & itemValues[i]) == itemValues[i])
					maskVal |= 1 << i;
			}
			else if (val == 0)
			{
				maskVal |= 1 << i;
			}
		}

		var newMaskVal = EditorGUI.MaskField(rect, label, maskVal, itemNames);
		var changes = maskVal ^ newMaskVal;

		for (var i = 0; i < itemValues.Length; i++)
		{
			if ((changes & (1 << i)) != 0)
			{
				if ((newMaskVal & (1 << i)) != 0)
				{
					if (itemValues[i] == 0)
					{
						val = 0;
						break;
					}
					else
						val |= itemValues[i];
				}
				else
				{
					val &= ~itemValues[i];
				}
			}
		}

		return val;
	}
}


[CustomPropertyDrawer(typeof(BitMaskAttribute))]
public class EnumBitMaskPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		label.text = label.text;
		prop.intValue = EditorExtension.DrawBitMaskField(position, prop.intValue, fieldInfo.FieldType, label);
	}
}