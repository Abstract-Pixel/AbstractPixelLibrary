using System;
using UnityEngine;

namespace AbstractPixel.Utility.Save
{
    [Serializable]
    public class GlobalMetaData
    {
        public static readonly string MetaDataFileName = "GlobalMetaData";
        public string Version;
        public string LastSavedProfileID;
        public string CreationDateAndTime;

        public GlobalMetaData(string _lastSavedProfileID)
        {
            Version = Application.version;
            LastSavedProfileID = _lastSavedProfileID;
            CreationDateAndTime = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
