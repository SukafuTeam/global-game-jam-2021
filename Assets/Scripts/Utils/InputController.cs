using UnityEngine;

public class InputController {

	public static bool Up
	{
		get { return Input.GetKeyDown(KeyCode.UpArrow); }
	}
	
	public static bool Right
	{
		get { return Input.GetKeyDown(KeyCode.RightArrow); }
	}
	
	public static bool Down
	{
		get { return Input.GetKeyDown(KeyCode.DownArrow); }
	}
	
	public static bool Left
	{
		get { return Input.GetKeyDown(KeyCode.LeftArrow); }
	}

	public static bool Start
	{
		get { return Input.GetKeyDown(KeyCode.Space); }
	}

	public static bool MultiPress
	{
		get
		{
			var amount = (Up ? 1 : 0) + (Right ? 1 : 0) + (Down ? 1 : 0) + (Left ? 1 : 0);
			return amount < 2;
		}
	}

	public static bool AnyPress
	{
		get { return Up || Right || Down || Left; }
	}
}
