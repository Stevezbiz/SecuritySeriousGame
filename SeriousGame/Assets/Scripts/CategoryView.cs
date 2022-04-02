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
using Image = UnityEngine.UI.Image;

public class CategoryView : MonoBehaviour {
    [SerializeField] CountermeasureItemDetails details;
    [SerializeField] GameObject countermeasureItem;
    [SerializeField] RectTransform content;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject bottomPanel;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] Image titleImage;

    List<GameObject> toDestroy = new List<GameObject>();

    /**
     * <summary>Load all the items in the list</summary>
     */
    public void Load(CategoryCode c) {
        Category cat = gameManager.GetCategory(c);
        titleText.SetText(cat.name.ToLower());
        titleImage.sprite = cat.sprite;
        foreach (GameObject obj in toDestroy) {
            Destroy(obj);
        }
        toDestroy.Clear();
        foreach (ShopItemCode id in gameManager.GetShopItemsByCategory(c)) {
            AddCountermeasureRecord(gameManager.GetShopItem(id));
        }
    }

    /**
     * <summary>Load an item in the list</summary>
     */
    void AddCountermeasureRecord(ShopItemInfo sii) {
        // create the new item
        GameObject newItem = Instantiate(countermeasureItem, content, false);
        newItem.GetComponent<CountermeasureItem>().Load(sii, details);
        toDestroy.Add(newItem);
    }

    /**
     * <summary></summary>
     */
    public void OpenView(CategoryCode c) {
        TimeManager.Pause();
        Load(c);
        gameObject.SetActive(true);
        bottomPanel.SetActive(false);
    }

    /**
     * <summary></summary>
     */
    public void CloseView() {
        details.gameObject.SetActive(false);
        TimeManager.Resume();
        bottomPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
