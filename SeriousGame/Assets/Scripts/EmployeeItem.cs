using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EmployeeItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI statusText;

    EmployeeCode id;
    EmployeeView parent;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(EmployeeInfo e, EmployeeView parent) {
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

    /**
     * <summary>Applies the effects of firing an employee</summary>
     */
    public void Fire() {
        statusText.SetText("DISPONIBILE");
    }
}
