using System.Collections.Generic;
using System;

public class AIDumb : AIPlayer{

    Random random = new Random();

    public override int GetMove(GameState gameState, int player){
        
        List<GameState> possibleNextStates = gameState.GetAllPossibleNextStates(player);

        return possibleNextStates[random.Next(0, possibleNextStates.Count)].LastMove;
    }
}