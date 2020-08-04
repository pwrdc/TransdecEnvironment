using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Text))]
public class EventsText : MonoBehaviour, EventsLogReceiver
{
    public int maxLines = 10;

    class Line
    {
        public string eventName;
        public int count;
        public Line(string eventName, int count)
        {
            this.eventName = eventName;
            this.count = count;
        }
    }
    Queue<Line> events = new Queue<Line>();
    Line last;
    Text text;

    public void Receive(string eventName)
    {
        if (last.eventName == eventName)
        {
            last.count++;
        }
        else
        {
            last = new Line(eventName, 1);
            events.Enqueue(last);
        }
        if (events.Count > maxLines)
        {
            events.Dequeue();
        }
        text.text = string.Join("\n", events.Select(
            line => {
                if (line.count == 1)
                    return line.eventName;
                else
                    return line.eventName + " x " + line.count;
            }
        ));
    }

    void Start()
    {
        text = GetComponent<Text>();
        text.text = "";
        for (int i = 0; i < maxLines; i++)
        {
            last = new Line("", 1);
            events.Enqueue(last);
        }
        EventsLogger.AddReceiver(this);
    }
}
