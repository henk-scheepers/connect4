using System.Collections.Generic;

public abstract class AIPlayer{
    public abstract int GetMove(GameState gameState, int player);
}

// public interface IAIPlayer{
//     int GetMove(GameState gameState, int player);
// }

// public class AIDumb : IAIPlayer{

//     public int GetMove(GameState gameState, int player){
        
//         List<GameState> possibleNextStates = gameState.GetAllPossibleNextStates(player);

//         return possibleNextStates[Random.Range(0, possibleNextStates.Count)].LastMove;
//     }
// }