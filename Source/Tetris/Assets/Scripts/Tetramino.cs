using UnityEngine;
using System.Collections;

public class Tetramino : MonoBehaviour 
{
	// If the Tetramino is inactive it doesn't do anything, no updates, no movement
	public bool isActive;

	// Transform cache
	public Transform mytransform;

	// Dimensions of the tetramino
	public Vector2 dimensions;

	// Default time to fall one tile down
	private float defaultFallTime;
	// Time to fall in rapid fall mode while holding the down arrow key
	private float rapidFallTime;
	// Current time to fall
	private float fallTimer;
	// Are we falling rapidly now
	private bool rapidFall;

	// All the tiles this tetramino is made of
	public Tile[] tiles;

	// Number of this tetramino
	public int myNumber;

	// Tools for fixing rotation offset
	private int currentFrame;
	public Vector3[] rotationFrames;
	private Vector3 rotationOverlapFixOffset;

	// Values for the small move window you get after the tetramino hits the floor
	public float moveWindow;
	private float moveWindowTimer;
	public	bool isDying;

	public float rapidBlockTimer;

	public float DefaultFallTime
	{
		set { defaultFallTime = value; }
	}

	public float RapidFallTime 
	{
		set { rapidFallTime = value; }
	}

	// Set the basic values and cache the transform
	void Awake () 
	{
		rotationOverlapFixOffset = Vector3.zero;
		currentFrame = 0;

		mytransform = this.transform;

		isDying = false;
		fallTimer = 0.0f;
		moveWindowTimer = 0.0f;

		return;
	}

	// Handles falling down
	void Update () 
	{
		if(!isActive)
			return;
		HandleGhost();
		if(isDying)
		{
			moveWindowTimer -= Time.deltaTime;
			killMe();
			return;
		}

		if(rapidBlockTimer > 0.0f)
			rapidBlockTimer -= Time.deltaTime;

		if(rapidFall)
			fallTimer = Mathf.Min(fallTimer, rapidFallTime);

		// Update the timer
		if(fallTimer > 0.0f)
			fallTimer -= Time.deltaTime;

		// If we should fall, call appropriate function and set the timer to currently used fall time
		if(fallTimer <= 0.0f)
		{
			MoveDown();
			if(rapidFall && rapidBlockTimer <= 0.0f)
				fallTimer = rapidFallTime;
			else
				fallTimer = defaultFallTime;
		}
		return;
	}

	// Try to move right, if can't disregard the call
	public void MoveSideways(int ammount)
	{		
		if(!isActive)
			return;

		// Check all the tiles we're made of if they can move
		Vector3 move = new Vector3(ammount, 0.0f, 0.0f);
		bool canMove = true;

		foreach(Tile tile in tiles)
			canMove &= tile.CanMove(move);

		// If any of them can't (or we don't exist, just safety) don't move, otherwise do
		if(canMove && mytransform != null)
		{
			SoundManager.PlaySound("snd_move");
			mytransform.position += new Vector3(ammount, 0, 0);
			// This thing fixes our postion when we rotate near a wall, but now we are certainly not near any
			// (since we moved), so discard that value
			rotationOverlapFixOffset = Vector3.zero;
			UpdateTilePos();
		}
		return;
	}

	// Same as above, but if we can't fall we kill ourselves since we hit the floor
	public void MoveDown()
	{		
		if(!isActive)
			return;

		Vector3 move = new Vector3(0.0f, -1.0f, 0.0f);
		bool canMove = true;
		
		foreach(Tile tile in tiles)
			canMove &= tile.CanMove(move);
		
		if(canMove && mytransform != null)
		{
			mytransform.position += new Vector3(0, -1.0f, 0);
			UpdateTilePos();
		}
		else
		{
			isDying = true;
			moveWindowTimer = moveWindow;
			killMe();
		}
		return;
	}

	// Set the move mode to rapid/normal and update the timer if rapid
	public void FallRapid(bool rapidFall)
	{
		this.rapidFall = rapidFall;

		// Make sure we immediately start falling if we went into rapid mode
		if(rapidFall)
			fallTimer = Mathf.Min(fallTimer, rapidFallTime);

		return;
	}

	Vector3[] moves;

