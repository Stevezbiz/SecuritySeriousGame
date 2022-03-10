using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Message : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;

    GameManager gameManager;
    ActionCode action;

    /**
     * <summary>Populate the message with the text to show</summary>
     */
    public void Load(GameManager gameManager, string message, ActionCode action, Person p) {
        nameText.SetText(p.name.ToLower());
        image.sprite = p.icon;
        this.gameManager = gameManager;
        this.action = action;
        messageText.SetText(message);
        gameObject.SetActive(false);
    }

    public void Show() {
        TimeManager.Pause();
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the pop-up window</summary>
     */
    public void CloseButton() {
        TimeManager.Resume();
        gameManager.CloseMessage();
        switch (action) {
            case ActionCode.CONTINUE:
                Destroy(gameObject);
                break;
            case ActionCode.GAME_OVER:
                gameObject.transform.parent.GetComponent<GameManager>().PrintFinalReport();
                Destroy(gameObject);
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }
}
