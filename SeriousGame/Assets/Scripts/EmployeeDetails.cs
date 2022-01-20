using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class EmployeeDetails : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI moneyGainText;
    [SerializeField] Image networkBar;
    [SerializeField] Image accessBar;
    [SerializeField] Image softwareBar;
    [SerializeField] Image assetBar;
    [SerializeField] Image servicesBar;
    [SerializeField] TextMeshProUGUI taskText;

    GameManager gameManager;

    public void Load(EmployeeInfo e, GameManager gameManager) {
        this.gameManager = gameManager;
        titleText.SetText(e.name);
        descriptionText.SetText(e.description);
        Dictionary<Category, float> abilities = EmployeeUtils.GetAbilities(e.abilities);
        networkBar.fillAmount = abilities[Category.NETWORK] / 10;
        accessBar.fillAmount = abilities[Category.ACCESS] / 10;
        softwareBar.fillAmount = abilities[Category.SOFTWARE] / 10;
        assetBar.fillAmount = abilities[Category.ASSET] / 10;
        servicesBar.fillAmount = abilities[Category.SERVICES] / 10;
        moneyGainText.SetText("Guadagno: " + e.GetMoneyGain() + " F/h");
        switch (e.status) {
            case TaskType.NONE:
                taskText.SetText("");
                moneyGainText.color = COLOR.BLUE;
                break;
            case TaskType.INSTALL:
                taskText.SetText("Sta installando " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                moneyGainText.color = COLOR.YELLOW;
                break;
            case TaskType.REPAIR:
                taskText.SetText("Sta riparando i danni provocati dall'attacco " + gameManager.GetAttack((AttackCode)gameManager.GetTaskTarget(e.id)).name);
                moneyGainText.color = COLOR.YELLOW;
                break;
            case TaskType.UPGRADE:
                taskText.SetText("Sta installando " + gameManager.GetShopItem((ShopItemCode)gameManager.GetTaskTarget(e.id)).name);
                moneyGainText.color = COLOR.YELLOW;
                break;
            case TaskType.PREVENT:
                string category;
                switch ((Category)gameManager.GetTaskTarget(e.id)) {
                    case Category.NETWORK:
                        category = "Rete";
                        break;
                    case Category.ACCESS:
                        category = "Accesso";
                        break;
                    case Category.SOFTWARE:
                        category = "Software";
                        break;
                    case Category.ASSET:
                        category = "Risorse";
                        break;
                    case Category.SERVICES:
                        category = "Servizi";
                        break;
                    default:
                        Debug.Log("Error: undefined Category");
                        category = "";
                        break;
                }
                taskText.SetText("Sta facendo prevenzione nel settore " + category);
                moneyGainText.color = COLOR.YELLOW;
                break;
            default:
                Debug.Log("Error: undefined TaskType");
                break;
        }
    }

    public void Close() {
        Destroy(gameObject);
    }
}
