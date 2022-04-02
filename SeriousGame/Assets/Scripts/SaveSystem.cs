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
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityWebRequest = UnityEngine.Networking.UnityWebRequest;

public class SaveSystem : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject waitingMessage;
    [SerializeField] GameObject errorMessage;

    /**
     * <summary>Save the game data on a file</summary>
     */
    public void SaveGame(GameSave gameSave, ModelSave modelSave) {
        StartCoroutine(SaveGameRoutine(gameSave, modelSave));
    }

    /**
     * <summary>Load the game data from a file</summary>
     */
    public void LoadGame() {
        StartCoroutine(LoadGameRoutine());
    }

    /**
     * <summary></summary>
     */
    IEnumerator LoadGameRoutine() {
        WWWForm form = new WWWForm();
        form.AddField("mode", "r");
        form.AddField("saveFile", IOUtils.GetPlayerGameSavePath());
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.saveOrLoadGameScript, form);
        GenericMessage message = Instantiate(waitingMessage, gameObject.transform, false).GetComponent<GenericMessage>();
        message.Load("Caricamento in corso...", ActionCode.CONTINUE);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log(www.error);
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva", ActionCode.BYPASS_LOADING);
            message.Close();
        } else if (www.downloadHandler.text == "Error Loading File") {
            Debug.Log("Error Loading File");
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Errore: impossibile caricare il salvataggio", ActionCode.BYPASS_LOADING);
            message.Close();
        } else if (www.downloadHandler.text == "Save Not Present") {
            Debug.Log("Save Not Present");
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Non è presente nessun salvataggio", ActionCode.BYPASS_LOADING);
            message.Close();
        } else {
            // success
            gameManager.LoadGameData(JsonUtility.FromJson<GameSave>(www.downloadHandler.text));
            StartCoroutine(LoadModelRoutine(message));
        }
    }

    /**
     * <summary></summary>
     */
    IEnumerator LoadModelRoutine(GenericMessage message) {
        WWWForm form = new WWWForm();
        form.AddField("mode", "r");
        form.AddField("saveFile", IOUtils.GetPlayerModelSavePath());
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.saveOrLoadGameScript, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log(www.error);
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva", ActionCode.BYPASS_LOADING);
            message.Close();
        } else if (www.downloadHandler.text == "Error Loading File") {
            Debug.Log("Error Loading File");
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Errore: impossibile caricare il salvataggio", ActionCode.BYPASS_LOADING);
            message.Close();
        } else if (www.downloadHandler.text == "Save Not Present") {
            Debug.Log("Save Not Present");
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Non è presente nessun salvataggio", ActionCode.BYPASS_LOADING);
            message.Close();
        } else {
            // success
            BKTModel.LoadModel(JsonUtility.FromJson<ModelSave>(www.downloadHandler.text));
            message.Close();
        }
    }

    /**
     * <summary></summary>
     */
    IEnumerator SaveGameRoutine(GameSave gameSave, ModelSave modelSave) {
        WWWForm form = new WWWForm();
        form.AddField("mode", "w");
        form.AddField("saveFile", IOUtils.GetPlayerGameSavePath());
        form.AddField("gameSave", JsonUtility.ToJson(gameSave));
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.saveOrLoadGameScript, form);
        GenericMessage message = Instantiate(waitingMessage, gameObject.transform, false).GetComponent<GenericMessage>();
        message.Load("Salvataggio in corso...", ActionCode.CONTINUE);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log(www.error);
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva", ActionCode.CONTINUE);
            message.Close();
        } else if (www.downloadHandler.text == "Error Saving File") {
            Debug.Log("Error Saving File");
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Errore: impossibile effettuare il salvataggio", ActionCode.CONTINUE);
            message.Close();
        } else {
            // success
            Debug.Log(www.downloadHandler.text);
            StartCoroutine(SaveModelRoutine(modelSave, message));
        }
    }

    /**
     * <summary></summary>
     */
    IEnumerator SaveModelRoutine(ModelSave modelSave, GenericMessage message) {
        WWWForm form = new WWWForm();
        form.AddField("mode", "w");
        form.AddField("saveFile", IOUtils.GetPlayerModelSavePath());
        form.AddField("modelSave", JsonUtility.ToJson(modelSave));
        using UnityWebRequest www = UnityWebRequest.Post(IOUtils.saveOrLoadModelScript, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            Debug.Log(www.error);
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Comunicazione con il server fallita: controlla che la tua connessione ad Internet sia attiva", ActionCode.CONTINUE);
            message.Close();
        } else if (www.downloadHandler.text == "Error Saving File") {
            Debug.Log("Error Saving File");
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Errore: impossibile effettuare il salvataggio", ActionCode.CONTINUE);
            message.Close();
        } else {
            // success
            Debug.Log(www.downloadHandler.text);
            message.Close();
            GenericMessage eMessage = Instantiate(errorMessage, gameObject.transform, false).GetComponent<GenericMessage>();
            eMessage.Load("Salvataggio completato", ActionCode.CONTINUE);
        }
    }
}
