using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1_Tutorial : MonoBehaviour {
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] RectTransform selfPos;
    [SerializeField] GameObject parent;
    [SerializeField] GameObject focusElement;
    [SerializeField] GameObject gui;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject notes;
    [SerializeField] GameObject messageList;
    [SerializeField] GameObject message;
    [SerializeField] TMPro.TMP_InputField messageBody;

    LVL1_Network network;

    int slide = 0;
    string[] slideText = {
        "Il <i>PortStealing</i> è un attacco informatico che significa letteralmente \"furto della porta\". Lo scopo di questa tecnica è impersonare un altro host all'interno della rete \"rubando\" la porta che sta utilizzando per comunicare con lo switch",
        "Lo switch infatti mantiene al proprio interno una tabella in cui, per ogni indirizzo MAC, viene memorizzata l'associazione con la porta da cui proveniva il messaggio. I valori nella tabella vengono modificati quando viene rilevata una nuova combinazione MAC-porta",
        "Questo meccansimo è molto utile se occorre spostare gli host della rete senza riconfigurare manualmente lo switch. Ora seguono un paio di esempi per comprendere meglio il funzionamento",
        "In condizioni normali:\n- Un host è connesso alla porta 1 dello switch\n- l'host viene scollegato e ricollegato sulla porta 2\n- appena l'host invia un messaggio lo switch aggiorna la propria tabella e sa che in futuro dovrà inviare i messaggi destinati all'host sulla porta 2",
        "Tuttavia si può considerare uno scenario diverso:\n- A è collegato alla porta 1, mentre B alla porta 2\n- B invia un messaggio contraffatto con il MAC di A\n- lo switch invierà i futuri messaggi indirizzati ad A verso B, assumendo che A si sia spostato. In realtà si tratta di un dirottamento ad opera di B",
        "La vittima non viene quindi attaccata direttamente, ma momentaneamente esclusa dalla conversazione. In qualsiasi momento può inviare un messaggio per \"riconquistare\" la porta che le è stata sottratta",
        "Puoi visualizzare la tabella dello switch cliccando su di esso",
        "Puoi visualizzare i messaggi ricevuti e inviati cliccando sul tasto \"MESSAGGI\"",
        "Puoi inviare un nuovo messaggio cliccando sul tasto \"NUOVO MESSAGGIO\" e per visualizzare un messaggio nella lista è sufficiente cliccare su di esso",
        "Una volta individuato un codice all'interno di un messaggio puoi salvarlo nei tuoi appunti evidenziandolo e poi cliccando sul tasto \"SALVA NEGLI APPUNTI\"",
        "Puoi aprire gli appunti cliccando sull'icona del taccuino",
        "Nella finestra che si aprirà sono visualizzati gli appunti salvati ed è possibile inserire i codici individuati",
        "Ora prova tu ad intercettare le conversazioni di Alice e Bob. Il tuo obiettivo è scovare almeno un codice riservato all'interno dei loro messaggi",
    };

    Vector3[] slidePos = {
        new Vector3(0, -355, 0), // 5
        new Vector3(0, -355, 0), // 5
        new Vector3(0, -385, 0), // 4
        new Vector3(0, -320, 0), // 6
        new Vector3(0, -320, 0), // 6
        new Vector3(0, -385, 0), // 4
        new Vector3(0, -445, 0), // 2
        new Vector3(0, 445, 0), // 2
        new Vector3(0, 160, 0), // 3
        new Vector3(0, -150, 0), // 3
        new Vector3(0, 460, 0), // 1
        new Vector3(0, 0, 0), // 2
        new Vector3(0, -415, 0), // 3
    };

    Vector2[] slideDim = {
        new Vector2(1880, 330), // 5
        new Vector2(1880, 330), // 5
        new Vector2(1880, 270), // 4
        new Vector2(1880, 400), // 6
        new Vector2(1880, 400), // 6
        new Vector2(1880, 270), // 4
        new Vector2(1880, 150), // 2
        new Vector2(1880, 150), // 2
        new Vector2(1880, 210), // 3
        new Vector2(1880, 210), // 3
        new Vector2(1880, 120), // 1
        new Vector2(1880, 150), // 2
        new Vector2(1880, 210), // 3
    };

    List<GameObject> toDestroy = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        network = GameObject.FindGameObjectWithTag("Network").GetComponent<LVL1_Network>();
        renderSlide(slide++);
    }

    // Update is called once per frame
    void Update() {

    }

    public void Next() {
        if (slide == slideText.Length) {
            network.BeginLevel();
            Destroy(gameObject);
        } else {
            renderSlide(slide++);
        }
    }

    public void Skip() {
        network.BeginLevel();
        Destroy(gameObject);
    }

    void renderSlide(int i) {
        selfPos.anchoredPosition = slidePos[i];
        selfPos.sizeDelta = slideDim[i];
        text.text = slideText[i];

        foreach(GameObject obj in toDestroy) {
            Destroy(obj);
        }
        toDestroy.Clear();

        switch (i) {
            case 6:
                renderContent6();
                break;
            case 7:
                renderContent7();
                break;
            case 8:
                renderContent8();
                break;
            case 9:
                renderContent9();
                break;
            case 10:
                renderContent10();
                break;
            case 11:
                renderContent11();
                break;
            case 12:
                notes.SetActive(false);
                gui.SetActive(true);
                break;
            default:
                break;
        }
    }

    void renderContent6() {
        // switchButton
        GameObject focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        RectTransform focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(-54, 162, 0);
        focusTransform.sizeDelta = new Vector2(140, 80);
    }

    void renderContent7() {
        // messageButton
        GameObject focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        RectTransform focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(740, -470, 0);
        focusTransform.sizeDelta = new Vector2(430, 130);
    }

    void renderContent8() {
        // visualize the messageList
        messageList.SetActive(true);
        toDestroy.Add(messageList);

        // newMessageButton
        GameObject focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        RectTransform focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(-570, -425, 0);
        focusTransform.sizeDelta = new Vector2(600, 130);

        // message
        focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(0, -53, 0);
        focusTransform.sizeDelta = new Vector2(1700, 120);
    }

    void renderContent9() {
        // visualize message
        message.SetActive(true);
        toDestroy.Add(message);

        // select code
        messageBody.Select();

        // saveInNotesButton
        GameObject focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        RectTransform focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(0, -422, 0);
        focusTransform.sizeDelta = new Vector2(600, 120);
    }

    void renderContent10() {
        // notesButton
        GameObject focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        RectTransform focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(875, -335, 0);
        focusTransform.sizeDelta = new Vector2(160, 160);
    }

    void renderContent11() {
        // notes
        GameObject focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        RectTransform focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(-300, 260, 0);
        focusTransform.sizeDelta = new Vector2(1100, 170);
        
        // insertSecret
        focus = Instantiate(focusElement);
        toDestroy.Add(focus);
        focus.transform.SetParent(parent.transform);
        focusTransform = focus.GetComponent<RectTransform>();
        focusTransform.anchoredPosition = new Vector3(0, -252, 0);
        focusTransform.sizeDelta = new Vector2(1650, 110);

        notes.SetActive(true);
        gui.SetActive(false);
    }
}
