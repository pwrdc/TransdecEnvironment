using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(TextAttribute))]
internal sealed class TextDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        // position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
        position = EditorGUI.IndentedRect(position);
        GUI.Label(position, (attribute as TextAttribute).text, EditorStyles.label);
    }

    public override float GetHeight()
    {
        return EditorGUIUtility.singleLineHeight;
    }
}

// src: https://forum.unity.com/threads/helpattribute-allows-you-to-use-helpbox-in-the-unity-inspector-window.462768/#post-3014998
[CustomPropertyDrawer(typeof(HelpBoxAttribute))]
public class HelpBoxAttributeDrawer : DecoratorDrawer
{
    public override float GetHeight()
    {
        var helpBoxAttribute = attribute as HelpBoxAttribute;
        if (helpBoxAttribute == null) return base.GetHeight();
        var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
        if (helpBoxStyle == null) return base.GetHeight();
        return Mathf.Max(40f, helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 4);
    }

    public override void OnGUI(Rect position)
    {
        var helpBoxAttribute = attribute as HelpBoxAttribute;
        if (helpBoxAttribute == null) return;
        EditorGUI.HelpBox(position, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.messageType));
    }

    private MessageType GetMessageType(HelpBoxMessageType helpBoxMessageType)
    {
        switch (helpBoxMessageType)
        {
            default:
            case HelpBoxMessageType.None: return MessageType.None;
            case HelpBoxMessageType.Info: return MessageType.Info;
            case HelpBoxMessageType.Warning: return MessageType.Warning;
            case HelpBoxMessageType.Error: return MessageType.Error;
        }
    }
}