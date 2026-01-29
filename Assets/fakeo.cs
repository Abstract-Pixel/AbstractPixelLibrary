using AbstractPixel.Utility;
using AbstractPixel.Utility.Save;
using System.Collections;
using UnityEngine;

public class fakeo : MonoBehaviour, ISaveable
{
    [SerializeField] Vector3 position;
    public string Guid { get; set; }
    public SaveCategory saveCategory { get; set; }

    IEnumerator  Start()
    {
        yield return new WaitForSeconds(1f);
        SaveManager.Instance.SaveALL();
    }

    public object CaptureData()
    {
        Vector3Data data = position;
        return data;
    }

    public void RestoreData(object deserialisedData)
    {
        Vector3Data data = (Vector3Data)deserialisedData;
        position = data;


        
    }
}
