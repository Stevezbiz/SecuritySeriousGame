using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class NotificationItem : MonoBehaviour {
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image image;

    float seconds = 7f;
    float fadeSpeed = 1f;
    bool fade = false;

    void Update() {
        if (fade) {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, .0f, fadeSpeed * Time.deltaTime);
            if (canvasGroup.alpha <= .001f) Destroy(gameObject);
        }
    }

    public void Load(string message, string name, Sprite s) {
        messageText.SetText(message);
        nameText.SetText(name.ToLower());
        image.sprite = s;
        StartCoroutine(CountDownToDestroy());
    }

    IEnumerator CountDownToDestroy() {
        yield return new WaitForSeconds(seconds);
        fade = true;
    }
}
