using System.Collections.Generic;
using System;

public class DumbAI : AIPlayer{

    Random random = new Random();

    public override GameState GetMove(GameState gameState, int player){
        
        List<GameState> possibleNextStates = gameState.GetAllPossibleNextStates(player);

        return possibleNextStates[random.Next(0, possibleNextStates.Count)];
    }
}