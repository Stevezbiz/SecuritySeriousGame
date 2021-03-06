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
using Button = UnityEngine.UI.Button;
using Outline = UnityEngine.UI.Outline;

public class CountermeasureItemDetails : MonoBehaviour {
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI actualDescriptionText;
    [SerializeField] TextMeshProUGUI actualResistancesText;
    [SerializeField] TextMeshProUGUI actualCostsText;
    [SerializeField] GameObject enableButton;
    [SerializeField] GameObject disableButton;
    [SerializeField] GameManager gameManager;
    [SerializeField] RectTransform content;

    CountermeasureItem parent;
    ShopItemCode id;

    /**
     * <summary>Populate the item of the shop with all the elements to show</summary>
     */
    public void Load(ShopItemCode id, CountermeasureItem parent) {
        this.id = id;
        this.parent = parent;
        ShopItemInfo sii = gameManager.GetShopItem(id);
        ComposeDetails(sii);
        // set the visual aspect
        switch (sii.status) {
            case ShopItemStatus.NOT_OWNED:
                enableButton.SetActive(false);
                disableButton.SetActive(false);
                break;
            case ShopItemStatus.NOT_INSTALLED:
                enableButton.SetActive(false);
                disableButton.SetActive(false);
                break;
            case ShopItemStatus.INSTALLING:
                enableButton.SetActive(false);
                disableButton.SetActive(false);
                break;
            case ShopItemStatus.UPGRADING:
                enableButton.SetActive(false);
                disableButton.SetActive(false);
                break;
            case ShopItemStatus.ACTIVE:
                enableButton.SetActive(false);
                disableButton.SetActive(true);
                break;
            case ShopItemStatus.INACTIVE:
                enableButton.SetActive(true);
                disableButton.SetActive(false);
                break;
            default:
                Debug.Log("Error: undefined ShopItemStatus");
                break;
        }
        content.localPosition = new Vector3(content.localPosition.x, 0f, content.localPosition.z);
    }

    /**
     * <summary>Compose all the details of the item</summary>
     */
    void ComposeDetails(ShopItemInfo sii) {
        // compose details of actual level
        if (sii.level > 0) {
            string resistances;
            string costs = "";
            int l = sii.level - 1;
            actualDescriptionText.SetText("LIVELLO " + sii.level + "\n" + sii.description[l]);
            // set the technical details about resistances
            resistances = "Resistenze:";
            foreach (Resistance r in sii.resArray[l].resistances) {
                resistances += string.Format("\n    {0,-24}", gameManager.GetAttack(r.id).name.ToUpper());
                if (r.duration != 0) resistances += string.Format(" || durata {0,3}%", (gameManager.GetActualDurationResistance(r.duration) * 100).ToString("-0."));
                if (r.miss != 0) resistances += string.Format(" | difesa {0,3}%", (gameManager.GetActualMissResistance(r.miss) * 100).ToString("+0."));
                if (r.endurance != 0) resistances += string.Format(" | complessit? {0,3}%", (gameManager.GetActualEnduranceResistance(r.endurance) * 100).ToString("+0."));
            }
            if (sii.resArray.Length == 0) resistances += " nessuna";
            resistances += "\n";
            // set the technical details about costs and usability
            float moneyMalus = sii.moneyMalus[l];
            float usersMod = sii.usersMod[l];
            // possible bonuses
            if (moneyMalus < 0) resistances += "Guadagno aggiuntivo: " + (-moneyMalus) + " F/h\n";
            if (usersMod < 0) resistances += "Prestazioni e usabilit?: +" + (-usersMod * 100) + "%\n";
            // possible maluses
            if (moneyMalus < 0) costs += "Costo: 0 F/h\n";
            else costs += "Costo: " + moneyMalus + " F/h\n";
            if (usersMod > 0) costs += "Prestazioni e usabilit?: -" + (usersMod * 100) + "%\n";
            actualResistancesText.SetText(resistances);
            actualCostsText.SetText(costs);
        } else {
            actualDescriptionText.SetText("LIVELLO 0: non hai ancora installato questo potenziamento");
            actualResistancesText.SetText("");
            actualCostsText.SetText("");
        }
        titleText.SetText(sii.name.ToLower());
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
}
