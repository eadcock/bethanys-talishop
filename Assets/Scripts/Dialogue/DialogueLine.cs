using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct DialogueLine
{
    public string Line => line;
    public Mood Mood => mood;
    public Speaker Speaker => speaker;

    readonly string line;
    readonly Mood mood;
    readonly Speaker speaker;

    public void Deconstruct(out string line, out Mood mood, out Speaker speaker) => (line, mood, speaker) = (Line, Mood, Speaker);

    public DialogueLine(string line, Mood mood, Speaker speaker) => (this.line, this.mood, this.speaker) = (line, mood, speaker);

    public override string ToString()
    {
        return speaker + "\n" + line;
    }
}
