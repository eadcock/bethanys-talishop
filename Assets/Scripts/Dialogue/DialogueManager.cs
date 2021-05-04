using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Xml;
using quiet;

public class DialogueManager : MonoBehaviour
{
    private static List<Dialogue> dialogue;
    private static int currentDialogue;
    private static GameState resumeState;

    public static GameObject dialoguePanel;
    public static GameObject bethObject;
    public static GameObject textObject;
    public static TMPro.TextMeshProUGUI text;

    public static Dialogue CurrentDialogue
    {
        get
        {
            return dialogue[currentDialogue];
        }
    }

    // Start is called before the first frame update
    void Start()
    {

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
            else if (go.name == "Dialogue Box")
            {
                textObject = go;
                foreach(Transform t in go.transform)
                {
                    text ??= t.GetComponent<TMPro.TextMeshProUGUI>();
                }
            }
        }
    }

    public void StartPlayingDialogue(GameState resumeState)
    {
        DialogueManager.resumeState = resumeState;

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
        DialogueManager.text.text = text.ToString();
    }

    public void StopPlayingDialogue()
    {
        // Hide beth sprite and textbox
        bethObject.SetActive(false);
        textObject.SetActive(false);

        GameMaster.Instance.GameStateManager.SwapState(resumeState);
        CurrentDialogue?.endBehaviour();
        currentDialogue++;
        if (currentDialogue > dialogue.Count - 1) currentDialogue = 0;
        GameMaster.Instance.Save.SaveDialogueToProfile(GameMaster.Instance.Save.CurrentProfile, currentDialogue);
    }

    public bool ShouldStart(GameState phase)
    {
        if (!currentDialogue.InRange(0, dialogue.Count - 1) || GameMaster.Instance.Save.CurrentSaveData.options.skipDialogue) return false;
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
        doc.Load(Application.streamingAssetsPath + @"\dialogue.xml");

        XmlNode root = doc.DocumentElement;

        foreach(XmlNode n in root.ChildNodes)
        {
            dialogue.Add(new Dialogue(n));
        }

        currentDialogue = GameMaster.Instance.Save.CurrentSaveData.currentDialogue;
    }
}
