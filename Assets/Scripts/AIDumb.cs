using System.Collections.Generic;
using UnityEngine;

public class AIDumb : AIPlayer{

    public override int GetMove(GameState gameState, int player){
        
        List<GameState> possibleNextStates = gameState.GetAllPossibleNextStates(player);

        return possibleNextStates[Random.Range(0, possibleNextStates.Count)].LastMove;
    }
}