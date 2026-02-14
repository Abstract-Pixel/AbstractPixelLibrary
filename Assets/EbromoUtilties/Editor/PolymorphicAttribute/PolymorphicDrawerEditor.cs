using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine;


[CustomPropertyDrawer(typeof(PolymorphicAttribute),true)]
public class PolymorphicDrawerEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
    }
    public override VisualElement CreatePropertyGUI(SerializedProperty _property)
    {
        Debug.Log($"Drawer running for: {_property.name}");
        VisualElement root = new VisualElement();
        Label label = new Label("Please show somthing");
        root.Add(label);
        Type propertyType = PolymorphicTypeUtility.GetPropertyTypeFromManagedReference(_property);
        List<Type> types = PolymorphicTypeUtility.GetPropertyCompatibleTypes(_property);
        List<string> typeNames = types.Select(t => t.Name).ToList();

        int defaultIndex = 0;
        if (propertyType != null)
        {
            if (typeNames.Contains(propertyType.Name))
            {
                defaultIndex = typeNames.IndexOf(propertyType.Name);
            }
        }

        DropdownField typesDropDown = new DropdownField("Type", typeNames, defaultIndex);
        root.Add(typesDropDown);
        return root;
    }

}

