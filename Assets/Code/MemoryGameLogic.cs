using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TestProject;
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

    private int NumberOfAttempts;


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

        CardDeck.AddRange(Utils.PopulateGrid(CardPrefab, pickedNumberOfPairs * 2, Utils.GetVisibleWorldBounds(Camera.main, 1f), 3, 4.2f));
    }

    public void CheckCardStatus()
    {
        var flippedCards = CardDeck.Where(x => x.GetComponent<CardLogic>().IsFlipped).Count();
    }
}



