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

    /**
     * <summary></summary>
     */
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
