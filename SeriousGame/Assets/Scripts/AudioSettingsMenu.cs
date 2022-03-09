using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Outline = UnityEngine.UI.Outline;
using Slider = UnityEngine.UI.Slider;

public class AudioSettingsMenu : MonoBehaviour {
    [SerializeField] AudioSource soundtrack;
    //[SerializeField] AudioSource effects;
    [SerializeField] Image musicImage;
    [SerializeField] Image effectsImage;
    [SerializeField] Outline musicOutline;
    [SerializeField] Outline effectsOutline;
    [SerializeField] Slider musicBar;
    [SerializeField] Slider effectsBar;

    public void OpenAudioSettings() {
        if(soundtrack.mute) {
            musicImage.color = COLOR.GREEN_DISABLED;
            musicOutline.effectColor = COLOR.GREEN_DISABLED;
            musicBar.SetValueWithoutNotify(0f);
            musicBar.interactable = false;
        } else {
            musicImage.color = COLOR.GREEN;
            musicOutline.effectColor = COLOR.GREEN;
            musicBar.SetValueWithoutNotify(soundtrack.volume);
            musicBar.interactable = true;
        }
        //if (effects.mute) {
        //    effectsImage.color = COLOR.GREEN_DISABLED;
        //    effectsOutline.effectColor = COLOR.GREEN_DISABLED;
        //    effectsBar.SetValueWithoutNotify(0f);
        //    effectsBar.interactable = false;
        //} else {
        //    effectsImage.color = COLOR.GREEN;
        //    effectsOutline.effectColor = COLOR.GREEN;
        //    effectsBar.SetValueWithoutNotify(effects.volume);
        //    effectsBar.interactable = true;
        //}
        gameObject.SetActive(true);
    }

    public void CloseAudioSettings() {
        gameObject.SetActive(false);
    }

    public void MuteUnmuteMusic() {
        if (soundtrack.mute) {
            soundtrack.mute = false;
            musicImage.color = COLOR.GREEN;
            musicOutline.effectColor = COLOR.GREEN;
            musicBar.SetValueWithoutNotify(soundtrack.volume);
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
        //if (effects.mute) {
        //    effects.mute = false;
        //    effectsImage.color = COLOR.GREEN;
        //    effectsOutline.effectColor = COLOR.GREEN;
        //    effectsBar.SetValueWithoutNotify(effects.volume);
        //    effectsBar.interactable = true;
        //} else {
        //    effects.mute = true;
        //    effectsImage.color = COLOR.GREEN_DISABLED;
        //    effectsOutline.effectColor = COLOR.GREEN_DISABLED;
        //    effectsBar.SetValueWithoutNotify(0f);
        //    effectsBar.interactable = false;
        //}
    }

    public void ChangeMusicVolume() {
        soundtrack.volume = musicBar.value;
    }

    public void ChangeEffectsVolume() {
        //effects.volume = effectsBar.value;
    }
}
