using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class TextAttribute : PropertyAttribute
{
    public readonly string text;

    public TextAttribute(string text)
    {
        this.text = text;
    }
}

public enum HelpBoxMessageType { None, Info, Warning, Error }

public class HelpBoxAttribute : PropertyAttribute
{
    public string text;
    public HelpBoxMessageType messageType;

    public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None)
    {
        this.text = text;
        this.messageType = messageType;
    }
}