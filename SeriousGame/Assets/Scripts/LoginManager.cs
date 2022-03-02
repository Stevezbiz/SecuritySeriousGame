using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoginManager : MonoBehaviour {

    // Start is called before the first frame update
    void Start() {
        if (!Directory.Exists(IOUtils.playersDirPath)) {
            // create storge structures
            Directory.CreateDirectory(IOUtils.playersDirPath);
        }
        if (!Directory.Exists(IOUtils.gameDataDirPath)) {
            // create storge structures
            Directory.CreateDirectory(IOUtils.gameDataDirPath);
        }
    }

    public void Login() {
        string username = "";
        string password = "";
        string path = IOUtils.GetPlayerDirPath(username);
        // verify that the user is present
        if (!Directory.Exists(path)) {
            // player not present
        } else {
            // player registered, verify the password
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            PlayerList playerList = formatter.Deserialize(fs) as PlayerList;
            fs.Close();
            bool correct = false;
            foreach (PlayerData p in playerList.list) {
                if (p.password == password) {
                    correct = true;
                    break;
                }
            }
            if (correct) {
                // login successful
            } else {
                // login failed
            }
        }
    }

    public void Register() {
        string username = "";
        string password = "";
        string path = IOUtils.GetPlayerDirPath(username);
        // verify that the user is not present
        if (Directory.Exists(path)) {
            // player already exists
        } else {
            // add the new player
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);
            PlayerList playerList = formatter.Deserialize(fs) as PlayerList;
            fs.Close();
            playerList.list.Add(new PlayerData(username, password));
            fs = new FileStream(path, FileMode.Open);
            formatter.Serialize(fs, playerList);
            fs.Close();
            Directory.CreateDirectory(path);
        }
    }
}
