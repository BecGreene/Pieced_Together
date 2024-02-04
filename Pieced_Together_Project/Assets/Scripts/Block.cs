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
    public enum Direction
    {
        vertical = 0,
        horizontal = 1
    }
    public enum Type
    {
        Mystery,
        Normal
    }
    public Direction direction = Direction.vertical;
    public Type type = Type.Normal;
    public bool damaged = false;
    public Sprite[] sprites;
    private Vector3 pos;
    private bool dragging = false;
    private Camera cam;
    private Vector3 offset;
    private Vector2 horizClamp;
    private Vector2 vertClamp;
    private int XRange { get => BoardManager.Instance.Width - size; }
    private int YRange { get => (BoardManager.Instance.Height - size) * -1; }
    void Start()
    {
        cam = Camera.main;
        horizClamp = new Vector2(0, XRange);
        vertClamp = new Vector2(0, YRange);
    }
    private void OnMouseDrag()
    {
        if (!dragging) return;
        Vector3 moveVec = cam.ScreenToWorldPoint(Input.mousePosition) + offset;
        pos = transform.position;
        ClampVal(moveVec);
        transform.position = pos;
    }
    private void OnMouseDown()
    {
        offset = transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
        horizClamp = new Vector2(0, XRange);
        vertClamp = new Vector2(0, YRange);
        dragging = true;
    }
    private void OnMouseUp() => SnapToPlace();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!dragging) return;
        Vector2 cPos = collision.transform.position;
        if (direction == Direction.horizontal)
        {
            if(cPos.x > transform.position.x)
            {
                horizClamp.y = cPos.x - size;
            }
            else
            {
                horizClamp.x = cPos.x + 1;
            }
        }
        else
        {
            if (cPos.y > transform.position.y)
            {
                vertClamp.x = cPos.y - 1;
            }
            else
            {
                vertClamp.y = cPos.y + size;
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
        ClampVal(new Vector2((int)Math.Round(pos.x), (int)Math.Round(pos.y)));
        transform.position = pos;
    }

    private void ClampVal(Vector2 val)
    {
        switch (direction)
        {
            case Direction.horizontal:
                pos.x = Mathf.Clamp(val.x, horizClamp.x, horizClamp.y);
                break;
            case Direction.vertical:
                pos.y = Mathf.Clamp(val.y, vertClamp.y, vertClamp.x);
                break;
        }
    }
}
