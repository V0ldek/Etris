using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
	public FlashAnim detachAnim;
	public FlashAnim destroyAnim;

	// Cache for transorm
	public Transform mytransform;

	// Number of the tetramino the tile was created by
	public int familyNumber;

	// Ghost tile
	public Tile ghostPrefab;
	private Tile ghostObject;

	public bool isAGhost;

	public float distToAnimFall;
	public float distFallen;

	public Vector2 positionInGrid;

	bool animate;

	// Used for DFS in GridUpdateManager.Cascade() POST: appears that it is no longer used anywhere
	//private bool visited;

	// Set default values and cache the Transform
	void Awake() 
	{		
		mytransform = this.transform;

		if(isAGhost)
			return;

		ghostObject = Instantiate(ghostPrefab, mytransform.position, Quaternion.identity) as Tile;
		distToAnimFall = 0;
		familyNumber = -1;
		positionInGrid = Vector2.zero;
		destroyAnim.colour = new Vector3(0.0f, 255.0f, 255.0f);
		detachAnim.colour = new Vector3(255.0f, 255.0f, 255.0f);

		return;
	}

	void Start()
	{
		UpdatePos();
		animate = false;
	}
	
	void Update()
	{
		if(isAGhost)
		{
			return;
		}
		if(familyNumber == -1)
		{
			UpdatePos();
			return;
		}
		if(this.mytransform.parent != null && distToAnimFall == 0)
			UpdatePos();

		if((!Playground.haltAnim && distToAnimFall > 0))
		{
			float sub = Mathf.Min (20 * Time.deltaTime, distToAnimFall);
			distToAnimFall -= sub;
			if(distToAnimFall < 0)
				distToAnimFall = 0.0f;
			this.mytransform.position -= new Vector3(0, sub, 0);
		}

		if(!Playground.haltAnim && animate && distToAnimFall == 0.0f)
		{
			--Playground.movingTiles;
			animate = false;
		}

		return;
	}

	// Translate global position of our transform to grid coordinates
	private Vector2 GetGridPosition()
	{
		if(isAGhost)
			return Vector2.zero;
		// How the fuck did you even call this if I don't exist?!
		if(this == null)
			Debug.Log ("Tile is null ??? - at: Tile.GetGridPosition()"); // POST: Debug.Error rather

		// Translate the position and fix any float errors
		Vector2 pos = new Vector2(mytransform.position.x, mytransform.position.y) + new Vector2(5.5f, 0.0f);

		if(Mathf.Ceil(pos.x) - pos.x <= 0.001f)
			pos.x = Mathf.Ceil(pos.x);
		else if(pos.x - Mathf.Floor(pos.x) <= 0.001f)
			pos.x = Mathf.Floor(pos.x);

		if(Mathf.Ceil(pos.y) - pos.y <= 0.001f)
			pos.y = Mathf.Ceil(pos.y);
		else if(pos.y - Mathf.Floor(pos.y) <= 0.001f)
			pos.y = Mathf.Floor(pos.y);

		return pos;
	}

	// Check if the tile can be moved by given vector
	public bool CanMove(Vector3 move)
	{
		if(isAGhost)
			return false;
		// First get our current position and find out where we'll end up after the move
		if(move.y > 0)
			Debug.LogError("Trying to move upwards"); // POST: ambiguous
		Vector2 pos = positionInGrid;
		pos.x += move.x;
		pos.y += move.y;
		// If we went off the grid's bounds we can't go there
		if(pos.x <= 0 || pos.x >= Playground.gridDimensions.x + 1 || pos.y <= 0 || pos.y >= Playground.gridDimensions.y + 5)
			return false;

		// Check if the tile is occupied, if it is we can't move there, if it isn't we can

		return Playground.isOccupied(pos) == 0;
	}

	// Move the tile by the given vector and update the playground to reflect that
	public void MoveMe(Vector3 move)
	{
		if(isAGhost)
			return;
		// Get our position
		Vector2 pos = positionInGrid;

		if(move.y > 0)
			Debug.LogError("Trying to move upwards"); // POST: ambiguous

		// Clear grid in that position
		Playground.deleteTile(pos);

		// Change the position to where we'll end up
		pos.x += move.x;
		pos.y += move.y;

		// Fill the grid on our new position
		Playground.addTile(pos, this);

		// Update the transform

		positionInGrid.x += move.x;
		positionInGrid.y += move.y;
		distToAnimFall -= move.y;
		++Playground.movingTiles;
		animate = true;
	}

	public void DisplayGhost(int tilesDown)
	{
		if(isAGhost || ghostObject == null)
			return;
		ghostObject.mytransform.position = this.mytransform.position;
		ghostObject.mytransform.position -= new Vector3(0.0f, tilesDown, 0.0f);
	}

	// Check how many tiles underneath us are available for moving
	public int FindFallDistance()
	{
		if(isAGhost)
			return 0;
		Vector2 dist = new Vector2(0.0f, 1.0f);
		Vector2 pos = this.positionInGrid;

		while(Playground.isOccupied(pos - dist) == 0)
			dist.y += 1.0f;

		return (int)(dist.y - 1.0f);
	}

	public void DeleteGhost()
	{
		if(ghostObject != null && ghostObject.gameObject != null)
		{
			Destroy(ghostObject.gameObject);
			ghostObject = null;	
		}
	}

	public void UpdatePos()
	{
		positionInGrid = GetGridPosition();
	}

	public void PlayDetachFlash()
	{
		Instantiate(detachAnim, this.mytransform.position, Quaternion.identity);
	}

	public void killMe()
	{
		FlashAnim anim = Instantiate(destroyAnim, this.mytransform.position, Quaternion.identity) as FlashAnim;
		anim.haltGame = true;
		Playground.haltAnim = true;
		destroyAnim.windUpTime = 0.1f;
		destroyAnim.dissolveTime = 0.2f;
		Destroy (this.gameObject);
	}
}
