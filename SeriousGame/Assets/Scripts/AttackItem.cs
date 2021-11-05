using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;

    AttackCode id;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(AttackInfo attack) {
        this.id = attack.id;
        text.SetText(attack.name);
    }

    /**
     * <summary>Function called when the element is clicked</summary>
     */
    public void OnClick() {

    }
}
