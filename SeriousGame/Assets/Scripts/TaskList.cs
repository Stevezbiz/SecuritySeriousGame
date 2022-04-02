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

public class TaskList : MonoBehaviour {
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject taskItem;
    [SerializeField] EmployeeList empoyeeList;
    [SerializeField] RectTransform content;
    [SerializeField] EmployeeView employeeView;
    [SerializeField] TextMeshProUGUI selector;
    [SerializeField] Button selectorButton;

    List<GameObject> toDestroy = new List<GameObject>();

    void Load() {
        foreach (GameObject o in toDestroy) {
            Destroy(o);
        }
        toDestroy.Clear();
        foreach (Task t in gameManager.GetAssignedTasks()) {
            GameObject newItem = Instantiate(taskItem, content, false);
            newItem.GetComponent<TaskItem>().Load(gameManager, employeeView, t, true);
            toDestroy.Add(newItem);
        }
        foreach (Task t in gameManager.GetWaitingTasks()) {
            GameObject newItem = Instantiate(taskItem, content, false);
            newItem.GetComponent<TaskItem>().Load(gameManager, employeeView, t, false);
            toDestroy.Add(newItem);
        }
        content.localPosition = new Vector3(content.localPosition.x, 0f, content.localPosition.z);
    }

    public void OpenView() {
        selector.color = COLOR.GREEN;
        selectorButton.interactable = false;
        empoyeeList.CloseView();
        Load();
        gameObject.SetActive(true);
    }

    public void CloseView() {
        selector.color = COLOR.GREEN_DISABLED;
        selectorButton.interactable = true;
        gameObject.SetActive(false);
    }
}
