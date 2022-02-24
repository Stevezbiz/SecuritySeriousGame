using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PersonController : MonoBehaviour {
    GameManager gameManager;
    float speed;
    bool arrived;
    bool up;
    
    void FixedUpdate() {
        if (!arrived) {
            WalkRight();
            if (transform.localPosition.x > 900) {
                arrived = true;
                // trigger quiz
                gameManager.LaunchQuiz();
            }
        } else {
            WalkLeft();
            if (transform.localPosition.x < 0) {
                // delete object
                Destroy(gameObject);
            }
        }
    }

    void WalkRight() {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        MoveVertically();
    }

    void WalkLeft() {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        MoveVertically();
    }

    void MoveVertically() {
        if (up) {
            transform.Translate(Vector3.up * (speed * 0.5f) * Time.deltaTime);
            if (transform.localPosition.y > 20) up = false;
        } else {
            transform.Translate(Vector3.down * (speed * 0.5f) * Time.deltaTime);
            if (transform.localPosition.y < -20) up = true;
        }
    }

    public void Load(GameManager gameManager, Person p) {
        this.gameManager = gameManager;
        gameObject.GetComponent<Image>().sprite = p.figure;
        speed = 150f;
        arrived = false;
        up = false;
    }
}
