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

public class SettingsMenu : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject audioSettings;
    [SerializeField] AudioSource audioSource;
    [SerializeField] SaveSystem saveSystem;

    /**
     * <summary>Open the setting menu</summary>
     */
    public void OpenSettings() {
        TimeManager.Pause();
        gameObject.SetActive(true);
    }

    /**
     * <summary>Close the setting menu and resume the game</summary>
     */
    public void CancelButton() {
        TimeManager.Resume();
        gameObject.SetActive(false);
    }

    /**
     * <summary>Save the game data</summary>
     */
    public void SaveButton() {
        saveSystem.SaveGame(gameManager.SaveGame(), gameManager.SaveModel());
    }

    /**
     * <summary>Load a saved game</summary>
     */
    public void LoadGameButton() {
        IOUtils.load = true;
        SceneLoader.ReloadScene();
    }

    /**
     * <summary>Go to the main menu</summary>
     */
    public void ExitButton() {
        SceneLoader.LoadScene("MainMenu");
    }
}
