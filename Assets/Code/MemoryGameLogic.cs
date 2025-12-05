using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class MemoryGameLogic : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Number of pairs of cards in the memory game - easy version")]
    [SerializeField] public int numberOfPairs_easy = 6;
    [Tooltip("Number of pairs of cards in the memory game - medium version")]
    [SerializeField] public int numberOfPairs_medium = 12;
    [Tooltip("Number of pairs of cards in the memory game - medium version")]
    [SerializeField] public int numberOfPairs_hard = 66;

    private int pickedNumberOfPairs;


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
    }

    public void PickMedium()
    {
        pickedNumberOfPairs = numberOfPairs_medium;
    }

    public void PickHard()
    {
        pickedNumberOfPairs = numberOfPairs_hard;
    }

    public void StartGame()
    {

    }
}



public class Card
{
    public int id;
    public bool isFlipped;
    public bool isMatched;
    public Card(int id)
    {
        this.id = id;
        this.isFlipped = false;
    }
}