using WarGame.Core.Models;
using WarGame.Core.Enums; 
namespace WarGame.Core.Services;

/// <summary>
/// Provides the core game logic for the War card game: playing cards, comparing ranks,
/// resolving ties through tiebreaker rounds, and awarding the pot to the winner.
/// </summary>
public class PlayCardLogic
{
    /// <summary>
    /// Executes one full round of War:
    /// <list type="number">
    ///   <item>Every active player reveals their top card; all cards go into the pot.</item>
    ///   <item>The player with the highest rank wins the pot.</item>
    ///   <item>On a tie for the highest rank, only the tied players play a tiebreaker.
    ///         Non-tied players' cards stay in the pot.</item>
    ///   <item>Tiebreakers repeat recursively until one player wins or all tied players
    ///         are eliminated.</item>
    /// </list>
    /// </summary>
    /// <param name="playedCards">Tracks which card each player has placed this round.</param>
    /// <param name="players">All players and their current hands.</param>
    /// <param name="pot">The shared pot accumulating cards until a winner is decided.</param>
    public static void CompareCards(PlayedCard playedCards, PlayerHand players, Pot pot)
    {
        // STEP 1: reset the played-card tracker for this fresh round ──────
        for (int i = 1; i <= players.NumberOfPlayers; i++)
            playedCards.PlayedCards[$"Card Played by Player {i}"] = null; // null resets the dictionary states.

        // STEP 2: each player dequeues their top card into the pot 
        for (int i = 1; i <= players.NumberOfPlayers; i++)
        {
            var hand = players.PlayerHands[$"Hand Player {i}"];

            if (hand.CardsInHand.Count > 0)
            {
                Card card = hand.CardsInHand.Dequeue();
                playedCards.PlayedCards[$"Card Played by Player {i}"] = card;
                pot.CardsInPot.Add(card);                      // all cards go to pot immediately
                Console.WriteLine($"Player {i}: {card.DisplayRank()}");
            }
            else
            {
                // Null entry signals this player is eliminated
                playedCards.PlayedCards[$"Card Played by Player {i}"] = null;
                Console.WriteLine($"Player {i}: (eliminated - no cards)");
            }
        }

        // STEP 3: determine the round winner 

        // Only consider entries where a card was actually played
        var activeEntries = playedCards.PlayedCards
            .Where(x => x.Value != null)
            .ToList();

        if (!activeEntries.Any())
        {
            Console.WriteLine("No active players remaining.");
            return;
        }

        Rank highestRank  = activeEntries.Max(x => x.Value!.Rank);
        var  tiedEntries  = activeEntries.Where(x => x.Value!.Rank == highestRank).ToList();

        string? winnerNum;

        if (tiedEntries.Count == 1)
        {
            // Clear winner – extract the player number from the dictionary key
            winnerNum = tiedEntries.First().Key.Replace("Card Played by Player ", "");
        }
        else
        {
            // Two or more players tied → kick off a War tiebreaker
            var tiedNums = tiedEntries
                .Select(x => x.Key.Replace("Card Played by Player ", ""))
                .ToList();

            Console.WriteLine($"\nTie between {string.Join(" and ", tiedNums.Select(n => "Player " + n))}!");
            Console.WriteLine($"Pot includes: {string.Join(", ", pot.CardsInPot.Select(c => c.DisplayRank()))}");

            // ResolveTiebreaker returns the winner's player number, or null if all are eliminated
            winnerNum = ResolveTiebreaker(tiedNums, players, pot);
        }

        // STEP 4: award the pot 
        if (winnerNum != null)
        {
            AwardPot(winnerNum, players, pot);
        }
        else
        {
            // All tied players ran out of cards simultaneously – discard the pot
            Console.WriteLine("All tied players were eliminated — pot is discarded.");
            pot.CardsInPot.Clear();
        }
    }

    /// <summary>
    /// Recursively plays tiebreaker rounds among only the players currently tied.
    /// Each call deals one additional face-up card per tied player into the pot.
    /// Non-tied players do NOT participate and their cards remain in the pot untouched.
    /// </summary>
    /// <param name="tiedNums">
    /// Player numbers (as strings, e.g. "1", "3") still competing in this tiebreaker.
    /// </param>
    /// <param name="players">All players and their current hands.</param>
    /// <param name="pot">The shared pot to accumulate tiebreaker cards.</param>
    /// <returns>
    /// The winning player's number as a string (e.g. "1"), or <c>null</c>
    /// if every tied player was eliminated before a winner could be decided.
    /// </returns>
    private static string? ResolveTiebreaker(List<string> tiedNums, PlayerHand players, Pot pot)
    {
        var tbCards = new Dictionary<string, Card?>();
        var outputParts = new List<string>(); // builds the "Player 1: K | Player 3: 9" line

        foreach (var num in tiedNums)
        {
            var hand = players.PlayerHands[$"Hand Player {num}"];

            if (hand.CardsInHand.Count > 0)
            {
                Card card = hand.CardsInHand.Dequeue();
                tbCards[num] = card;
                pot.CardsInPot.Add(card);                      // tiebreaker cards also go to pot
                outputParts.Add($"Player {num}: {card.DisplayRank()}");
            }
            else
            {
                // This tied player cannot play a tiebreaker card → they are eliminated
                tbCards[num] = null;
                outputParts.Add($"Player {num}: (eliminated)");
            }
        }

        Console.WriteLine($"Tiebreaker: {string.Join(" | ", outputParts)}");

        var active = tbCards.Where(x => x.Value != null).ToList();
        if (!active.Any()) return null; // every tied player ran out of cards

        Rank maxRank   = active.Max(x => x.Value!.Rank);
        var stillTied  = active.Where(x => x.Value!.Rank == maxRank).ToList();

        if (stillTied.Count == 1)
        {
            string winner = stillTied.First().Key;
            Console.WriteLine($"Tiebreaker winner: Player {winner} ({stillTied.First().Value!.DisplayRank()})");
            return winner;
        }

        // More than one player tied again → recurse with only those players
        var nextTied = stillTied.Select(x => x.Key).ToList();
        Console.WriteLine(
            $"Still tied! Another tiebreaker between " +
            $"{string.Join(" and ", nextTied.Select(n => "Player " + n))}...");

        return ResolveTiebreaker(nextTied, players, pot);
    }

    /// <summary>
    /// Transfers every card in <paramref name="pot"/> to the back of the winner's hand queue,
    /// then clears the pot. Prints the winner line including updated card counts for all players.
    /// </summary>
    /// <param name="winnerNum">The winning player's number as a string (e.g. "1").</param>
    /// <param name="players">All players and their current hands.</param>
    /// <param name="pot">The pot to drain into the winner's hand.</param>
    private static void AwardPot(string winnerNum, PlayerHand players, Pot pot)
    {
        // Move all pot cards to the back of the winner's queue (order preserved)
        foreach (Card card in pot.CardsInPot)
            players.PlayerHands[$"Hand Player {winnerNum}"].CardsInHand.Enqueue(card);

        pot.CardsInPot.Clear();

        // Build card-count summary AFTER adding the pot so counts are accurate
        string counts = string.Join(", ",
            Enumerable.Range(1, players.NumberOfPlayers)
                .Select(i => $"P{i}={players.PlayerHands[$"Hand Player {i}"].CardsInHand.Count}"));

        Console.WriteLine($"Winner: Player {winnerNum} (Cards: {counts})");
    }
}