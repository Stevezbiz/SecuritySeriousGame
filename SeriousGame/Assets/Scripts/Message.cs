using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Message : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;

    float oldTimeScale;
    ActionCode action;

    /**
     * <summary>Populate the message with the text to show</summary>
     */
    public void Load(string message, ActionCode action, Person p) {
        nameText.SetText(p.name.ToLower());
        image.sprite = p.sprite;
        oldTimeScale = Time.timeScale;
        this.action = action;
        Time.timeScale = 0;
        messageText.SetText(message);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the pop-up window</summary>
     */
    public void CloseButton() {
        switch (action) {
            case ActionCode.CONTINUE:
                Time.timeScale = oldTimeScale;
                gameObject.SetActive(false);
                break;
            case ActionCode.GAME_OVER:
                Time.timeScale = 0f;
                gameObject.SetActive(false);
                gameManager.PrintFinalReport();
                break;
            default:
                Time.timeScale = oldTimeScale;
                gameObject.SetActive(false);
                break;
        }
    }
}
