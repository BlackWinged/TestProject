using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MemoryGameLogic : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Number of pairs of cards in the memory game - easy version")]
    [SerializeField] public int numberOfPairs_easy = 6;
    [Tooltip("Number of pairs of cards in the memory game - medium version")]
    [SerializeField] public int numberOfPairs_medium = 12;
    [Tooltip("Number of pairs of cards in the memory game - medium version")]
    [SerializeField] public int numberOfPairs_hard = 66;


    [Header("Prefabs and shit")]
    [Tooltip("Base card object")]
    [SerializeField] public GameObject CardPrefab;

    private int pickedNumberOfPairs;
    private List<GameObject> CardDeck = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickEasy()
    {
        pickedNumberOfPairs = numberOfPairs_easy;
        StartGame();
    }

    public void PickMedium()
    {
        pickedNumberOfPairs = numberOfPairs_medium;
        StartGame();
    }

    public void PickHard()
    {
        pickedNumberOfPairs = numberOfPairs_hard;
        StartGame();
    }

    public void StartGame()
    {
        var buttons = GameObject.FindGameObjectsWithTag("HideableMenu");
        foreach (var button in buttons) button.SetActive(false);

        for (int i = 0; i < pickedNumberOfPairs; i += 2)
        {
            GameObject instance = Instantiate(CardPrefab, new Vector2(5, 5), Quaternion.identity);
            GameObject instance2 = Instantiate(CardPrefab, new Vector2(5, 5), Quaternion.identity);
            CardDeck.Add(instance);
        }
    }
}



