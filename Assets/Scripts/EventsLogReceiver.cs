using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EventsLogReceiver
{
    void Receive(string eventName);
}
