using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Xml;
using quiet;

public class DialogueManager : MonoBehaviour
{
    List<Dialogue> dialogue;

    private int currentDialogue;

    public GameObject dialoguePanel;
    public GameObject bethObject;
    public GameObject textObject;
    public TMPro.TextMeshProUGUI text;

    private GameState resumeState;

    public Dialogue CurrentDialogue
    {
        get
        {
            return dialogue[currentDialogue];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentDialogue = 0;
    }

    public void Init()
    {
        dialoguePanel = GameObject.FindGameObjectWithTag("Dialogue");
        foreach(Transform child in dialoguePanel.transform)
        {
            GameObject go = child.gameObject;
            if(go.name == "Beth")
            {
                bethObject = go;
            }
            else
            {
                textObject = go;
                foreach(Transform t in go.transform)
                {
                    text = t.GetComponent<TMPro.TextMeshProUGUI>();
                }
            }
        }
    }

    public void StartPlayingDialogue(GameState resumeState)
    {
        this.resumeState = resumeState;

        // Display beth and the text box
        bethObject.SetActive(true);
        textObject.SetActive(true);
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        DialogueLine? text = CurrentDialogue.GetNextLine();
        if (!text.HasValue)
        {
            StopPlayingDialogue();
            return;
        }

        // Update Beth sprite and text
        bethObject.GetComponent<Beth>().SwapMood(text.Value.Mood);
        this.text.text = text.ToString();
    }

    public void StopPlayingDialogue()
    {
        // Hide beth sprite and textbox
        bethObject.SetActive(false);
        textObject.SetActive(false);

        GameMaster.Instance.GameStateManager.SwapState(resumeState);
        CurrentDialogue.endBehaviour();
        currentDialogue++;
    }

    public bool ShouldStart(GameState phase)
    {
        if (!currentDialogue.InRange(0, dialogue.Count - 1)) return false;
        return GameMaster.Instance.ActiveLevel == CurrentDialogue.level && phase.ToString().ToLower() == CurrentDialogue.init;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadDialogue()
    {
        dialogue = new List<Dialogue>();

        XmlDocument doc = new XmlDocument();
        doc.Load(Application.dataPath + @"\Scripts\Dialogue\dialogue.xml");

        XmlNode root = doc.DocumentElement;

        foreach(XmlNode n in root.ChildNodes)
        {
            dialogue.Add(new Dialogue(n));
        }
    }
}
