using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CollisionCursor : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out int X, out int Y);
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);
    private Vector3 lastMousePos;
    public Vector3 collPos;
    private Camera cam;
    private int XRange => BoardManager.Instance.Width;

    private int YRange => BoardManager.Instance.Height * -1;
    private Vector2 xClamp;
    private Vector2 yClamp;
    public Texture2D[] CursorImages;
    private GameObject collidedBlock;
    private Block collidedScript;
    public static CollisionCursor Instance;
    //private int SpeedCap = 25;
    //private Block.Direction direction = Block.Direction.none;
    private bool dragging = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //Cursor.visible = false;
        //SetCursorPos(960, 600);
        cam = Camera.main;
        lastMousePos = Input.mousePosition;
        collPos = lastMousePos;
        Cursor.SetCursor(CursorImages[0], Vector2.zero, CursorMode.Auto);
        xClamp = new Vector2(0, XRange);
        yClamp = new Vector2(0, YRange);
    }

    // Update is called once per frame
    void Update()
    {
        //Cursor.visible = false;
        
        //Call Events
        if(collidedBlock != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                /*direction = */collidedScript.CallMouseDown();
                dragging = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                collidedScript.CallMouseUp();
                //direction = Block.Direction.none;
                dragging = false;
                lastMousePos = Input.mousePosition;
                collPos = cam.WorldToScreenPoint(transform.position);
                return;
            }

        }
        if (dragging && Input.GetMouseButton(0)) return;

        Vector3 delta = Input.mousePosition - lastMousePos;
        if (delta == Vector3.zero) return;
        //if (delta.magnitude >= SpeedCap) delta = delta.normalized * SpeedCap;
        /*if(direction == Block.Direction.horizontal)
        {
            delta.y = 0;
        }
        else if (direction == Block.Direction.vertical)
        {
            delta.x = 0;
        }*/
        lastMousePos = Input.mousePosition;
        collPos += delta;
        Vector3 worldPos = cam.ScreenToWorldPoint(collPos);
        worldPos.z = 1;
        worldPos = new Vector3(Mathf.Clamp(worldPos.x, xClamp.x, xClamp.y), Mathf.Clamp(worldPos.y, yClamp.y, yClamp.x), 1);
        transform.position = worldPos;
        collPos = cam.WorldToScreenPoint(worldPos);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Block") || dragging || Block.Won) return;
        collidedBlock = collision.gameObject;
        collidedScript = collidedBlock.GetComponent<Block>();
        Cursor.SetCursor(CursorImages[1], Vector2.zero, CursorMode.Auto);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Block")) return;
        if (collidedBlock == null || collidedBlock != collision.gameObject) return;
        Cursor.SetCursor(CursorImages[0], Vector2.zero, CursorMode.Auto);
        if (!Input.GetMouseButton(0) && !dragging)
        {
            collidedBlock = null;
            collidedScript = null;
        }
    }
    public static void MoveTo(Vector3 pos) => Instance.MoveTo_P(pos);
    private void MoveTo_P(Vector3 pos)
    {
        lastMousePos = pos;
        collPos = pos;
        transform.position = cam.ScreenToWorldPoint(pos);
    }
}
