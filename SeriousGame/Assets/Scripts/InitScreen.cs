using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InitScreen : MonoBehaviour {
    [SerializeField] TextMeshProUGUI versionText;

    void Start() {
        versionText.SetText("v" + Application.version);
    }

    /**
     * <summary>Load the scene of the main menu</summary>
     */
    public void PlayButton() {
        SceneLoader.LoadScene("LoginMenu");
    }
}
