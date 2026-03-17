using WarGame.Core.Models;
using WarGame.Core.Services;

namespace WarGame.ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== WELCOME TO THE WAR CARD GAME ===\n");

        // 1. SETUP

        // Build and shuffle a fresh 52-card deck
        Deck gameDeck = new Deck();
        gameDeck.ShuffleDeck();

        // Ask how many players and initialise their empty hands
        PlayerHand players = new PlayerHand();
        players.AskNumberOfPlayers();

        // Deal all 52 cards round-robin; earlier players get any extra card
        players.DistributeCards(players, gameDeck);

        Pot        gamePot     = new Pot();
        PlayedCard playedCards = new PlayedCard(players);

        Console.WriteLine($"\nGame started with {players.NumberOfPlayers} players.");
        PrintCardCounts(players);
        Console.WriteLine();

        // 2. MAIN GAME LOOP 
        // Continue until only one player still holds cards, or 10 000 rounds pass
        const int RoundLimit = 10_000; // Same as 10000 it just looks pretty.
        int roundCounter = 1;

        while (CountActivePlayers(players) > 1 && roundCounter <= RoundLimit)
        {
            Console.WriteLine($"\n--- Round {roundCounter} ---");
            Console.WriteLine("Press any key to play next turn...");
            Console.ReadKey(intercept: true); // intercept: true hides the key press from the console
            Console.WriteLine();

            PlayCardLogic.CompareCards(playedCards, players, gamePot);

            roundCounter++;
        }

        // 3. GAME OVER 
        Console.WriteLine();

        if (roundCounter > RoundLimit)
        {
            // Round cap reached – winner is the player holding the most cards
            DeclareRoundLimitWinner(players, RoundLimit);
        }
        else
        {
            // Normal end – one player holds all the cards
            DeclareFinalWinner(players);
        }
    }

    // ── HELPER METHODS

    /// <summary>
    /// Returns the number of players who still have at least one card in hand.
    /// When this drops to 1 (or 0) the game ends.
    /// </summary>
    /// <param name="players">All players and their current hands.</param>
    /// <returns>Count of players with one or more cards remaining.</returns>
    static int CountActivePlayers(PlayerHand players)
    {
        int count = 0;
        foreach (var hand in players.PlayerHands.Values)
        {
            if (hand.CardsInHand.Count > 0) count++;
        }
        return count;
    }

    /// <summary>
    /// Prints the current card count for every player on one line.
    /// Example output: <c>Cards: P1=26, P2=13, P3=13</c>
    /// </summary>
    /// <param name="players">All players and their current hands.</param>
    static void PrintCardCounts(PlayerHand players)   
    {
        string counts = string.Join(", ", Enumerable.Range(1, players.NumberOfPlayers) 
                .Select(i => $"P{i}={players.PlayerHands[$"Hand Player {i}"].CardsInHand.Count}"));
        Console.WriteLine($"Cards: {counts}");   // Select applies secret foreach to each player.
    }

    /// <summary>
    /// Announces the winner when the game ends naturally (one player holds all 52 cards).
    /// </summary>
    /// <param name="players">All players and their current hands.</param>
    static void DeclareFinalWinner(PlayerHand players)
    {
        foreach (var entry in players.PlayerHands)
        {
            if (entry.Value.CardsInHand.Count > 0)
            {
                // Extract the player number from the key "Hand Player N"
                string playerNum = entry.Key.Replace("Hand Player ", "");
                Console.WriteLine($"************************************");
                Console.WriteLine($"*** THE FINAL WINNER IS PLAYER {playerNum}! ***");
                Console.WriteLine($"************************************");
                return;
            }
        }
    }

    /// <summary>
    /// Announces the result when the 10 000-round hard cap is reached.
    /// The player with the most cards wins; if multiple players are tied on card count,
    /// a draw is declared listing all tied players.
    /// </summary>
    /// <param name="players">All players and their current hands.</param>
    /// <param name="roundLimit">The round cap that was reached (used in the output message).</param>
    static void DeclareRoundLimitWinner(PlayerHand players, int roundLimit)
    {
        Console.WriteLine($"Round limit of {roundLimit} reached!");

        // Find the highest card count held by any player
        int maxCards = players.PlayerHands.Max(x => x.Value.CardsInHand.Count);

        // Collect all players who share that highest count
        var topPlayers = players.PlayerHands
            .Where(x => x.Value.CardsInHand.Count == maxCards)
            .ToList();

        if (topPlayers.Count == 1)
        {
            string playerNum = topPlayers.First().Key.Replace("Hand Player ", "");
            Console.WriteLine($"*** PLAYER {playerNum} WINS WITH {maxCards} CARDS! ***");
        }
        else
        {
            // Two or more players share the top card count → draw
            string drawPlayers = string.Join(", ",
                topPlayers.Select(p => p.Key.Replace("Hand Player ", "Player ")));
            Console.WriteLine($"*** IT'S A DRAW BETWEEN: {drawPlayers} (each holds {maxCards} cards) ***");
        }
    }
}