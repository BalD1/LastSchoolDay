using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public static class CustomLogger
{
    public enum E_LogType
    {
        Basic,
        Warning,
        Error
    }
    public static void Log(this MonoBehaviour sender, object message, E_LogType logType = E_LogType.Basic, [CallerLineNumber] int atLine = -1)
        => Log(message, sender.gameObject, logType, atLine);
    public static void Log(object message, GameObject sender, E_LogType logType = E_LogType.Basic, [CallerLineNumber] int atLine = -1)
        => Log(sender.GetComponent<MonoBehaviour>().GetType(), message, sender, atLine, logType);
    public static void Log(object message, float sendTime, GameObject sender, E_LogType logType = E_LogType.Basic, [CallerLineNumber] int atLine = -1)
    {
        message += $" <color=#444444#><i>at {sendTime}</i></color>";
        Log(sender.GetComponent<MonoBehaviour>().GetType(), message, sender, atLine, logType);
    }
    private static void Log(Type scriptType, object message, GameObject sender, int atLine, E_LogType logType = E_LogType.Basic)
    {
        bool isManager = scriptType.ToString().Contains("Manager");
        string color = isManager ? "FF0000" : "0000FF";
        StringBuilder finalMessage = new StringBuilder();
        finalMessage.Append(atLine)
                    .Append($"[<color=#{color}><b>{scriptType}</b></color>")
                    .Append(isManager ? "]" : $", <b>{sender.gameObject.name}</b>]")
                    .Append(" : ")
                    .Append(message);
        switch (logType)
        {
            case E_LogType.Basic:
                Debug.Log(finalMessage.ToString(), sender);
                break;
            case E_LogType.Warning:
                Debug.LogWarning(finalMessage.ToString(), sender);
                break;
            case E_LogType.Error:
                Debug.LogError(finalMessage.ToString(), sender);
                break;
        }

    }
    public static void Log(this object sender, object message, E_LogType logType = E_LogType.Basic, [CallerLineNumber] int atLine = -1)
    {
        Type scriptType = sender.GetType();
        bool isManager = scriptType.ToString().Contains("Manager");
        string color = isManager ? "FF0000" : "0000FF";
        StringBuilder finalMessage = new StringBuilder();
        finalMessage.Append(atLine)
                    .Append($"[<color=#{color}><b>{scriptType}</b></color>")
                    .Append(isManager ? "]" : $", <b>{sender}</b>]")
                    .Append(" : ")
                    .Append(message);

        GameObject senderObj = null;
        if (sender is MonoBehaviour)
            senderObj = (sender as MonoBehaviour).gameObject;

        switch (logType)
        {
            case E_LogType.Basic:
                Debug.Log(finalMessage.ToString(), senderObj);
                break;
            case E_LogType.Warning:
                Debug.LogWarning(finalMessage.ToString(), senderObj);
                break;
            case E_LogType.Error:
                Debug.LogError(finalMessage.ToString(), senderObj);
                break;
        }
    }

    public static void BaseComponentsLog(this object sender, IComponentHolder.E_Component componentType, IComponentHolder.E_Result tryGetResult, IComponentHolder holder, [CallerLineNumber] int atLine = -1)
    {
        string message = $"Could not find {componentType} in {holder}, error type : {tryGetResult}";
        Log(holder, message, E_LogType.Error, atLine);
    }
}
