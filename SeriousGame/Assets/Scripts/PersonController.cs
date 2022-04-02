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
