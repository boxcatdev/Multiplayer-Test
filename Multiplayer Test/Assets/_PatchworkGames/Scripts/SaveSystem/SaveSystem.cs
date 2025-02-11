using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace PatchworkGames
{
    public class SaveSystem
    {
        #region Game Data
        public static void SaveGame(GameData gameData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(GetGameSavePath(), FileMode.Create);
            bf.Serialize(fs, gameData);
            fs.Close();

            Debug.Log("Saving game...");
        }
        public static GameData LoadGame()
        {
            if (!File.Exists(GetGameSavePath()))
            {
                return null;
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(GetGameSavePath(), FileMode.Open);

                GameData data = bf.Deserialize(fs) as GameData;

                return data;
            }
        }
        private static string GetGameSavePath()
        {
            return Application.persistentDataPath + "/gameData.sgd";
        }
        #endregion
    }
}

