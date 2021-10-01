using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem {
    public static bool load = false;

    /**
     * <summary>Save the game data on a flie</summary>
     */
    public static void SaveGame(GameSave gameSave) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedata.data";
        FileStream fs = new FileStream(path, FileMode.Create);
        formatter.Serialize(fs, gameSave);
        fs.Close();
    }

    /**
     * <summary>Load the game data from a file</summary>
     */
    public static GameSave LoadGame() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedata.data";
        FileStream fs = new FileStream(path, FileMode.Open);
        GameSave gameSave = formatter.Deserialize(fs) as GameSave;
        fs.Close();
        load = false;
        return gameSave;
    }
}
