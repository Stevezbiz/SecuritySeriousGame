using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
        Dictionary<ShopItemCategory, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[ShopItemCategory.NETWORK] / 10;
        accessBar.fillAmount = abilities[ShopItemCategory.ACCESS] / 10;
        softwareBar.fillAmount = abilities[ShopItemCategory.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[ShopItemCategory.ASSET] / 10;
        servicesBar.fillAmount = abilities[ShopItemCategory.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.moneyGain + " F/h");
    }

    public void Close() {
        Destroy(gameObject);
    }
}
