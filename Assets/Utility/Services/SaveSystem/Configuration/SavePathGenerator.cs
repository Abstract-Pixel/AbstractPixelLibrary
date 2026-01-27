using System.IO;
using UnityEngine;

namespace AbstractPixel.Utility
{
    public static class SavePathGenerator
    {
        private static string Root => Path.Combine(Application.persistentDataPath, "SaveFiles");

        // 1. GLOBAL PATHS
        public static string GetGlobalPath(string filename)
        {
            // Returns: .../SaveFiles/Global/settings.json
            return Path.Combine(Root, "Global", filename);
        }

        // 2. PROFILE PATHS
        public static string GetProfileRoot(string profileId)
        {
            // Returns: .../SaveFiles/GameSaves/Profiles/Warrior_Lv50/
            return Path.Combine(Root, "GameSaves", "Profiles", profileId);
        }

        public static string GetProfileBackupPath(string profileId)
        {
            // Returns: .../SaveFiles/GameSaves/Profiles/Warrior_Lv50/Backups/
            return Path.Combine(GetProfileRoot(profileId), "Backups");
        }

        // 3. AUTOSAVE PATHS
        public static string GetAutoSavePath(string profileId)
        {
            // Returns: .../SaveFiles/GameSaves/AutoSave/Warrior_Lv50/
            return Path.Combine(Root, "GameSaves", "AutoSave", profileId);
        }
    }
}
