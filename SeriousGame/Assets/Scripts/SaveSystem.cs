using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
    public static bool load = false;
    public static string player = "";

    /**
     * <summary>Save the game data on a file</summary>
     */
    public static void SaveGame(GameSave gameSave) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(IOUtils.GetPlayerGameSavePath(player), FileMode.Create);
        formatter.Serialize(fs, gameSave);
        fs.Close();
    }

    public static void SaveModel(ModelSave modelSave) {
        File.WriteAllText(IOUtils.GetPlayerModelSavePath(player), JsonUtility.ToJson(modelSave));
    }

    /**
     * <summary>Load the game data from a file</summary>
     */
    public static GameSave LoadGame() {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(IOUtils.GetPlayerGameSavePath(player), FileMode.Open);
        GameSave gameSave = formatter.Deserialize(fs) as GameSave;
        fs.Close();
        load = false;
        return gameSave;
    }

    public static ModelSave LoadModel() {
        return JsonUtility.FromJson<ModelSave>(File.ReadAllText(IOUtils.GetPlayerModelSavePath(player)));
    }
}
