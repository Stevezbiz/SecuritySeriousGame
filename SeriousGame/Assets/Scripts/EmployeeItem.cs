using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmployeeItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;

    EmployeeCode id;
    EmployeeShop parent;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(EmployeeInfo e, EmployeeShop parent) {
        this.id = e.id;
        this.parent = parent;
        nameText.SetText(e.name);
        if (e.owned) statusText.SetText("ASSUNTO");
        else statusText.SetText("DISPONIBILE");
    }

    /**
     * <summary>Function called when the element is clicked</summary>
     */
    public void OnClick() {
        parent.ComposeDetails(id);
    }

    /**
     * <summary>Applies the effects of hiring an employee</summary>
     */
    public void Hire() {
        statusText.SetText("ASSUNTO");
    }
}
