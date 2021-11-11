using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class EmployeeCard : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;

    public void Load(EmployeeInfo e) {
        titleText.SetText(e.name);
        descriptionText.SetText(e.description);
        Dictionary<Category, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[Category.NETWORK] / 10;
        accessBar.fillAmount = abilities[Category.ACCESS] / 10;
        softwareBar.fillAmount = abilities[Category.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[Category.ASSET] / 10;
        servicesBar.fillAmount = abilities[Category.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.moneyGain + " F/h");
    }

    public void Close() {
        Destroy(gameObject);
    }
}
