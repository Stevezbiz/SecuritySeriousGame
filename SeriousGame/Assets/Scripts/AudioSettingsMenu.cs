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
using Outline = UnityEngine.UI.Outline;
using Slider = UnityEngine.UI.Slider;

public class AudioSettingsMenu : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioSource soundtrack;
    [SerializeField] AudioSource effects;
    [SerializeField] Image musicImage;
    [SerializeField] Image effectsImage;
    [SerializeField] Outline musicOutline;
    [SerializeField] Outline effectsOutline;
    [SerializeField] Slider musicBar;
    [SerializeField] Slider effectsBar;

    float sNorm = .3f;
    float eNorm = 1f;

    public void Setup(GameConfig gameConfig) {
        soundtrack.volume = sNorm * gameConfig.musicVolume;
        soundtrack.mute = gameConfig.musicMute;
        effects.volume = eNorm * gameConfig.effectsVolume;
        effects.mute = gameConfig.effectsMute;
    }

    public void OpenAudioSettings() {
        if (soundtrack.mute) {
            musicImage.color = COLOR.GREEN_DISABLED;
            musicOutline.effectColor = COLOR.GREEN_DISABLED;
            musicBar.SetValueWithoutNotify(0f);
            musicBar.interactable = false;
        } else {
            musicImage.color = COLOR.GREEN;
            musicOutline.effectColor = COLOR.GREEN;
            musicBar.SetValueWithoutNotify(soundtrack.volume / sNorm);
            musicBar.interactable = true;
        }
        if (effects.mute) {
            effectsImage.color = COLOR.GREEN_DISABLED;
            effectsOutline.effectColor = COLOR.GREEN_DISABLED;
            effectsBar.SetValueWithoutNotify(0f);
            effectsBar.interactable = false;
        } else {
            effectsImage.color = COLOR.GREEN;
            effectsOutline.effectColor = COLOR.GREEN;
            effectsBar.SetValueWithoutNotify(effects.volume / eNorm);
            effectsBar.interactable = true;
        }
        gameObject.SetActive(true);
    }

    public void CloseAudioSettings() {
        gameManager.SaveAudioSettings(soundtrack.volume / sNorm, soundtrack.mute, effects.volume / eNorm, effects.mute);
        gameObject.SetActive(false);
    }

    public void MuteUnmuteMusic() {
        if (soundtrack.mute) {
            soundtrack.mute = false;
            musicImage.color = COLOR.GREEN;
            musicOutline.effectColor = COLOR.GREEN;
            musicBar.SetValueWithoutNotify(soundtrack.volume / sNorm);
            musicBar.interactable = true;
        } else {
            soundtrack.mute = true;
            musicImage.color = COLOR.GREEN_DISABLED;
            musicOutline.effectColor = COLOR.GREEN_DISABLED;
            musicBar.SetValueWithoutNotify(0f);
            musicBar.interactable = false;
        }
    }

    public void MuteUnmuteEffects() {
        if (effects.mute) {
            effects.mute = false;
            effectsImage.color = COLOR.GREEN;
            effectsOutline.effectColor = COLOR.GREEN;
            effectsBar.SetValueWithoutNotify(effects.volume / eNorm);
            effectsBar.interactable = true;
        } else {
            effects.mute = true;
            effectsImage.color = COLOR.GREEN_DISABLED;
            effectsOutline.effectColor = COLOR.GREEN_DISABLED;
            effectsBar.SetValueWithoutNotify(0f);
            effectsBar.interactable = false;
        }
    }

    public void ChangeMusicVolume() {
        soundtrack.volume = sNorm * musicBar.value;
    }

    public void ChangeEffectsVolume() {
        effects.volume = eNorm * effectsBar.value;
    }
}
