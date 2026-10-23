namespace AUBE.Mechanics;

public static class DiceHandler
{
    /// <summary>
    /// Rolls a pool of dice and calculates successes based on the result.
    /// </summary>
    /// <param name="dieMaxFaces">The number of faces on the die (D6, D10, etc.)
    /// <param name="dicePool">The number of dice rolled at once.
    /// <param name="successThreshold">The number needed for a successful roll.
    /// <param name="critThreshold">The number needed for a critical success.
    /// <returns>Returns a tuple with (Number of NORMAL successes, Number of CRITICAL successes)</returns>
    public static (int successes, int criticalSuccesses) RollDice(
        int dieMaxFaces, int dicePool, int successThreshold, int critThreshold = -1)
    {
        var successes = 0;
        var criticalSuccesses = 0;

        for(int i=0; i<dicePool; i++)
        {
            var roll = Random.Shared.Next(1, dieMaxFaces + 1);

            if (roll >= successThreshold)
            {
                if (critThreshold > 0 && roll >= critThreshold)
                    criticalSuccesses++;
                else
                    successes++;
            }
        }

        return (successes, criticalSuccesses);
    }

    /// <summary>
    /// Rolls a single die and returns True or False based on the success threshold.
    /// </summary>
    /// <param name="dieMaxFaces">The number of faces on the die (D6, D10, etc.)
    /// <param name="successThreshold">The number needed for a successful roll.
    /// <returns>Returns a bool on whether or not it was a success.</returns>
    public static bool RollDie(int dieMaxFaces, int successThreshold)
    {
        var roll = Random.Shared.Next(1, dieMaxFaces + 1);

        return roll >= successThreshold;
    }
}