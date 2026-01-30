using AbstractPixel.Utility;
using AbstractPixel.Utility.Save;
using UnityEngine;

[Saveable(SaveCategory.Game, "Fakeo2")]
public class anotherFako : MonoBehaviour, ISaveable<Vector3Data>
{
    [SerializeField] Vector3 example;
    public Vector3Data CaptureData()
    {
        return example;
    }

    public void RestoreData(Vector3Data _loadedData)
    {
       example = _loadedData;
    }
}
