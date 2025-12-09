using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TestProject;
using TMPro;
using Unity.VisualScripting;
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
    [Tooltip("The amount of time cards stay visible")]
    [SerializeField] public float cardHangTime = 1f;

    [Tooltip("Number of columns on screen")]
    [SerializeField] public int numberOfColumns = 6;
    [Tooltip("Grid y axis padding")]
    [SerializeField] public float yAxisPadding = 4.2f;
    [Tooltip("Positional jitter")]
    [SerializeField] public float posJitter = 0.0f;


    [Header("Prefabs and shit")]
    [Tooltip("Base card object")]
    [SerializeField] public GameObject CardPrefab;
    [Tooltip("Number of attempts")]
    [SerializeField] public TMP_Text CounterLabel;
    [Tooltip("Discar pile transform")]
    [SerializeField] public Transform DiscardPile;

    private int pickedNumberOfPairs;
    private List<GameObject> CardDeck = new List<GameObject>();

    private int NumberOfAttempts;
    private float cardDelayTimer;
    private bool isWaitingToResetCards = false;

    // Card images loaded from CardImages folder
    private Sprite[] cardImages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardImages = Resources.LoadAll<Sprite>("CardImages");
    }

    // Update is called once per frame
    void Update()
    {
        CheckCardReset();
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

        CardDeck.AddRange(Utils.PopulateGrid(CardPrefab, pickedNumberOfPairs * 2, Utils.GetVisibleWorldBounds(Camera.main, 1f), numberOfColumns, yAxisPadding, posJitter));
        ShuffleList(CardDeck);
        AssignCardImages();
    }

    /// <summary>
    /// Assigns images from CardImages folder to cards in the deck
    /// </summary>
    void AssignCardImages()
    {
        if (cardImages == null || cardImages.Length == 0)
        {
            Debug.LogWarning("No card images loaded! Cannot assign images to cards.");
            return;
        }

        var cardSize = CardDeck.First().GetComponent<BoxCollider>().size;

        // Shuffle the card images to randomize which image goes to which pair
        var shuffledImages = new List<Sprite>(cardImages);
        ShuffleList(shuffledImages);

        // Ensure we have enough images for the number of pairs
        int imagesNeeded = pickedNumberOfPairs;
        if (shuffledImages.Count < imagesNeeded)
        {
            Debug.LogWarning($"Not enough card images! Need {imagesNeeded}, but only have {shuffledImages.Count}. Some cards will not have images.");
        }

        // Assign images to card pairs
        for (int i = 0; i < CardDeck.Count; i += 2)
        {
            int cardIndex = i;

            var firstCard = CardDeck[i].GetComponent<CardLogic>();
            var secondCard = CardDeck[i + 1].GetComponent<CardLogic>();

            var firstFrontRenderer = CardDeck[i].GetComponentsInChildren<SpriteRenderer>().Where(x => x.name.Contains("Front")).FirstOrDefault();
            var secondFrontRenderer = CardDeck[i + 1].GetComponentsInChildren<SpriteRenderer>().Where(x => x.name.Contains("Front")).FirstOrDefault();

            firstCard.CardId = shuffledImages[i].name;
            secondCard.CardId = shuffledImages[i].name;

            Sprite cardImage = shuffledImages[i];
            //cardImage.
            firstFrontRenderer.sprite = cardImage;
            firstFrontRenderer.transform.localScale = new Vector3(0.1993179f, 0.1369252f, 1.29586f);
            secondFrontRenderer.sprite = cardImage;
            secondFrontRenderer.transform.localScale = new Vector3(0.1993179f, 0.1369252f, 1.29586f);
        }


    }

    /// <summary>
    /// Shuffles a list using Fisher-Yates algorithm
    /// </summary>
    void ShuffleList<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public bool CanFlipCard()
    {
        var result = false;
        var flippedCards = CardDeck.Where(x => x.GetComponent<CardLogic>().isFlipped).Count();

        if (flippedCards < 2)
        {
            result = true;
        }

        return result;
    }

    public void RegisterAttempt()
    {
        var flippedCards = CardDeck.Where(x => x.GetComponent<CardLogic>().isFlipped);
        var flippedIds = flippedCards.GroupBy(x => x.GetComponent<CardLogic>().CardId);

        if (flippedCards.Count() < 2) return;

        cardDelayTimer = Time.time;
        isWaitingToResetCards = true;
        NumberOfAttempts++;
        CounterLabel.text = $"Attempts: {NumberOfAttempts}";

        if (flippedIds.Count() == 1)
        {
            foreach (var card in flippedCards)
            {
                card.GetComponent<CardLogic>().isFlipped = false;
                card.GetComponent<CardLogic>().IsDiscarded = true;
            }
        }
    }

    public void CheckCardReset()
    {
        if (isWaitingToResetCards)
        {
            if (Time.time - cardDelayTimer >= cardHangTime)
            {
                foreach (var card in CardDeck.Select(x => x.GetComponent<CardLogic>()))
                {
                    if (card.isFlipped)
                    {
                        StartCoroutine(card.FlipCard());
                        card.isFlipped = false;
                    }

                    if (card.IsDiscarded)
                    {
                        StartCoroutine(card.Discard(card.transform.position, DiscardPile.position));
                    }
                }
                isWaitingToResetCards = false;

                var remainingCards = CardDeck.Where(x => !x.GetComponent<CardLogic>().IsDiscarded).Count();
                if (remainingCards == 0)
                {
                    WinTheGame();
                }
            }
        }
    }

    public void WinTheGame()
    {
        var texts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var winSplash = texts.Where(x => x.gameObject.name == "Victory splash").FirstOrDefault();
        winSplash.gameObject.SetActive(true);
    }
}



