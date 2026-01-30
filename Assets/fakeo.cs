using AbstractPixel.Utility;
using AbstractPixel.Utility.Save;
using System.Collections;
using UnityEngine;

public class fakeo : MonoBehaviour, ISaveable<Vector3Data>
{
    [SerializeField] Vector3 position;
    public string Guid { get; set; }
    public SaveCategory saveCategory { get; set; }

    IEnumerator  Start()
    {
        yield return new WaitForSeconds(1f);
        SaveManager.Instance.LoadALL();
    }

    public Vector3Data CaptureData()
    {
        Vector3Data data = position;
        return data;
    }

    public void RestoreData(Vector3Data _loadedData)
    {
        // Vector3Data data = SaveDataConverter.Convert<Vector3Data>(deserialisedData);
        //position = data;  
    }
}
