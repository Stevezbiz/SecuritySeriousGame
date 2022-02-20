using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDetail : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI nextDescriptionText;
    [SerializeField] TextMeshProUGUI nextRequirementsText;
    [SerializeField] TextMeshProUGUI nextResistancesText;
    [SerializeField] TextMeshProUGUI nextCostsText;
    [SerializeField] TextMeshProUGUI actualDescriptionText;
    [SerializeField] TextMeshProUGUI actualResistancesText;
    [SerializeField] TextMeshProUGUI actualCostsText;
    [SerializeField] GameObject purchaseButton;
    [SerializeField] GameObject enableButton;
    [SerializeField] GameObject disableButton;
    [SerializeField] GameObject installingButton;
    [SerializeField] GameObject installButton;
    [SerializeField] GameObject upgradeButton;
    [SerializeField] GameObject upgradingButton;
    [SerializeField] GameObject purchaseLockImage;
    [SerializeField] TextMeshProUGUI purchaseText;
    [SerializeField] Outline purchaseOutline;
    [SerializeField] GameObject upgradeLockImage;
    [SerializeField] TextMeshProUGUI upgradeText;
    [SerializeField] Outline upgradeOutline;
    [SerializeField] GameManager gameManager;
    [SerializeField] EmployeeChoice employeeChoice;
    [SerializeField] RectTransform content;

    ShopItem parent;
    ShopItemCode id;
    Task task;

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
        purchaseLockImage.SetActive(false);
        upgradeButton.GetComponentInChildren<Button>().interactable = true;
        upgradeText.color = COLOR.GREEN;
        upgradeOutline.effectColor = COLOR.GREEN;
        upgradeLockImage.SetActive(false);
        switch (sii.status) {
            case ShopItemStatus.NOT_OWNED:
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
        if (sii.locked[sii.level]) {
            if (sii.status == ShopItemStatus.NOT_OWNED) {
                purchaseButton.GetComponentInChildren<Button>().interactable = false;
                purchaseText.color = COLOR.GREEN_DISABLED;
                purchaseOutline.effectColor = COLOR.GREEN_DISABLED;
                purchaseLockImage.SetActive(true);
            } else {
                upgradeButton.GetComponentInChildren<Button>().interactable = false;
                upgradeText.color = COLOR.GREEN_DISABLED;
                upgradeOutline.effectColor = COLOR.GREEN_DISABLED;
                upgradeLockImage.SetActive(true);
            }
            
        }
        content.SetPositionAndRotation(new Vector3(content.position.x, 0f, content.position.z), Quaternion.identity);
    }

    /**
     * <summary>Compose all the details of the item</summary>
     */
    void ComposeDetails(ShopItemInfo sii) {
        // compose details of actual level
        if (sii.level > 0) {
            actualDescriptionText.SetText("LIVELLO " + sii.level + " (installato)\n" + sii.description[sii.level - 1]);
            ComposeLevelDetails(sii, sii.level - 1, true);
        } else {
            actualDescriptionText.SetText("LIVELLO 0: non hai ancora installato questo potenziamento");
            actualResistancesText.SetText("");
            actualCostsText.SetText("");
        }
        titleText.SetText(sii.name.ToLower());
        if (sii.level == sii.maxLevel) {
            costText.SetText("");
            nextDescriptionText.SetText("Non ci sono potenziamenti disponibili");
            nextRequirementsText.SetText("");
            nextResistancesText.SetText("");
            nextCostsText.SetText("");
        } else {
            costText.SetText(sii.cost[sii.level] + " Fondi");
            nextDescriptionText.SetText("LIVELLO " + (sii.level + 1) + "\n" + sii.description[sii.level]);
            ComposeLevelDetails(sii, sii.level, false);
        }
    }

    void ComposeLevelDetails(ShopItemInfo sii, int l, bool actual) {
        string requirements = "";
        string resistances;
        string costs = "";
        // set the technical details about requirements
        if (!actual && sii.locked[l]) {
            requirements += "Per sbloccare questo oggetto devi prima acquistare:\n";
            foreach (Requirement r in sii.reqArray[l].requirements) {
                requirements += "    " + gameManager.GetShopItem(r.id).name.ToUpper() + " - Lv." + r.level + "\n";
            }
        }
        // set the technical details about resistances
        resistances = "Resistenze:";
        if (actual) {
            // cumulative resistances (possessed item)
            foreach (Resistance r in sii.resArray[l].resistances) {
                resistances += string.Format("\n    {0,-24}",gameManager.GetAttack(r.id).name.ToUpper());
                if (r.duration != 0) resistances += string.Format(" || durata {0,3}%", (gameManager.GetActualDurationResistance(r.duration) * -100).ToString("-0."));
                if (r.miss != 0) resistances += string.Format(" | difesa {0,3}%", (gameManager.GetActualMissResistance(r.miss) * 100).ToString("+0."));
                if (r.endurance != 0) resistances += string.Format(" | complessitÓ {0,3}%", (gameManager.GetActualEnduranceResistance(r.endurance) * 100).ToString("+0."));
            }
        } else {
            // differential resistances (next level)
            foreach (Resistance r in gameManager.GetShopItemResistances(sii.id)) {
                resistances += string.Format("\n    {0,-24}", gameManager.GetAttack(r.id).name.ToUpper());
                if (r.duration != 0) resistances += string.Format(" || durata {0,3}%", (r.duration * 100).ToString("-0."));
                if (r.miss != 0) resistances += string.Format(" | difesa {0,3}%", (r.miss * 100).ToString("+0."));
                if (r.endurance != 0) resistances += string.Format(" | complessitÓ {0,3}%", (r.endurance * 100).ToString("+0."));
            }
        }
        
        if (sii.resArray.Length == 0) resistances += " nessuna";
        resistances += "\n";
        // set the technical details about costs and usability
        float moneyMalus = sii.moneyMalus[l];
        float usersMod = sii.usersMod[l];
        if (!actual && l > 0) {
            usersMod -= sii.usersMod[l - 1];
            moneyMalus -= sii.moneyMalus[l - 1];
        }
        // possible bonuses
        if (moneyMalus < 0) resistances += "Guadagno aggiuntivo: " + (-moneyMalus) + " F/h\n";
        if (usersMod < 0) resistances += "Prestazioni e usabilitÓ: +" + (-usersMod * 100) + "%\n";
        // possible maluses
        if (moneyMalus < 0) costs += "Costo: 0 F/h\n";
        else costs += "Costo: " + moneyMalus + " F/h\n";
        if (usersMod > 0) costs += "Prestazioni e usabilitÓ: -" + (usersMod * 100) + "%\n";
        if (actual) {
            actualResistancesText.SetText(resistances);
            actualCostsText.SetText(costs);
        } else {
            nextRequirementsText.SetText(requirements);
            nextResistancesText.SetText(resistances);
            nextCostsText.SetText(costs);
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
