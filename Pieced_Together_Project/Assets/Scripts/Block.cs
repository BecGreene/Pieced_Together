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
        Normal,
        Fragile
    }

    public enum Barriers
    {
        Top,
        Bottom,
        Left,
        Right,
        None
    }
    public Direction direction = Direction.vertical;
    public Type type = Type.Normal;
    public Barriers barriers = Barriers.None;
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
    
    [SerializeField] AudioClip moveBlockClip;
    [SerializeField] AudioClip collideBlockClip;
    bool clipPlayed = false;

    private float sizeChange = 0.1f;
    private float sizeOffset;

    /// <summary>
    /// The high end of how far this block can move. For most blocks,
    /// that is the barrier at the edge of the board. For the target block,
    /// it's 300 because we will not have a board that is 300 wide.
    /// </summary>
    private int XRange;
    private int YRange;
    private int XRangeL;
    private int YRangeL;

    private float initX;
    private float initY;

    private Vector3 lastMousePos;
    private readonly float speedCap = 0.5f;
    public static bool Won = false;
    public int XSize;
    public int YSize;

    private int timesMoved = 0;
    [SerializeField] private int durability = 3;
    private SpriteRenderer sRenderer;
    private Vector3 winVec;
    public bool disabled = false;
    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        sizeOffset = sizeChange * (1/2f);
        initX = transform.position.x;
        initY = transform.position.y;
        YRange  = (type == Type.Target && barriers == Barriers.Top)    ? -300 : (BoardManager.Instance.Height - size) * -1;
        XRange  = (type == Type.Target && barriers == Barriers.Left)   ? 300 : BoardManager.Instance.Width - size;
        XRangeL = (type == Type.Target && barriers == Barriers.Right)  ? -300 : 0;
        YRangeL = (type == Type.Target && barriers == Barriers.Bottom) ? 300 : 0;
        xClamp = new Vector2(XRangeL, XRange);
        yClamp = new Vector2(YRangeL, YRange);
        XSize = direction == Direction.horizontal ? size : 1;
        YSize = direction == Direction.vertical ? size : 1;

        winVec = Vector3.zero;
        switch (barriers)
        {
            case Barriers.Bottom:
                winVec = new Vector3(0, 0.1f, 0);
                break;
            case Barriers.Top:
                winVec = new Vector3(0, -0.1f, 0);
                break;
            case Barriers.Left:
                winVec = new Vector3(0.1f, 0, 0);
                break;
            case Barriers.Right:
                winVec = new Vector3(-0.1f, 0, 0);
                break;
        }
    }
    /// <summary>
    /// Update is called every frame, but it will
    /// leave immediately if the block doesn't need to be updated.
    /// Otherwise, the block will take over control from the mouse
    /// </summary>
    private void Update()
    {
        /*if (!dragging && !Won)
        {
            SnapToPlace();
            return;
        }*/
        if (!dragging && (!Won || (Won && type != Type.Target))) return;
        if (Won && type == Type.Target)
        {
            transform.position += winVec;
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
        xClamp = new Vector2(XRangeL, XRange + (sizeOffset * (5 / 3f)));
        yClamp = new Vector2(YRangeL, YRange - (sizeOffset * (5 / 3f)));

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
        AddMove();
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
        ClampValPerfect(new Vector2((int)Math.Round(pos.x), (int)Math.Round(pos.y)));
        transform.position = pos;
    }
    private void AddMove()
    {
        //Possible add a move to the move counter
        if (transform.position != StartingPos)
        {
            BoardManager.UpdateMoves();
            timesMoved++;
            if(type == Type.Fragile && timesMoved >= 2)
            {
                disabled = true;
                sRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            /*if(!damaged && timesMoved >= durability)
            {
                BoardManager.UpdateDamaged();
                damaged = true;
            }
            if (damaged && sprites.Length >= 2)
            {
                sRenderer.sprite = sprites[1];
            }*/
        }
    }
    public void CheckWin()
    {
        if (type != Type.Target) return;
        switch (barriers)
        {
            case Barriers.Bottom:
                if (transform.position.y >= 1)
                {
                    BoardManager.Instance.DisplayWin();
                    Won = true;
                }
                break;
            case Barriers.Top:
                if (transform.position.y >= (BoardManager.Instance.Height - size + 1) * - 1)
                {
                    BoardManager.Instance.DisplayWin();
                    Won = true;
                }
                break;
            case Barriers.Left:
                if (transform.position.x >= BoardManager.Instance.Width - size + 1)
                {
                    BoardManager.Instance.DisplayWin();
                    Won = true;
                }
                break;
            case Barriers.Right:
                if (transform.position.x <= 0)
                {
                    BoardManager.Instance.DisplayWin();
                    Won = true;
                }
                break;
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
    private void ClampValPerfect(Vector2 val)
    {
        ClampVal(val);
        switch (direction)
        {
            case Direction.horizontal: pos.y = initY; break;
            case Direction.vertical  : pos.x = initX; break;
        }
    }
}
