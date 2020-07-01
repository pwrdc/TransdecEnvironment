using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSLabel : MonoBehaviour
{
    public Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = Mathf.Round((int)(1.0f / Time.smoothDeltaTime)).ToString();
    }
}