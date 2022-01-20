using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetail : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI requirementsText;
    [SerializeField] TextMeshProUGUI resistancesText;
    [SerializeField] TextMeshProUGUI costsText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject enableButton;
    [SerializeField] GameObject disableButton;
    [SerializeField] GameObject installingButton;
    [SerializeField] GameObject installButton;
    [SerializeField] GameObject upgradeButton;
    [SerializeField] GameObject upgradingButton;
    [SerializeField] GameObject lockImage;
    [SerializeField] TextMeshProUGUI purchaseText;
    [SerializeField] Outline purchaseOutline;
    [SerializeField] GameManager gameManager;
    [SerializeField] EmployeeChoice employeeChoice;

    ShopItem parent;
    ShopItemCode id;
    Task task;

    /**
     * <summary>Compose all the details of the item</summary>
     */
    void ComposeDetails(ShopItemInfo sii) {
        titleText.SetText(sii.name + "\n" + sii.cost[sii.level] + " Fondi");
        descriptionText.SetText(sii.description);
        // set the technical details about requirements
        string requirements = "";
        if (sii.locked) {
            requirements += "Per sbloccare questo potenziamento devi prima acquistare:\n";
            foreach (ShopItemCode code in sii.requirements) {
                requirements += "    " + gameManager.GetShopItem(code).name + "\n";
            }
        }
        requirementsText.SetText(requirements);
        // set the technical details about resistances
        string resistances = "";
        resistances += "Resistenze:\n";
        foreach (Resistance res in sii.resistances[sii.level].resistances) {
            resistances += "    " + gameManager.GetAttack(res.id).name + "\n";
            if (res.duration != 0) resistances += "        durata dell'attacco -" + (res.duration * 100) + "%\n";
            if (res.miss != 0) resistances += "        probabilità di bloccare l'attacco +" + (res.miss * 100) + "%\n";
            if (res.endurance != 0) resistances += "        tempo medio tra 2 attacchi consecutivi +" + (res.endurance * 100) + "%\n";
        }
        if (sii.resistances.Length == 0) resistances += "nessuna\n";
        // set the technical details about costs and usability
        if (sii.moneyMalus[sii.level] < 0) resistances += "Guadagno aggiuntivo: " + (-sii.moneyMalus[sii.level]) + " F/h\n";
        if (sii.usersMod[sii.level] > 1) resistances += "Prestazioni e usabilità: +" + ((sii.usersMod[sii.level] - 1) * 100) + "%\n";
        resistancesText.SetText(resistances);
        string costs = "";
        if (sii.moneyMalus[sii.level] < 0) costs += "Costo: 0 F/h\n";
        else costs += "Costo: " + sii.moneyMalus + " F/h\n";
        if (sii.usersMod[sii.level] != 0 && sii.usersMod[sii.level] < 1) costs += "Prestazioni e usabilità: -" + (sii.usersMod[sii.level] * 100) + "%\n";
        costsText.SetText(costs);
    }

    /**
     * <summary>Populate the item of the shop with all the elements to show</summary>
     */
    public void Load(ShopItemCode id, ShopItem parent) {
        this.id = id;
        this.parent = parent;
        ShopItemInfo sii = gameManager.GetShopItem(id);
        ComposeDetails(sii);
        // set the visual aspect
        purchaseButton.SetActive(false);
        enableButton.SetActive(false);
        disableButton.SetActive(false);
        installingButton.SetActive(false);
        installButton.SetActive(false);
        upgradeButton.SetActive(false);
        upgradingButton.SetActive(false);
        purchaseButton.GetComponentInChildren<Button>().interactable = true;
        purchaseText.color = COLOR.GREEN;
        purchaseOutline.effectColor = COLOR.GREEN;
        lockImage.SetActive(false);
        switch (sii.status) {
            case ShopItemStatus.NOT_OWNED:
                if (sii.locked) {
                    purchaseButton.GetComponentInChildren<Button>().interactable = false;
                    purchaseText.color = COLOR.GREEN_DISABLED;
                    purchaseOutline.effectColor = COLOR.GREEN_DISABLED;
                    lockImage.SetActive(true);
                }
                purchaseButton.SetActive(true);
                break;
            case ShopItemStatus.NOT_INSTALLED:
                task = gameManager.GetInstallTask(id);
                installButton.SetActive(true);
                break;
            case ShopItemStatus.INSTALLING:
                installingButton.SetActive(true);
                break;
            case ShopItemStatus.UPGRADING:
                upgradingButton.SetActive(true);
                break;
            case ShopItemStatus.ACTIVE:
                disableButton.SetActive(true);
                if (sii.level != sii.maxLevel) upgradeButton.SetActive(true);
                break;
            case ShopItemStatus.INACTIVE:
                enableButton.SetActive(true);
                if (sii.level != sii.maxLevel) upgradeButton.SetActive(true);
                break;
            default:
                Debug.Log("Error: undefined shopItemStatus");
                break;
        }
    }

    /**
     * <summary>Applies the effects of buying an item of the shop</summary>
     */
    public void PurchaseItem() {
        gameManager.PurchaseShopItem(id);
        parent.NotInstalled();
        task = gameManager.GetInstallTask(id);
        purchaseButton.SetActive(false);
        installButton.SetActive(true);
    }

    public void InstallItem(EmployeeCode eid) {
        gameManager.AssignEmployee(eid, task.id);
        parent.Installing();
        installButton.SetActive(false);
        installingButton.SetActive(true);
    }

    public void UpgradeItem(EmployeeCode eid) {
        gameManager.StartUpgradeShopItem(id);
        task = gameManager.GetUpgradeTask(id);
        gameManager.AssignEmployee(eid, task.id);
        parent.Upgrading();
        upgradeButton.SetActive(false);
        enableButton.SetActive(false);
        disableButton.SetActive(false);
        upgradingButton.SetActive(true);
    }

    /**
     * <summary>Applies the effects of enabling an item of the shop</summary>
     */
    public void EnableItem() {
        gameManager.EnableShopItem(id);
        parent.Enable();
        disableButton.SetActive(true);
        enableButton.SetActive(false);
    }

    /**
     * <summary>Applies the effects of disabling an item of the shop</summary>
     */
    public void DisableItem() {
        gameManager.DisableShopItem(id);
        parent.Disable();
        enableButton.SetActive(true);
        disableButton.SetActive(false);
    }

    /**
     * <summary>Close the details</summary>
     */
    public void Back() {
        gameObject.SetActive(false);
    }

    public void OpenEmployeeChoiceToInstall() {
        employeeChoice.Load(id, gameManager.GetShopItem(id).category, this, TaskType.INSTALL);
    }

    public void OpenEmployeeChoiceToUpgrade() {
        employeeChoice.Load(id, gameManager.GetShopItem(id).category, this, TaskType.UPGRADE);
    }
}
