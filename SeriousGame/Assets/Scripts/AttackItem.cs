using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackItem : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;

    Task task;
    SecurityView parent;

    /**
     * <summary>Populate the item with all the elements to show</summary>
     */
    public void Load(Task task, string name, SecurityView parent) {
        this.task = task;
        this.parent = parent;
        text.SetText(name);
    }

    /**
     * <summary>Function called when the element is clicked</summary>
     */
    public void OnClick() {
        parent.OpenEmployeeChoice(task);
    }
}
