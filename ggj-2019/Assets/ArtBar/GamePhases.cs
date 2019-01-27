public class GamePhases
{
    public enum GameplayPhase
    {
        FadeIn = 0,
        BadEventStart = 1,
        Evacuation = 2,     // Player Gameplay Input ON
        FloorEvacuationStart = 3,
        FloorEvacuationBreakPoint = 4,
        FloorEvacuationEnd = 5,
        PlayerJump = 6,     // Player Gameplay Input OFF
        PlayerInTruck = 7,
        TruckStart = 8,
        TruckStop = 9,
        DeEvacuation = 10, // Player Gameplay Input ON
        ItemShot = 11,
        LastItemShot = 12, // Player Gameplay Input OFF
        FadeOut = 13,
        FewDaysLater = 14,
        GameOver = 15,      // summary
        PlayerDie = 16,
        StartNewGame = 17,
        Summary = 18,
    }


}
