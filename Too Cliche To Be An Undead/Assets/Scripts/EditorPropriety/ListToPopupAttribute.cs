using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ListToPopupAttribute : PropertyAttribute
{
    public Type propertyType;
    public string propertyName;

    public ListToPopupAttribute(Type _propertyType, string _propertyName)
    {
        this.propertyType = _propertyType;
        this.propertyName = _propertyName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ListToPopupAttribute))]
public class ListToPopupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ListToPopupAttribute popup = attribute as ListToPopupAttribute;
        List<string> stringList = null;

        if (popup.propertyType.GetField(popup.propertyName) != null)
        {
            stringList = popup.propertyType.GetField(popup.propertyName).GetValue(popup.propertyType) as List<string>;
        }

        if (stringList != null && stringList.Count != 0)
        {
            int selectedIndex = Mathf.Max(stringList.IndexOf(property.stringValue), 0);
            selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, stringList.ToArray());
            property.stringValue = stringList[selectedIndex];
        }
        else EditorGUI.PropertyField(position, property, label);
    }
}

#endif