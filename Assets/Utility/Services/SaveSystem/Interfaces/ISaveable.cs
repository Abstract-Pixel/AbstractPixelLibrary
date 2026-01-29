using AbstractPixel.Utility.Save;
using UnityEngine;

namespace AbstractPixel.Utility
{
    public  interface ISaveable
    {
        public string Guid { get; set; }
        public SaveCategory saveCategory { get; set; }
        public object CaptureData();
        public void RestoreData(object deserialisedData); 
    }
}
