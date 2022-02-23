using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class PersonController : MonoBehaviour {
    GameManager gameManager;
    float speed;
    bool arrived;
    
    void FixedUpdate() {
        if (!arrived) {
            if (transform.localPosition.x < 700) transform.Translate(Vector3.right * speed * Time.deltaTime);
            else {
                arrived = true;
                // trigger quiz
                gameManager.LaunchQuiz();
            }
        } else {
            if (transform.localPosition.x > 0) transform.Translate(Vector3.left * speed * Time.deltaTime);
            else {
                // delete object
                Destroy(gameObject);
            }
        }
    }

    public void Load(GameManager gameManager, Person p) {
        this.gameManager = gameManager;
        gameObject.GetComponent<Image>().sprite = p.sprite;
        speed = 150f;
        arrived = false;
    }
}
