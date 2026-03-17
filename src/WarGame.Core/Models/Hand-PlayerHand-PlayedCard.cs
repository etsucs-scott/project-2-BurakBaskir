namespace WarGame.Core.Models;

/// <summary>
/// Represents one player's hand of cards as a first-in-first-out queue.
/// Cards are added to the back (Enqueue) and drawn from the front (Dequeue).
/// </summary>
public class Hand
{
    /// <summary>The ordered collection of cards currently held by this player.</summary>
    public Queue<Card> CardsInHand { get; set; } = new Queue<Card>();
}

/// <summary>
/// Manages all players' hands and handles card distribution at the start of the game.
/// Internally uses a <see cref="Dictionary{TKey,TValue}"/> keyed by player label
/// (e.g. "Hand Player 1") so hands can be looked up by name throughout the game.
/// </summary>
public class PlayerHand
{
    /// <summary>Maps each player label to their <see cref="Hand"/>.</summary>
    public Dictionary<string, Hand> PlayerHands { get; set; } = new Dictionary<string, Hand>();

    private int _numberOfPlayers; // do not forget to ask if I should make this static or not!!!!!

    /// <summary>
    /// Gets or sets the number of players.
    /// Assigning this property also resets <see cref="PlayerHands"/>,
    /// creating one empty <see cref="Hand"/> entry per player.
    /// </summary>
    public int NumberOfPlayers 
    {
        get => _numberOfPlayers;
        set
        {
            _numberOfPlayers = value;

            // Rebuild the dictionary every time the player count is reset
            PlayerHands.Clear();
            for (int i = 1; i <= _numberOfPlayers; i++)
            {
                PlayerHands.Add($"Hand Player {i}", new Hand());
            }
        }
    }

    /// <summary>Initialises a <see cref="PlayerHand"/> with no players yet assigned.</summary>
    public PlayerHand() { }

    /// <summary>
    /// Prompts the user to enter the number of players (2–4) and stores the result.
    /// Loops until a valid integer in the accepted range is provided.
    /// </summary>
    /// <returns>The validated number of players (2, 3, or 4).</returns>
    public int AskNumberOfPlayers()
    {
        Console.WriteLine("How many players are playing? (2-4)");

        // TryParse avoids a crash on non-numeric input; range check enforces game rules
        if (int.TryParse(Console.ReadLine(), out int result) && result >= 2 && result <= 4)
        {
            NumberOfPlayers = result;
            return NumberOfPlayers;
        }

        Console.WriteLine("Invalid input. Please enter a whole number between 2 and 4.");
        return AskNumberOfPlayers(); // retry recursively
    }

    /// <summary>
    /// Deals all cards from <paramref name="gameDeck"/> to the players' hands
    /// in round-robin order (card 0 → Player 1, card 1 → Player 2, …).
    /// If the deck doesn't divide evenly the earlier players receive the extra cards,
    /// which is a natural consequence of round-robin dealing.
    /// </summary>
    /// <param name="players">The <see cref="PlayerHand"/> whose hands will receive cards.</param>
    /// <param name="gameDeck">The shuffled deck to deal from.</param>
    /// <returns>The same <paramref name="players"/> instance with all cards distributed.</returns>
    public PlayerHand DistributeCards(PlayerHand players, Deck gameDeck)
    {
        int cardIndex = 0;

        foreach (Card card in gameDeck.ShuffledDeck)
        {
            // Modulo maps each card to a player slot (0-based), then we add 1 for the key
            int playerNum = (cardIndex % NumberOfPlayers) + 1;
            players.PlayerHands[$"Hand Player {playerNum}"].CardsInHand.Enqueue(card);
            cardIndex++;
        }

        return players;
    }
}

/// <summary>
/// Tracks the single card each player has placed face-up in the current round.
/// A <c>null</c> value means that player is eliminated or has not played this round.
/// </summary>
public class PlayedCard
{
    /// <summary>
    /// Maps each player label (e.g. "Card Played by Player 1") to the card they played,
    /// or <c>null</c> if that player is eliminated / has no card this round.
    /// </summary>
    public Dictionary<string, Card?> PlayedCards { get; set; } = new Dictionary<string, Card?>();

    /// <summary>
    /// Initialises the dictionary with a <c>null</c> entry for every player.
    /// </summary>
    /// <param name="playersHand">The <see cref="PlayerHand"/> that defines how many players exist.</param>
    public PlayedCard(PlayerHand playersHand)
    {
        for (int i = 1; i <= playersHand.NumberOfPlayers; i++)
        {
            PlayedCards[$"Card Played by Player {i}"] = null;
        }
    }
}