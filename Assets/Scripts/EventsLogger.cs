using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is meant for
/// logging simulation events for the used.
/// Adding new logger requires implementing EventsLogReceiver 
/// and calling EventsLogger.AddReceiver on class instance.
/// </summary>
public class EventsLogger : MonoBehaviour
{
    private static List<EventsLogReceiver> receivers=new List<EventsLogReceiver>();

    public static void AddReceiver(EventsLogReceiver receiver)
    {
        receivers.Add(receiver);
    }

    public static void Log(string eventName)
    {
        foreach(var receiver in receivers)
        {
            receiver.Receive(eventName);
        }
    }
}
