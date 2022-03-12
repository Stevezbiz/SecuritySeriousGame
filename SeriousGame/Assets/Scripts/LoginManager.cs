using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Crypto = System.Security.Cryptography;
using UnityWebRequest = UnityEngine.Networking.UnityWebRequest;

public class LoginManager : MonoBehaviour {
    [SerializeField] GameObject loginView;
    [SerializeField] GameObject registerView;
    [SerializeField] TMP_InputField loginUsername;
    [SerializeField] TMP_InputField loginPassword;
    [SerializeField] TMP_InputField registerUsername;
    [SerializeField] TMP_InputField registerPassword;
    [SerializeField] TMP_InputField registerPasswordConfirm;
    [SerializeField] GameObject errorMessage;
    [SerializeField] TextMeshProUGUI errorMessageText;

    readonly Crypto.RNGCryptoServiceProvider rng = new Crypto.RNGCryptoServiceProvider();

    // Start is called before the first frame update
    void Start() {
        Debug.Log("Address: " + IOUtils.rootPath);
        // initialize file structure
        StartCoroutine(CreateMainDataFolder());
    }

    IEnumerator CreateMainDataFolder() {
        WWWForm form = new WWWForm();
        form.AddField("dataFolder", IOUtils.dataFolder);
        // send request
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.createDataFolderScript, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            errorMessageText.SetText("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva");
            errorMessage.SetActive(true);
            Debug.Log(www.error);
        } else {
            // check response
            if (www.downloadHandler.text == "Error Creating Folder") {
                Debug.Log("Error Creating Folder");
            } else {
                Debug.Log("Folder Created");
            }
        }
    }

    IEnumerator CreatePlayerFolder(string username, string password) {
        WWWForm form = new WWWForm();
        form.AddField("playerFolder", IOUtils.GetPlayerFolder());
        // send request
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.createPlayerFolderScript, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            errorMessageText.SetText("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva");
            errorMessage.SetActive(true);
            Debug.Log(www.error);
        } else {
            // check response
            if (www.downloadHandler.text == "Error Creating Folder") {
                Debug.Log("Error Creating Folder");
            } else {
                // prepare login
                errorMessageText.SetText("Registrazione completata: benvenuto/a " + username);
                errorMessage.SetActive(true);
                OpenLoginView();
                loginUsername.text = username;
                loginPassword.text = password;
            }
        }
    }

    IEnumerator UpdatePlayerList(PlayerList players, string username, string password) {
        WWWForm form = new WWWForm();
        form.AddField("mode", "w");
        form.AddField("playersFile", IOUtils.playersFile);
        form.AddField("playerList", JsonUtility.ToJson(players));
        // send request
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.LoginOrRegisterScript, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            errorMessageText.SetText("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva");
            errorMessage.SetActive(true);
            Debug.Log(www.error);
        } else {
            // check response
            if (www.downloadHandler.text == "Error Reading File") {
                Debug.Log("Error Updating List");
            } else {
                Debug.Log("Player List Updated");
                StartCoroutine(CreatePlayerFolder(username, password));
            }
        }
    }

    IEnumerator LoginRoutine(string username, string password) {
        WWWForm form = new WWWForm();
        form.AddField("mode", "r");
        form.AddField("playersFile", IOUtils.playersFile);
        // send request
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.LoginOrRegisterScript, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            errorMessageText.SetText("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva");
            errorMessage.SetActive(true);
            Debug.Log(www.error);
        } else {
            // check response
            if (www.downloadHandler.text == "File Created" || www.downloadHandler.text == "File Empty") {
                Debug.Log("Giocatore non registrato");
                errorMessageText.SetText("Le credenziali sono errate, riprova");
                errorMessage.SetActive(true);
            } else {
                PlayerList playerList = JsonUtility.FromJson<PlayerList>(www.downloadHandler.text);
                bool correct = false;
                foreach (PlayerData p in playerList.list) {
                    if (username == p.username) {
                        using (Crypto.SHA256 sha256 = Crypto.SHA256.Create()) {
                            byte[] saltedPassword = p.salt.Concat(System.Text.Encoding.ASCII.GetBytes(password)).ToArray();
                            byte[] hash = sha256.ComputeHash(saltedPassword);
                            if (p.password.SequenceEqual(hash)) correct = true;
                        }
                        if (correct) break;
                    }
                }
                if (correct) {
                    // login successful
                    IOUtils.player = username;
                    SceneLoader.LoadScene("MainMenu");
                } else {
                    // login failed
                    errorMessageText.SetText("Le credenziali sono errate, riprova");
                    errorMessage.SetActive(true);
                }
            }
        }
    }

    IEnumerator RegisterRoutine(string username, string password) {
        PlayerData playerData;
        using (Crypto.SHA256 sha256 = Crypto.SHA256.Create()) {
            byte[] salt = new byte[5];
            rng.GetBytes(salt);
            byte[] saltPassword = salt.Concat(System.Text.Encoding.ASCII.GetBytes(password)).ToArray();
            byte[] hash = sha256.ComputeHash(saltPassword);
            playerData = new PlayerData(username, hash, salt);
        }
        WWWForm form = new WWWForm();
        form.AddField("mode", "r");
        form.AddField("playersFile", IOUtils.playersFile);
        // send request
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.LoginOrRegisterScript, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            errorMessageText.SetText("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva");
            errorMessage.SetActive(true);
            Debug.Log(www.error);
        } else {
            PlayerList playerList = new PlayerList();
            // check response
            if (www.downloadHandler.text == "File Created" || www.downloadHandler.text == "File Empty") {
                playerList.list.Add(playerData);
                StartCoroutine(UpdatePlayerList(playerList, username, password));
            } else {
                // check if username is already registered
                playerList = JsonUtility.FromJson<PlayerList>(www.downloadHandler.text);
                foreach (PlayerData p in playerList.list) {
                    if (username == p.username) {
                        errorMessageText.SetText("Username non disponibile");
                        errorMessage.SetActive(true);
                        yield break;
                    }
                }
                playerList.list.Add(playerData);
                StartCoroutine(UpdatePlayerList(playerList, username, password));
            }
        }
    }

    public void Login() {
        string username = loginUsername.text;
        string password = loginPassword.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            // void fields
            errorMessageText.SetText("Completa tutti i campi");
            errorMessage.SetActive(true);
        } else StartCoroutine(LoginRoutine(username, password));
    }

    public void Register() {
        string username = registerUsername.text;
        string password = registerPassword.text;
        string passwordConfirm = registerPasswordConfirm.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm)) {
            // void fields
            errorMessageText.SetText("Completa tutti i campi");
            errorMessage.SetActive(true);
        } else if (password.Length < 8) {
            // short password
            errorMessageText.SetText("Password troppo breve: deve essere lunga almeno 8 caratteri");
            errorMessage.SetActive(true);
        } else if (password != passwordConfirm) {
            // different passwords
            errorMessageText.SetText("Le password inserite non coincidono");
            errorMessage.SetActive(true);
        } else StartCoroutine(RegisterRoutine(username, password));
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
        errorMessage.SetActive(false);
    }
}
