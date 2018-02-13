using UnityEngine;
using System.Collections;

public class InputHandler
{
	
	private float defaultMoveCooldown;
	private float fastMoveCooldown; 
	private float defaultFallTime; 
	private float rapidFallTime;

	public InputHandler(float defaultMoveCooldown, float fastMoveCooldown)
	{
		this.defaultMoveCooldown = defaultMoveCooldown;
		this.fastMoveCooldown = fastMoveCooldown;
	}

	public void HandleInput(Tetramino currentTetramino, ref float moveCooldown)
	{
		// All our input (for now) handles the tetramino, so if there is none return immediately
		if(currentTetramino == null || currentTetramino.Equals(null))
			return;
		
		// If we have pressed the button just now, set the cooldown for fast move and move the tetramino to the left
		if(Input.GetKeyDown("left"))
		{
			moveCooldown = defaultMoveCooldown;
			currentTetramino.MoveSideways(-1);
		}
		// If we're holding the button long enough since it was pressed start moving rapidly to the left
		else if(Input.GetKey("left") && moveCooldown <= 0.0f)
		{
			moveCooldown = fastMoveCooldown;
			currentTetramino.MoveSideways(-1);
		}
		
		// Same as above but for the right
		if(Input.GetKeyDown("right"))
		{
			moveCooldown = defaultMoveCooldown;
			currentTetramino.MoveSideways(1);
		}
		else if(Input.GetKey("right") && moveCooldown <= 0.0f)
		{
			moveCooldown = fastMoveCooldown;
			currentTetramino.MoveSideways(1);
		}
		
		// If we pressed the button just now start falling rapidly. The switch gets reset with SpawnTetramino(), so the player doesn't accidentaly fuck himself over
		if(Input.GetKeyDown("down"))
		{
			currentTetramino.FallRapid(true);
		}
		
		// Go back to the default falling speed
		if(Input.GetKeyUp("down"))
		{
			currentTetramino.FallRapid(false);
		}
		// Rotate the tetramino
		if(Input.GetKeyDown("r"))
			currentTetramino.Rotate();
	}
}
