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

public class EmployeeList : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject employeeItem;
    [SerializeField] TaskList taskList;
    [SerializeField] RectTransform content;
    [SerializeField] TextMeshProUGUI selector;
    [SerializeField] Button selectorButton;

    List<GameObject> toDestroy = new List<GameObject>();

    /**
     * <summary></summary>
     */
    void Load() {
        foreach (GameObject o in toDestroy) {
            Destroy(o);
        }
        toDestroy.Clear();
        foreach (EmployeeInfo e in gameManager.GetHiredEmployees()) {
            GameObject newItem = Instantiate(employeeItem, content, false);
            newItem.GetComponent<EmployeeItem>().Load(gameManager, e, gameManager.GetEmployeeIcon(e.id));
            toDestroy.Add(newItem);
        }
        content.localPosition = new Vector3(content.localPosition.x, 0f, content.localPosition.z);
    }

    /**
     * <summary></summary>
     */
    public void OpenView() {
        selector.color = COLOR.GREEN;
        selectorButton.interactable = false;
        taskList.CloseView();
        Load();
        gameObject.SetActive(true);
    }

    /**
     * <summary></summary>
     */
    public void CloseView() {
        selector.color = COLOR.GREEN_DISABLED;
        selectorButton.interactable = true;
        gameObject.SetActive(false);
    }
}
