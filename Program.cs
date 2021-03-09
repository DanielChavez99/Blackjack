using System;
using System.Collections.Generic;
using static System.Console;

namespace Blackjack
{
    class Program
    {
        static bool gameOver = false;
        static void Main(string[] args)
        {
            WriteLine();

            //Create deck of cards
            Deck deck = new Deck();

            //Create two Lists to represent hands of the player and the dealer
            List<Card> playerHand = new List<Card>();
            List<Card> dealerHand = new List<Card>();

            //Deal two cards to the player and the dealer
            playerHand.Add(deck.DrawCard());
            dealerHand.Add(deck.DrawCard());
            playerHand.Add(deck.DrawCard());
            dealerHand.Add(deck.DrawCard());

            //Show initial hands for the player and the dealer
            ShowHand(dealerHand, "dealer", true);
            ShowHand(playerHand, "player");

            //Check if dealer's face up card is Ace
            if(dealerHand[0].GetValue() == Value.ace)
            {
                //Check if dealer has blackjack
                if(GetPoints(dealerHand) == 21)
                {
                    //Check if player also has blackjac;
                    if(GetPoints(playerHand) == 21)
                        WriteLine("\nDraw!");
                    //If not, dealer wins
                    else
                        WriteLine("\nThe dealer wins with a blackjack!");
                    gameOver = true;
                    ShowHand(dealerHand, "dealer");
                }
            }

            //Check if player has blackjack
            if(GetPoints(playerHand) == 21)
            {
                //Unflip dealer's hidden card 
                ShowHand(dealerHand, "dealer");

                //Check if dealer also has blackjack
                if(GetPoints(dealerHand) == 21)
                    WriteLine("\nDraw!");
                else
                    WriteLine("\nThe player wins with a blackjack!");
                gameOver = true;
            }

            //Enter game loop
            while(!gameOver)
            {
                //Ask user to hit or stand
                WriteLine("\nHit or Stand? (Enter H or S): ");
                char x = Console.ReadKey(true).KeyChar;

                //If player stands, dealer hits until his hand is in a range [17, 21] points or busts
                if(x == 'S' || x == 's')
                {
                    WriteLine("\nPlayer stands!");
                    Console.WriteLine();
                    ShowHand(dealerHand, "dealer");
                    while(GetPoints(dealerHand) <= GetPoints(playerHand))
                    {
                        WriteLine("\nThe dealer hits!");
                        dealerHand.Add(deck.DrawCard());
                        ShowHand(dealerHand, "dealer");
                    }
                    //ShowHand(dealerHand, "dealer");
                    CheckWin(dealerHand, "dealer", playerHand, "player");
                }
                //If player hits, check if his hand reaches 21 or busts; otherwise restart loop
                else if(x == 'H' || x == 'h')
                {
                    WriteLine("\nPlayer hits!");
                    playerHand.Add(deck.DrawCard());
                    ShowHand(playerHand, "player");
                    if(GetPoints(playerHand) >= 21)
                        CheckWin(playerHand, "player", dealerHand, "dealer");
                }
                //If player has no clue how to use a keyboard, this happens
                else
                {
                    WriteLine("\nWrong input dummy, try again!");
                }
            }
        }
        
        static void CheckWin(List<Card> hand, string handOwner, List<Card> opponentHand, string opponent)
        {
            int points = GetPoints(hand);
            if(points == 21)
            {
                if(GetPoints(opponentHand) == 21)
                    WriteLine("\nDraw!");
                else
                    WriteLine($"\nThe {handOwner} wins with a blackjack!");
                gameOver = true;
            }
            else if(points > 21)
            {
                WriteLine($"\nThe {handOwner} busts!");
                WriteLine($"\nThe {opponent} wins!");
                gameOver = true;
            }
            else
            {
                int opponentPoints = GetPoints(opponentHand);
                if(points > opponentPoints)
                    WriteLine($"\nThe {handOwner} wins!");
                else if(points < opponentPoints)
                    WriteLine($"\nThe {opponent} wins!");
                else
                    WriteLine($"\nDraw!");
                gameOver = true;
            }
        }

        static int GetPoints(List<Card> hand)
        {
            bool hasAce = false;
            int sum = 0;
            foreach(Card card in hand)
            {
                switch(card.GetValue())
                {
                    case Value.ace:
                        sum += 1;
                        hasAce = true;
                        break;
                    case Value.king: case Value.queen: case Value.jack:
                        sum += 10;
                        break;
                    default:
                        sum += (int)card.GetValue();
                        break;
                }
            }
            if(hasAce && sum + 10 <= 21)
                return sum + 10;
            else
                return sum;
        }

        static void ShowHand(List<Card> hand, string handOwner, bool cardHidden)
        {
            if(cardHidden)
            {
                WriteLine($"The {handOwner}'s hand: {hand[0]} (Face down card)");
            }
            else
            {
                ShowHand(hand, handOwner);
            }
        }

        static void ShowHand(List<Card> hand, string handOwner)
        {
            Console.Write($"The {handOwner}'s hand: ");
            foreach(Card card in hand)
            {
                Console.Write($"{card.ToString()} ");
            }
            Console.Write($"= {GetPoints(hand)} points\n");
        }
    }

    enum Suit {clubs, diamonds, hearts, spades};
    enum Value {two = 2, three, four, five, six, seven, eigth, nine, ten, jack, queen, king, ace};

    class Card
    {
        private Value value;
        private Suit suit;
        public Card(Value value, Suit suit)
        {
            this.value = value;
            this.suit = suit;
        }
        public Value GetValue() { return value; }
        public Suit GetSuit() { return suit; }

        public override string ToString()
        {
            return ValueToString(GetValue()) + SuitToString(GetSuit());
        }

        private string SuitToString(Suit suit)
        {
            switch(suit)
            {
                case Suit.clubs:
                    return "C";
                case Suit.diamonds:
                    return "D";
                case Suit.hearts:
                    return "H";
                case Suit.spades:
                    return "S";
                default:
                    WriteLine("Invalid suit");
                    return "";
            }
        }

        private string ValueToString(Value value)
        {
            switch(value)
            {
                case Value.ace:
                    return "A";
                case Value.king:
                    return "K";
                case Value.queen:
                    return "Q";
                case Value.jack:
                    return "J";
                default:
                    return ((int)value).ToString();
            }
        }
    }
    
    class Deck
    {
        private List<Card> deck = new List<Card>();
        public Deck()
        {
            foreach(Value value in Enum.GetValues(typeof(Value)))
            {
                foreach(Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    Card card = new Card(value, suit);
                    deck.Add(card);
                }
            }
        }

        public Card DrawCard()
        {
            Random random = new Random();
            int cardIndex = random.Next(0, deck.Count-1);
            Card cardToReturn = deck[cardIndex];
            deck.RemoveAt(cardIndex);

            return cardToReturn;
        }
    }
}
