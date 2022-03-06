using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour {
    [SerializeField] GameObject loginView;
    [SerializeField] GameObject registerView;
    [SerializeField] TMP_InputField loginUsername;
    [SerializeField] TMP_InputField loginPassword;
    [SerializeField] GameObject loginErrorMessage;
    [SerializeField] TMP_InputField registerUsername;
    [SerializeField] TMP_InputField registerPassword;
    [SerializeField] TMP_InputField registerPasswordConfirm;
    [SerializeField] GameObject registerErrorMessage;
    [SerializeField] TextMeshProUGUI registerErrorMessageText;

    // Start is called before the first frame update
    void Start() {
        if (!Directory.Exists(IOUtils.playersDirPath)) {
            // create storge structures
            Directory.CreateDirectory(IOUtils.playersDirPath);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(IOUtils.playersFilePath, FileMode.Create);
            formatter.Serialize(fs, new PlayerList());
            fs.Close();
        }
        if (!Directory.Exists(IOUtils.gameDataDirPath)) {
            // create storge structures
            Directory.CreateDirectory(IOUtils.gameDataDirPath);
        }
    }

    public void Login() {
        string username = loginUsername.text;
        string password = loginPassword.text;
        string path = IOUtils.GetPlayerDirPath(username);
        
        if(username == "" || password == "") {
            // void fields
            loginErrorMessage.SetActive(true);
            return;
        }
        // verify that the user is present
        if (!Directory.Exists(path)) {
            // player not present
            loginErrorMessage.SetActive(true);
            return;
        }
        // player registered, verify the password
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(IOUtils.playersFilePath, FileMode.Open);
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
            SaveSystem.player = username;
            SceneLoader.LoadScene("MainMenu");
        } else {
            // login failed
            loginErrorMessage.SetActive(true);
            return;
        }
    }

    public void Register() {
        string username = registerUsername.text;
        string password = registerPassword.text;
        string passwordConfirm = registerPasswordConfirm.text;
        string path = IOUtils.GetPlayerDirPath(username);

        if (username == "" || password == "" || passwordConfirm == "") {
            // void fields
            registerErrorMessageText.SetText("Completa tutti i campi");
            registerErrorMessage.SetActive(true);
            return;
        }
        if (Directory.Exists(path)) {
            // player already exists
            registerErrorMessageText.SetText("Username non disponibile");
            registerErrorMessage.SetActive(true);
            return;
        }
        if (password.Length < 8) {
            // short password
            registerErrorMessageText.SetText("Password troppo breve: deve essere lunga almeno 8 caratteri");
            registerErrorMessage.SetActive(true);
            return;
        }
        if (password != passwordConfirm) {
            // different passwords
            registerErrorMessageText.SetText("Le password inserite non coincidono");
            registerErrorMessage.SetActive(true);
            return;
        }
        // add the new player
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(IOUtils.playersFilePath, FileMode.Open);
        PlayerList playerList = formatter.Deserialize(fs) as PlayerList;
        fs.Close();
        playerList.list.Add(new PlayerData(username, password));
        fs = new FileStream(IOUtils.playersFilePath, FileMode.Open);
        formatter.Serialize(fs, playerList);
        fs.Close();
        Directory.CreateDirectory(path);
        registerErrorMessageText.SetText("Registrazione completata: benvenuto/a " + username);
        registerErrorMessage.SetActive(true);
        OpenLoginView();
        loginUsername.text = username;
        loginPassword.text = password;
    }

    public void OpenLoginView() {
        loginUsername.text = "";
        loginPassword.text = "";
        loginView.SetActive(true);
        registerView.SetActive(false);
    }

    public void OpenRegisterView() {
        registerUsername.text = "";
        registerPassword.text = "";
        registerPasswordConfirm.text = "";
        loginView.SetActive(false);
        registerView.SetActive(true);
    }

    public void CloseMessage() {
        loginErrorMessage.SetActive(false);
        registerErrorMessage.SetActive(false);
    }
}
