using AbstractPixel.Utility.Save;
using UnityEngine;

namespace AbstractPixel.Utility
{
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] SaveCategory category;
        [SerializeField] string GUID = string.Empty;
        ISaveable saveable;

        private void OnEnable()
        {
            saveable = GetComponent<ISaveable>();
            saveable.saveCategory = category;
            saveable.Guid = GUID;

            if(saveable != null )
            {
                SaveManager.Instance.RegisterSaveableObject(GUID, saveable);
            }
            
        }

        private void OnDestroy()
        {
            if( saveable != null )
            {
                SaveManager.Instance.UnregisterSaveableObject(GUID);
            }
            
        }

    }
}
