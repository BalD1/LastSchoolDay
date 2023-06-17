using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class LogsManager
{
    public static void Log(string message, GameObject sender, [CallerLineNumber] int atLine = -1)
        => Log(sender.GetComponent<MonoBehaviour>().GetType(), message, sender, atLine);
    public static void Log(string message, float sendTime, GameObject sender, [CallerLineNumber] int atLine = -1)
    {
        message += $" <color=#444444#><i>at {sendTime}</i></color>";
        Log(sender.GetComponent<MonoBehaviour>().GetType(), message, sender, atLine);
    }
    private static void Log(Type scriptType, string message, GameObject sender, int atLine)
    {
#if !UNITY_EDITOR
        return;
#endif
        bool isManager = scriptType.ToString().Contains("Manager");
        string color = isManager ? "FF0000" : "0000FF";
        Debug.LogFormat(sender,
            $"{atLine} : " +
            $"[<color=#{color}><b>{scriptType}</b></color>" +
            (isManager ? "]" :
            $", <b>{sender.gameObject.name}</b>]") +
            $" : {message}");
    }

}
