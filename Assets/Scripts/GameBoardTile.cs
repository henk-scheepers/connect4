using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardTile : MonoBehaviour
{
    public delegate void TileClickAction(Vector2Int position);
    public static event TileClickAction OnTileClicked;

    SpriteRenderer renderer;

    [SerializeField] GameObject dot;
    Vector2Int position;

    TileType type;
    
    // Start is called before the first frame update
    void Start()
    {
        renderer = dot.GetComponent<SpriteRenderer>();
        renderer.enabled = false;
    }

    public void SetPosition(int x, int y){
        position.x = x;
        position.y = y;
    }


    public TileType Type{ 
        get {
            return type;
        }
        set {
            type = value;
            bool isEmpty = false;

            if(type == TileType.EMPTY){
                isEmpty = true;
            }
            else if(type == TileType.RED){
                renderer.color = Color.red;
            }
            else if(type == TileType.BLUE){
                renderer.color = Color.blue;
            }

            renderer.enabled = !isEmpty;
        }
    }

    void OnMouseDown() { 
        if(OnTileClicked != null){
            OnTileClicked(position);
        }
    }
}

public enum TileType{
    EMPTY = 0,
    RED = 1,
    BLUE = 2
}
