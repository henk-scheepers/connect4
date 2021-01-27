using System.Collections.Generic;

public abstract class AIPlayer{
    public abstract GameState GetMove(GameState gameState, int player);
}