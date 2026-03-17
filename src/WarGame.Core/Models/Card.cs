using WarGame.Core.Enums;

namespace WarGame.Core.Models;

/// <summary>
/// Represents a single playing card with a <see cref="Suit"/> and a <see cref="Rank"/>.
/// Cards are compared solely by <see cref="Rank"/> during gameplay; suits are ignored.
/// </summary>
public class Card
{
    /// <summary>Gets or sets the suit of this card (Hearts, Diamonds, Clubs, Spades).</summary>
    public Suit Suit { get; set; }

    /// <summary>Gets or sets the rank of this card (Two through Ace).</summary>
    public Rank Rank { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="Card"/> with the given suit and rank.
    /// </summary>
    /// <param name="suit">The suit of the card.</param>
    /// <param name="rank">The rank of the card.</param>
    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    /// <summary>
    /// Returns an abbreviated display string for the card rank.
    /// Face cards and Ace use their letter symbol (J, Q, K, A);
    /// numbered cards display their numeric value (2–10).
    /// </summary>
    /// <returns>A short string such as "K", "A", "7", or "10".</returns>
    public string DisplayRank() => Rank switch
    {
        Rank.Jack  => "J",
        Rank.Queen => "Q",
        Rank.King  => "K",
        Rank.Ace   => "A",
        _          => ((int)Rank).ToString() // Cast enum to int to get numeric value for numbered cards
    };

    /// <summary>
    /// Returns a human-readable description of the card, e.g. "King of Spades".
    /// </summary>
    public override string ToString() => $"{Rank} of {Suit}";
}