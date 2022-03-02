using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ImageBlink : MonoBehaviour {
    Image image;
    float speed = 1f;

    void Awake() {
        image = gameObject.GetComponent<Image>();
    }

    void Update() {
        image.color = Color.Lerp(COLOR.DISABLED, COLOR.RED, Mathf.PingPong(Time.time * speed, 1));
    }
}
