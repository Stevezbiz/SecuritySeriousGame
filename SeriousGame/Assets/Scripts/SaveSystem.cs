using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem {
    public static bool load = false;

    public static void SaveGame(GameSave gameSave) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedata.data";
        FileStream fs = new FileStream(path, FileMode.Create);
        formatter.Serialize(fs, gameSave);
        fs.Close();
    }

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
