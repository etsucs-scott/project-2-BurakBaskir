using WarGame.Core.Enums;

namespace WarGame.Core.Models;

/// <summary>
/// Represents a standard 52-card deck (4 suits × 13 ranks).
/// After construction the deck is unshuffled; call <see cref="ShuffleDeck"/> before dealing.
/// </summary>
public class Deck
{
    private readonly Random _random = new Random();

    /// <summary>
    /// The flat list of all 52 cards used as the source for shuffling.
    /// </summary>
    public List<Card> InitialCards { get; private set; } = new List<Card>();

    /// <summary>
    /// The shuffled deck ready for dealing, represented as a <see cref="Stack{T}"/>.
    /// Cards are popped (dealt) from the top.
    /// </summary>
    public Stack<Card> ShuffledDeck { get; private set; } = new Stack<Card>();

    /// <summary>
    /// Initializes a new <see cref="Deck"/> and populates it with all 52 cards
    /// by iterating over every combination of <see cref="Suit"/> and <see cref="Rank"/>.
    /// </summary>
    public Deck()
    {
        // Enum.GetValues returns all defined values of the given enum type
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                InitialCards.Add(new Card(suit, rank));
            }
        }
    }

    /// <summary>
    /// Shuffles the deck by randomising the order of <see cref="InitialCards"/>
    /// using a random key sort, then loads the result into <see cref="ShuffledDeck"/>.
    /// </summary>
    /// <returns>This <see cref="Deck"/> instance, allowing method chaining.</returns>
    public Deck ShuffleDeck()
    {
        // OrderBy with a random key produces an unbiased shuffle
        InitialCards = InitialCards.OrderBy(_ => _random.Next()).ToList();
        ShuffledDeck = new Stack<Card>(InitialCards);
        return this;
    }
}