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

public class MainMenu : MonoBehaviour {
    [SerializeField] GameObject message;
    [SerializeField] GameObject credits;

    public void OpenQuiz() {
        Application.OpenURL("https://forms.gle/HcSwFLqoxUGMFFf76");
        message.SetActive(false);
    }

    public void Back() {
        message.SetActive(false);
    }

    /**
     * <summary>Create a new game</summary>
     */
    public void NewGameButton() {
        IOUtils.tutorial = true;
        SceneLoader.LoadScene("Game");
    }

    /**
     * <summary>Load a saved game</summary>
     */
    public void LoadGameButton() {
        IOUtils.load = true;
        SceneLoader.LoadScene("Game");
    }

    /**
     * <summary>Close the game</summary>
     */
    public void ExitButton() {
        SceneLoader.ExitGame();
    }

    public void Logout() {
        IOUtils.player = "";
        IOUtils.load = false;
        SceneLoader.LoadScene("LoginMenu");
    }

    public void CloseCredits() {
        credits.SetActive(false);
    }

    public void OpenCredits() {
        credits.SetActive(true);
    }
}
