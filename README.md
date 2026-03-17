# Project 2: War Game

A text-based War card game built in C#. The project separates the game into two main parts: the game domain logic (Core library) and the user interface (Console application). // Test files were created but not implemented.

## Git Usage & Setup

To get a local copy up and running, follow these steps:

1. Clone the repository:

git clone git@github.com:etsucs-scott/project-2-BurakBaskir.git
cd project-2-BurakBaskir

2. Build the solution:   

dotnet build

3. Run the game: 

dotnet run --project src/WarGame.Console

## Game Logic

**Controls & Display**
The game runs automatically in the console. There are no manual controls required from the player.

The console displays each round’s results.

Card counts for each player are updated dynamically.

The game continues until a win condition is met or the round limit is reached(10_000).

**Game Rules & Mechanics**

**Players**

The game supports 2 to 4 players.
Each player starts with an equal number of cards.

**Basic Gameplay**

Each round, all players reveal their top card.
The player with the highest card wins the round.
The winner collects all played cards and adds them to their deck.

**Tie Mechanic**

If two or more players play cards of equal value:
A tie (war) occurs.
Instead of placing cards face-down, each tied player plays one additional face-up card.*(This is Different than Original Mechanic.)*
The highest of these new cards determines the winner.
This process repeats if ties continue.

**Round Limit**

The game has a maximum of 10,000 rounds.
If the limit is reached:
The game ends automatically.
The player with the most cards is declared the winner.

**Win Condition**

A player wins by:
Collecting all cards in the game, or
Having the most cards when the round limit is reached.

**Lose Condition**

A player is eliminated when They run out of cards.