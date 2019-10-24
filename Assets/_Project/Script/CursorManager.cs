using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour {

	public Texture2D attackCursor;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

	public void setAttack()
    {
        Cursor.SetCursor(attackCursor, hotSpot, cursorMode);
    }

	public void resetCursor()
	{
		Cursor.SetCursor(null, Vector2.zero, cursorMode);
	}
}
