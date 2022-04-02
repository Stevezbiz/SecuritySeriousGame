/*
 * Project developed at Politecnico di Torino (2021-2022) by Stefano Gennero
 * in collaboration with prof. Andrea Atzeni and prof. Antonio Lioy.
 * 
 * Copyright 2022 Stefano Gennero
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 *      
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
    public static string saveOrLoadGameScript = Path.Combine(new string[] { scriptsFolder, "saveOrLoadGame.php" });
    public static string saveOrLoadModelScript = Path.Combine(new string[] { scriptsFolder, "saveOrLoadModel.php" });
    public static bool load = false;
    public static bool tutorial = false;
    public static string player = "";

    public static string GetPlayerFolder(string username) {
        return Path.Combine(new string[] { dataFolder, username });
    }

    public static string GetPlayerGameSavePath() {
        return Path.Combine(new string[] { dataFolder, player, "game_save.data" });
    }

    public static string GetPlayerModelSavePath() {
        return Path.Combine(new string[] { dataFolder, player, "model_save.data" });
    }
}
