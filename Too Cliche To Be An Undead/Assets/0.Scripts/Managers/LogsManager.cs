using System;
using UnityEngine;

public static class LogsManager
{
    public static void Log(MonoBehaviour script, string message, GameObject sender)
    {
#if !UNITY_EDITOR
        return;
#endif
        Type scriptType = script.GetType();
        bool isManager = scriptType.ToString().Contains("Manager");
        string color = isManager ? "FF0000" : "0000FF";
        Debug.LogFormat(sender, 
            $"[<color=#{color}><b>{scriptType}</b></color>" + 
            (isManager ? "]" :
            $", <b>{sender.gameObject.name}</b>]") + 
            $" : {message}");
    }
    public static void Log(MonoBehaviour script, string message, float sendTime, GameObject sender)
    {
        Log(script, message + $" <i> at {sendTime}</i>", sender);
    }
}
