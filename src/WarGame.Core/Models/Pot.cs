namespace WarGame.Core.Models;

/// <summary>
/// Represents the shared pot that accumulates every card played in a round
/// (including all tiebreaker cards). The eventual round winner collects the entire pot.
/// </summary>
public class Pot
{
    /// <summary>
    /// All cards currently waiting in the pot.
    /// Cards are appended as they are played and removed in bulk when a winner is determined.
    /// </summary>
    public List<Card>? CardsInPot { get; set; } = new List<Card>();
}