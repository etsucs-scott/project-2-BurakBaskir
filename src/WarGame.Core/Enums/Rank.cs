namespace WarGame.Core.Enums;
    /// <summary>
    /// Represents the rank of a playing card.
    /// Integer values allow direct comparison: higher value = higher rank.
    /// Ace (14) beats King (13), which beats Queen (12), and so on down to Two (2).
    /// </summary>
    public enum Rank 
    {
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14
    }