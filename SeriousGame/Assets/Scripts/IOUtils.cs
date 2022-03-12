using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    public string username;
    public byte[] password;
    public byte[] salt;

    public PlayerData(string username, byte[] password, byte[] salt) {
        this.username = username;
        this.password = password;
        this.salt = salt;
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
#if UNITY_EDITOR
    public static string rootPath = "http://localhost/fintech_tycoon";
#else
    public static string rootPath = Application.absoluteURL;
#endif
    public static string dataFolder = "Data";
    public static string playersFile = Path.Combine(new string[] { dataFolder, "players.json" });
    public static string scriptsFolder = Path.Combine(new string[] { rootPath, "WebScripts" });
    public static string LoginOrRegisterScript = Path.Combine(new string[] { scriptsFolder, "loginOrRegister.php" });
    public static string createDataFolderScript = Path.Combine(new string[] { scriptsFolder, "createDataFolder.php" });
    public static string createPlayerFolderScript = Path.Combine(new string[] { scriptsFolder, "createPlayerFolder.php" });

    public static string GetPlayerFolder(string player) {
        return Path.Combine(new string[] { dataFolder, player });
    }

    public static string GetPlayerGameSavePath(string player) {
        return Path.Combine(new string[] { rootPath, player, "game_save.data" });
    }

    public static string GetPlayerModelSavePath(string player) {
        return Path.Combine(new string[] { rootPath, player, "model_save.data" });
    }
}
