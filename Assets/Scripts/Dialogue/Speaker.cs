using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker
{
    string name;
    string color;

    public void Deconstruct(out string name, out string color) => (name, color) = (this.name, this.color);

    public Speaker(string name, string color)
    {
        this.name = name;
        this.color = color;
    }

    public override string ToString()
    {
        return $"<color=\"{color}\">{name}</color>";
    } 
}
