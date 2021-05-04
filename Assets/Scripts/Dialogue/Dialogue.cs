using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;
using System;
using quiet;
using System.Xml;

public class Dialogue : IEnumerable<DialogueLine>
{
    public static Dictionary<string, Speaker> speakers = new Dictionary<string, Speaker>()
    {
        { "beth", new Speaker("Bethany", "blue") },
        { "player", new Speaker("Player", "white") },
        { "unknown", new Speaker("Unknown", "white") },
    };

    public DialogueLine[] lines;
    public int level;
    public string init;
    public Action endBehaviour;
    private int currentLine;

    public Dialogue(IEnumerable<DialogueLine> lines, int level, string init = "start")
        => (this.lines, this.level, this.init, currentLine) = (lines.ToArray(), level, init, -1);

    public Dialogue(XmlNode text)
    {
        Collection<DialogueLine> lines = new Collection<DialogueLine>();
        if (!int.TryParse(text.Attributes?["level"].Value, out int level))
            level = 1;

        foreach (XmlNode inner in text)
        {
            lines.Add(new DialogueLine(inner.InnerText, (Mood)Enum.Parse(typeof(Mood), inner.Attributes["mood"].Value, true), speakers[inner.Attributes["speaker"].Value]));
        }

        this.lines = lines.ToArray();
        this.level = level;
        this.init = text.Attributes?["init"].Value ?? "start";

        if (text.Attributes["end-behaviour"] != null)
        {
            endBehaviour = 
                text.Attributes["end-behaviour"].Value switch
                {
                    "next-level" => GameMaster.SceneTransitioner.GoForward,
                    _ => () => { },
                };
        }
        else
        {
            endBehaviour = () => { };
        }

        currentLine = -1;
    }

    public DialogueLine? GetLine(int index) => index.InRange(0, lines.Length - 1) ? (DialogueLine?)lines[index] : null;

    public DialogueLine? GetNextLine()
    {
        currentLine++;
        if (currentLine > lines.Length - 1)
            return null;

        return lines[currentLine];
    }

    public void ResetLine()
    {
        currentLine = 0;
    }

    public IEnumerator<DialogueLine> GetEnumerator()
    {
        for (int i = 0; i < lines.Length; i++)
            yield return lines[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return lines.GetEnumerator();
    }
}
