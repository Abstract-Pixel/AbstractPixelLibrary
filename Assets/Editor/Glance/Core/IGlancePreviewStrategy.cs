using UnityEngine;
using UnityEngine.UIElements;

namespace Glance.Core
{
    public interface IGlancePreviewStrategy
    {
        void OnInitialize(Object target);
        void OnDrawUI(VisualElement root);
        void OnCleanup();
    }
}