	// Rotate the tetramino and fix offset
	public void Rotate()
	{		
		if(!isActive)
			return;

		if(moves == null)
		{
			moves = new Vector3[5] { new Vector3(0.0f, 0.0f, 0.0f),
				new Vector3(1.0f, 0.0f, 0.0f),
				new Vector3(-1.0f, 0.0f, 0.0f),
				new Vector3(2.0f, 0.0f, 0.0f),
				new Vector3(-2.0f, 0.0f, 0.0f) };
		}

		// Revert any earlier offsets
		Vector3 prevOffset = rotationOverlapFixOffset;
		mytransform.position -= rotationOverlapFixOffset;
		rotationOverlapFixOffset = Vector3.zero;

		// Rotate the transform
		mytransform.Rotate(new Vector3(0.0f, 0.0f, -90.0f));

		/* This bit requiers some explanation
		 * The rotationFrames are hard-coded values in the Unity tetramino prefabs that fix our position after 
		 * a rotation. This is needed since Transform.Rotate() rotates around the very center of the object, 
		 * which in most cases with our tetraminos is in the middle of nowhere. Thanks to these frames we are 
		 * able to remain in line with the grid, i.e. the T-shaped tetramino rotates nicely around its center 
		 * tile instead of some weird point that makes no sense gameplay-wise.
		 * We keep track of current frame, revert the previous one after any rotation and apply the next one
		 */
		mytransform.position -= rotationFrames[currentFrame];
		currentFrame = (currentFrame + 1) % rotationFrames.Length;
		mytransform.position += rotationFrames[currentFrame];

		SoundManager.PlaySound("snd_rotate");

		// Check if we didn't overlap with any tile or wall and try to move away from it. If it fails, revert the rotate

		bool clearToGo = true;

		UpdateTilePos();

		for(int i = 0; i < moves.Length; ++i)
		{
			clearToGo = true;

			foreach(Tile tile in tiles)
				clearToGo &= tile.CanMove(moves[i]);

			if(clearToGo)
			{
				rotationOverlapFixOffset = moves[i];
				mytransform.position += rotationOverlapFixOffset;
				return;	
			}
		}

		// OK, we're out of options, revert the rotate
		rotationOverlapFixOffset = prevOffset;
		mytransform.position += prevOffset;

		mytransform.Rotate (0.0f, 0.0f, 90.0f);

		mytransform.position -= rotationFrames[currentFrame];
		currentFrame = (currentFrame - 1 + rotationFrames.Length) % rotationFrames.Length;
		mytransform.position += rotationFrames[currentFrame];

		UpdateTilePos();

		return;
	}

	private void HandleGhost()
	{
		int minimumFallDistance = 10000;

		foreach(Tile tile in tiles)
			minimumFallDistance = Mathf.Min(minimumFallDistance, tile.FindFallDistance());

		foreach(Tile tile in tiles)
			tile.DisplayGhost(minimumFallDistance);
	}

	public bool isRapid()
	{
		return this.rapidFall;
	}

	// Destroy the tetramino but leave the tiles and update the grid.
	void killMe()
	{	
		if(!isActive)
		{
			Destroy(this.gameObject);
			return;
		}
		if(!isDying || moveWindowTimer > 0.0f)
			return;

		isDying = false;
		moveWindowTimer = 0.0f;

		Vector3 move = new Vector3(0.0f, -1.0f, 0.0f);
		bool canMove = true;
		
		foreach(Tile tile in tiles)
			canMove &= tile.CanMove(move);

		if(canMove)
			return;

		foreach(Tile tile in tiles)
		{
			tile.PlayDetachFlash();
			tile.DeleteGhost();
		}

		bool gameOver = false;

		foreach(Tile tile in tiles)
		{
			if(tile.positionInGrid.y > Playground.gridDimensions.y)
			{
				gameOver = true;
			}
			Playground.addTile(tile.positionInGrid, tile);
			tile.familyNumber = myNumber;
		}

		if(gameOver)
		{
			Destroy (this.gameObject);
			Playground.GameOver();
			return;
		}
		
		mytransform.DetachChildren();
		SoundManager.PlaySound ("snd_fallDown");
		Destroy(this.gameObject);

		return;
	}

	public void UpdateTilePos()
	{
		foreach(Tile tile in tiles)
			tile.UpdatePos();
	}
}
