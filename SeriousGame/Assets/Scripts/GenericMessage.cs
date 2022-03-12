using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GenericMessage : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;

    ActionCode action;

    public void Close() {
        TimeManager.Resume();
        if (action == ActionCode.BYPASS_LOADING) {
            IOUtils.load = false;
            SceneLoader.ReloadScene();
        } else {
            Destroy(gameObject);
        }
    }

    public void Load(string message, ActionCode action) {
        TimeManager.Pause();
        this.action = action;
        messageText.SetText(message);
    }
}
