using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorImg : MonoBehaviour
{
    public Texture2D cursor;
    [SerializeField] private PlayerInput playerInput;
    public CursorMode cursorMode = CursorMode.Auto;

    void Start()
    {
        //Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), cursorMode);
    }

    void Update()
    {
        //Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), cursorMode);
        //Cursor.lockState = CursorLockMode.None;
    }

   /* private void OnFire(InputValue inputValue)
    {
        Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.height / 2), cursorMode);
    }*/
}
