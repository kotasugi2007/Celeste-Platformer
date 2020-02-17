using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask oneWay;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool onTop;
    public int wallSide;

    [Space]

    [Header("Collision")]
    public Vector2 verticalSize, horizontalSize;
    public Vector2 bottomOffset, rightOffset, leftOffset, topOffest;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
        
        onGround = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, verticalSize, 0, groundLayer) || Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, verticalSize, 0, oneWay);
        onWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, horizontalSize, 0, groundLayer) 
            || Physics2D.OverlapBox((Vector2)transform.position + leftOffset, horizontalSize, 0, groundLayer);

        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightOffset, horizontalSize, 0, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftOffset, horizontalSize, 0, groundLayer);

        onTop = Physics2D.OverlapBox((Vector2)transform.position + topOffest, verticalSize, 0, groundLayer);

        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireCube((Vector2)transform.position  + bottomOffset, verticalSize); 
        Gizmos.DrawWireCube((Vector2)transform.position + rightOffset, horizontalSize);
        Gizmos.DrawWireCube((Vector2)transform.position + leftOffset, horizontalSize);
        Gizmos.DrawWireCube((Vector2)transform.position + topOffest, verticalSize);
    }
}