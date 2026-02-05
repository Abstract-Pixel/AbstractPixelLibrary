using System.Reflection;
using System;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    [Serializable]
    public class SaveableTarget
    {
        [HideInInspector][SerializeField]private string ClassName;
        public MonoBehaviour Script;
        public SaveableIdentification Identification;

        // Runtime Only
        public MethodInfo CaptureDataMethod;
        public MethodInfo RestoreDataMethod;
        public Type DataToSaveType;

        public SaveableTarget(MonoBehaviour _script, SaveableIdentification _identification)
        {
            Script = _script;
            Identification = _identification;
            ClassName = _identification.ClassName;
        }

    }
}
