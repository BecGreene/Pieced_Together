using System;
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
        Target,
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
    [SerializeField] AudioClip moveBlockClip;
    [SerializeField] AudioClip collideBlockClip;
    bool clipPlayed = false;

    private float sizeChange = 0.1f;
    private float sizeOffset;

    private int XRange;
    private int YRange;

    private Vector3 lastMousePos;
    private readonly float speedCap = 0.5f;
    public static bool Won = false;
    public int XSize;
    public int YSize;
    void Start()
    {
        cam = Camera.main;
        sizeOffset = sizeChange / 2f;
        YRange = (BoardManager.Instance.Height - size) * -1;
        XRange = (type != Type.Target) ? BoardManager.Instance.Width - size : 300;
        xClamp = new Vector2(0, XRange);
        yClamp = new Vector2(0, YRange);
        XSize = direction == Direction.horizontal ? size : 1;
        YSize = direction == Direction.vertical ? size : 1;
    }
    /// <summary>
    /// Update is called every frame, but it will
    /// leave immediately if the block doesn't need to be updated.
    /// Otherwise, the block will take over control from the mouse
    /// </summary>
    private void Update()
    {
        if (!dragging && (!Won || (Won && type != Type.Target))) return;
        if (Won && type == Type.Target)
        {
            transform.position += new Vector3(0.1f, 0, 0);
            return;
        }
        //Get change in mouse pos
        Vector3 delta = Input.mousePosition - lastMousePos;
        delta.z = 0;
        //If the mouse didn't move, leave
        if (delta.magnitude == 0) return;

        //Get the change in WORLD SPACE from the mouse pos
        delta = cam.ScreenToWorldPoint(Input.mousePosition) - cam.ScreenToWorldPoint(lastMousePos);

        //We only care about 1 axis
        if      (direction == Direction.horizontal) delta.y = 0;
        else if (direction == Direction.vertical  ) delta.x = 0;
        //If the WORLD SPACE movement is too large of a change, cap it
        if (delta.magnitude >= speedCap) delta = delta.normalized * speedCap;
        //Update the last mouse pos
        lastMousePos = Input.mousePosition;

        //Add change to pos
        pos = transform.position;
        pos += delta;
        //Clamp it
        ClampVal(pos);
        //Update transform
        transform.position = pos;

        if (!clipPlayed)
        {
            AudioSource.PlayClipAtPoint(moveBlockClip, cam.transform.position);
            clipPlayed = true;
        }

        //Update cursor position to match the change made
        CollisionCursor.MoveTo(cam.WorldToScreenPoint(pos - offset));

        //Check win
        CheckWin();
    }
    /// <summary>
    /// The new "OnMouseDown" called by the <see cref="CollisionCursor"/>
    /// </summary>
    public /*Direction*/void CallMouseDown()
    {
        //Reset clamps
        xClamp = new Vector2(0, XRange);
        yClamp = new Vector2(0, YRange);

        //Update bools
        dragging = true;
        clipPlayed = false;

        //Update scaling and grab it's current pos & offset
        StartingPos = transform.position;
        transform.localScale -= new Vector3(sizeChange, sizeChange, 0);
        transform.position += new Vector3(sizeOffset, (sizeOffset * -1), 0);
        offset = transform.position - cam.ScreenToWorldPoint(CollisionCursor.Instance.collPos);

        //Set the lastMousePos
        lastMousePos = Input.mousePosition;

        //return direction;
    }
    /// <summary>
    /// The new "OnMouseUp"
    /// </summary>
    public void CallMouseUp()
    {
        //Change scaling back to normal
        transform.localScale = Vector3.one;//new Vector3(transform.localScale.x + sizeChange, transform.localScale.y + sizeChange, 1);
        transform.position -= new Vector3(sizeOffset, (sizeOffset * -1), 0);
        
        //Snap
        SnapToPlace();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!dragging || !collision.gameObject.CompareTag("Block")) return;
        AudioSource.PlayClipAtPoint(collideBlockClip, cam.transform.position);
        //Get pos of collided object, the change the clamps
        //based on where the other block is. This is to stop the
        //player from being able to drag the block through other blocks
        Vector2 cPos = collision.transform.position;
        Block collScript = collision.gameObject.GetComponent<Block>();
        if (direction == Direction.horizontal)
        {
            if(cPos.x > transform.position.x)
            {
                xClamp.y = cPos.x - size + sizeOffset;
            }
            else
            {
                xClamp.x = cPos.x + collScript.XSize;
            }
        }
        else
        {
            if (cPos.y > transform.position.y)
            {
                yClamp.x = cPos.y - collScript.YSize;
            }
            else
            {
                yClamp.y = cPos.y + size - sizeOffset;
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
        if (type != Type.Target) return;
        if(transform.position.x >= BoardManager.Instance.Width - size + 1)
        {
            BoardManager.Instance.DisplayWin();
            Won = true;
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
