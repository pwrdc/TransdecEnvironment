using UnityEngine;
using UnityEngine.UI;

public class DataLabel : MonoBehaviour
{
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        Update();
    }

    void Update()
    {
        text.text = RobotAgent.Instance.GenerateDebugString();
    }
}
