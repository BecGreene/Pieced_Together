using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    /// <summary>
    /// The size of the long side of the block. The other side of the
    /// block is defaulted to 1
    /// </summary>
    [Range(2, 10)] public int size;
    /// <summary>
    /// Is it a horizontal or vertical piece
    /// </summary>
    public enum Direction
    {
        vertical = 0,
        horizontal = 1
    }
    /// <summary>
    /// The type of the block. Normal, heated, iced, etc.
    /// </summary>
    public enum Type
    {
        Mystery,
        Normal
    }
    public Direction direction = Direction.vertical;
    public Type type = Type.Normal;
    /// <summary>
    /// Has the block been damaged (fire was put next to ice, etc)
    /// </summary>
    public bool damaged = false;
    /// <summary>
    /// Normal and damaged sprites
    /// </summary>
    public Sprite[] sprites;
    private Vector3 pos;
    private bool dragging = false;
    private Camera cam;
    private Vector3 offset;
    private Vector2 xClamp;
    private Vector2 yClamp;
    /// <summary>
    /// This keeps track of if a block was actually moved after a drag, 
    /// or if the player just clicked it and never moved it.
    /// </summary>
    private Vector3 StartingPos;
    /// <summary>
    /// The high end of how far this block can move. For most blocks,
    /// that is the barrier at the edge of the board. For the target block,
    /// it's 300 because we will not have a board that is 300 wide.
    /// </summary>
    private int XRange { get
        {
            if (type != Type.Mystery) return BoardManager.Instance.Width - size;
            return 300;
        } }
    private int YRange { get => (BoardManager.Instance.Height - size) * -1; }
    void Start()
    {
        cam = Camera.main;
        xClamp = new Vector2(0, XRange);
        yClamp = new Vector2(0, YRange);
    }
    private void OnMouseDrag()
    {
        if (!dragging) return;
        //Move it
        Vector3 moveVec = cam.ScreenToWorldPoint(Input.mousePosition) + offset;
        pos = transform.position;
        //But don't let it leave the board or go through other blocks
        ClampVal(moveVec);
        transform.position = pos;
    }
    private void OnMouseDown()
    {
        offset = transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
        StartingPos = transform.position;
        //Reset clamps
        xClamp = new Vector2(0, XRange);
        yClamp = new Vector2(0, YRange);
        dragging = true;
    }
    private void OnMouseUp()
    {
        SnapToPlace();
        CheckWin();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!dragging) return;
        //Get pos of collided object, the change the clamps
        //based on where the other block is. This is to stop the
        //player from being able to frag the block through other blocks
        Vector2 cPos = collision.transform.position;
        if (direction == Direction.horizontal)
        {
            if(cPos.x > transform.position.x)
            {
                xClamp.y = cPos.x - size;
            }
            else
            {
                xClamp.x = cPos.x + 1;
            }
        }
        else
        {
            if (cPos.y > transform.position.y)
            {
                yClamp.x = cPos.y - 1;
            }
            else
            {
                yClamp.y = cPos.y + size;
            }
        }
    }
    /// <summary>
    /// Called when a block has stopped being dragged. 
    /// The math is currently based on the transform anchor being in the top left
    /// </summary>
    public void SnapToPlace()
    {
        dragging = false;
        pos = transform.position;
        //Round all vals so that they snap to the "grid". Then, make sure
        //The rounded position is in range
        ClampVal(new Vector2((int)Math.Round(pos.x), (int)Math.Round(pos.y)));
        transform.position = pos;

        //Possible add a move to the move counter
        if (transform.position != StartingPos) BoardManager.UpdateMoves();
    }
    public void CheckWin()
    {
        if (type != Type.Mystery) return;
        if(transform.position.x >= BoardManager.Instance.Width - size)
        {
            BoardManager.Instance.DisplayWin();
        }
    }

    private void ClampVal(Vector2 val)
    {
        switch (direction)
        {
            case Direction.horizontal:
                pos.x = Mathf.Clamp(val.x, xClamp.x, xClamp.y);
                break;
            case Direction.vertical:
                pos.y = Mathf.Clamp(val.y, yClamp.y, yClamp.x);
                break;
        }
    }
}
