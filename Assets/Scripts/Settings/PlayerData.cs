using System.IO;
using UnityEngine;

namespace Settings
{
    public static class PlayerData
    {
        public static string Path
        {
            get
            {
                string folderPath = Application.dataPath + "/PlayerData/";
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                return folderPath;
            }
        }
    }
}