using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Message : MonoBehaviour {
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;

    ActionCode action;

    /**
     * <summary>Populate the message with the text to show</summary>
     */
    public void Load(string message, ActionCode action, Person p) {
        nameText.SetText(p.name.ToLower());
        image.sprite = p.sprite;
        this.action = action;
        TimeManager.Pause();
        messageText.SetText(message);
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the pop-up window</summary>
     */
    public void CloseButton() {
        switch (action) {
            case ActionCode.CONTINUE:
                gameObject.SetActive(false);
                break;
            case ActionCode.GAME_OVER:
                gameObject.SetActive(false);
                gameObject.transform.parent.GetComponent<GameManager>().PrintFinalReport();
                break;
            default:
                gameObject.SetActive(false);
                break;
        }
        TimeManager.Resume();
    }
}
