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
