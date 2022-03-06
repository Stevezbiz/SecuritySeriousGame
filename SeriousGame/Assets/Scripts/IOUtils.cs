using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    public string username;
    public string password;

    public PlayerData(string username, string password) {
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class PlayerList {
    public List<PlayerData> list;

    public PlayerList() {
        list = new List<PlayerData>();
    }
}

public static class IOUtils {
    public static string rootPath = Application.persistentDataPath;
    public static string playersDirPath = Path.Combine(new string[] { rootPath, "Players" });
    public static string playersFilePath = Path.Combine(new string[] { playersDirPath, "players.data" });
    public static string gameDataDirPath = Path.Combine(new string[] { rootPath, "GameData" });

    public static string GetPlayerDirPath(string player) {
        return Path.Combine(new string[] { gameDataDirPath, player });
    }
}
