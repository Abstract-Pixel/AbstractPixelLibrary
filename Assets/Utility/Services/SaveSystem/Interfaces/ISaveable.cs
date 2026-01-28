using AbstractPixel.Utility.Save;
using UnityEngine;

namespace AbstractPixel.Utility
{
    public  interface ISaveable
    {
        public SaveCategory saveCategory { get; }
        public object CaptureData();
        public void RestoreData(object deserialisedData); 
    }
}
