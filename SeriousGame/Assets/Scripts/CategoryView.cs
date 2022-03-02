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

    public void OpenView(CategoryCode c) {
        TimeManager.Pause();
        Load(c);
        gameObject.SetActive(true);
        bottomPanel.SetActive(false);
    }

    public void CloseView() {
        details.gameObject.SetActive(false);
        TimeManager.Resume();
        bottomPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
