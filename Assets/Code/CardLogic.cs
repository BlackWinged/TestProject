using UnityEngine;

public class CardLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
