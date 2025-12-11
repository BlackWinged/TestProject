using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ErrorHandlingLogic : MonoBehaviour
{
    private List<string> MessageQueue = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddErrorMessage(string message)
    {
        MessageQueue.Add(message);
        var distinctMessages = MessageQueue.GroupBy(x => x).Select(g => g.First()).ToList();
        GetComponentInChildren<TMP_Text>().text = string.Join("\n", distinctMessages);
    }
}
