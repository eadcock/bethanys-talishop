using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct DialogueLine
{
    private static Speaker beth;
    public static Speaker Beth
    {
        get
        {
            if(beth is null)
                beth = new Speaker("Beth", "blue");

            return beth;
        }
    }

    private static Speaker player;
    public static Speaker Player
    {
        get
        {
            if (player is null)
                player = new Speaker("Player", "white");

            return player;
        }
    }

    public string Line => line;
    public Mood Mood => mood;
    public Speaker Speaker => speaker;

    string line;
    Mood mood;
    Speaker speaker;

    public DialogueLine(string line, Mood mood, Speaker speaker)
    {
        this.line = line;
        this.mood = mood;
        this.speaker = speaker;
    }

    public override string ToString()
    {
        return speaker + "\n" + line;
    }
}
