using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AbstractPixel.Utility
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)attribute;

            VisualElement propertyContainer = new VisualElement();
            propertyContainer.style.flexDirection = FlexDirection.Row;

            PropertyField propertyField = new PropertyField(property);
            propertyField.style.flexGrow = 1;
            propertyField.SetEnabled(false);
            if (!readOnlyAttribute.IsEditable) return propertyContainer;

            Label propertyLockText = new Label("🔒");
            propertyLockText.style.opacity = 0;
            propertyLockText.style.unityTextAlign = TextAnchor.MiddleCenter;
            propertyLockText.style.paddingLeft = 2.5f;
            propertyLockText.style.paddingRight = 2.5f;
            propertyLockText.style.marginLeft = 4f;
            propertyLockText.style.borderBottomLeftRadius = 2.5f;
            propertyLockText.style.borderBottomRightRadius = 2.5f;
            propertyLockText.style.borderTopLeftRadius = 2.5f;
            propertyLockText.style.borderTopRightRadius = 2.5f;
            propertyContainer.Add(propertyField);
            propertyContainer.Add(propertyLockText);

            propertyContainer.RegisterCallback<PointerEnterEvent>(evt =>
            {
                propertyLockText.style.opacity = 1;
                Color bgColor = propertyField.enabledSelf ? new Color(100, 100, 100, 0.25f) : new Color(130, 130, 130, 0.45f);
                propertyLockText.style.backgroundColor = bgColor; // Semi-transparent black background
            });

            propertyContainer.RegisterCallback<PointerLeaveEvent>(evt =>
            {
                propertyLockText.style.opacity = 0;
            });

            propertyLockText.RegisterCallback<PointerDownEvent>(evt =>
            {
                bool isCurrentlyEnabled = propertyField.enabledSelf;
                propertyField.SetEnabled(!isCurrentlyEnabled);
                propertyLockText.text = isCurrentlyEnabled ? "🔒" : "🔓";
                Color bgColor = propertyField.enabledSelf ? new Color(100, 100, 100, 0.25f) : new Color(130, 130, 130, 0.45f);
                propertyLockText.style.backgroundColor = bgColor;
            });
            return propertyContainer;
        }
    }
}
