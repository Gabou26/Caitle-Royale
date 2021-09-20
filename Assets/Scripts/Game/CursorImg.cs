using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorImg : MonoBehaviour
{
    public Texture2D cursor;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), cursorMode);
    }
}
