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
using UnityEngine;

public class NotificationList : MonoBehaviour {
    [SerializeField] GameObject notificationItem;

    Queue<GameObject> notifications = new Queue<GameObject>();
    int N = 3;

    public void AddNotification(string message, string name, Sprite s) {
        if (notifications.Count == N) {
            if (notifications.Peek() == null) {
                notifications.Dequeue();
            } else {
                Destroy(notifications.Dequeue());
            }
        }
        GameObject newItem = Instantiate(notificationItem, gameObject.transform, false);
        newItem.GetComponent<NotificationItem>().Load(message, name, s);
        notifications.Enqueue(newItem);
    }
}
