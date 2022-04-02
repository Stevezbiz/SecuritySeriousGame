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

    /**
     * <summary></summary>
     */
    public void Load(string message, string name, Sprite s) {
        messageText.SetText(message);
        nameText.SetText(name.ToLower());
        image.sprite = s;
        StartCoroutine(CountDownToDestroy());
    }

    /**
     * <summary></summary>
     */
    IEnumerator CountDownToDestroy() {
        yield return new WaitForSeconds(seconds);
        fade = true;
    }
}
