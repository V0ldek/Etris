using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Playground : MonoBehaviour 
{
	/*
	 * TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: 
     *
     * - Nothing for now
	 * 
	 * TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: TODO: 
	 * 
	 * DONE:
	 * - Line clear sound
	 * - Create a small time window after the tetramino falls in which you can still move and rotate
	 * - Create a switch which disables you from toggling rapid fall for a brief moment after tetramino spawns (a REALLY brief moment)
	 * - Add animations and combo info
	 */

	// The tetramino that is currently in play
	private Tetramino currentTetramino;

	// Dimensions of the grid
	public static Vector2 gridDimensions;

	// Grid for all tiles, so we can delete them all when the game is over and delete individual tiles
	private static Tile[,] tiles;

	// Instance of the GridUpdateManager which handles all algorithms for updating the grid after a line is cleared
	private GridUpdateManager gridUpdateManager;

	// Instance of the InputHandler which handles all the input
	private InputHandler inputHandler;

	[SerializeField]
	// Instance of TetraminoSpawner which handles spawning tetraminos
	private TetraminoSpawner tetraminoSpawner;

	[SerializeField]
	// Instance of SoundManager responsible for playing sounds
	public static SoundManager soundManager;

	[SerializeField]
	public ScoreManager scoreManager;

	[SerializeField]
	public LevelManager levelManager;

	public PlayerStats statsScript;

	// Used for cascade effect: any time a line is cleared this goes true which triggers grid update
	private bool lineWasCleared;

	// Used for moving the Tetramino in the x axis
	public float defaultMoveCooldown;
	public float fastMoveCooldown;
	private float moveCooldown;

	// Timers for tetramino falling
	public float[] defaultTimeForLevels;

	public float defaultFallTime;
	public float rapidFallTime;
	public float rapidBlockTime;

	public float moveWindowDefault;
	private float moveWindow;

	private int currentLevel;

	public static bool bHaltGame;
	public static bool haltGame;
	public static bool haltAnim;
	public static int movingTiles;

	public static bool gameOver;

	// Initialization
	void Start() 
	{
		Restart();
		currentTetramino = null;
		// Set default values
		gridDimensions = new Vector2(10, 20);
		gridUpdateManager = new GridUpdateManager();
		gridUpdateManager.InitializeGroupArray();
		inputHandler = new InputHandler(defaultMoveCooldown, fastMoveCooldown);
		lineWasCleared = false;
		moveCooldown = 0.0f;
		defaultFallTime = defaultTimeForLevels[0];
		levelManager.currentLevel = 1;
		currentLevel = 1;
		haltGame = false;
		haltAnim = false;
		movingTiles = 0;
		moveWindow = moveWindowDefault;

		GameObject playerStats = GameObject.Find("GLOBAL_playerStats");
		statsScript = playerStats.GetComponent<PlayerStats>();
		/*GameObject oldPlayerStats = GameObject.Find ("GLOBAL_playerStats");
		if(oldPlayerStats != null)
			Destroy(oldPlayerStats.gameObject);*/

		// Initialize grids and start the game by spawning first Tetramino
		InitializeGrid();

		// If both current and next tetraminos exist no job needs to be done
		currentTetramino = tetraminoSpawner.SpawnTetramino();
		currentTetramino.DefaultFallTime = defaultFallTime;
		currentTetramino.RapidFallTime = rapidFallTime;
		currentTetramino.moveWindow = this.moveWindow;
		currentTetramino.isDying = false;
		currentTetramino.rapidBlockTimer = this.rapidBlockTime;
		
		return;
	}

	void Update() 
	{
		if(gameOver && !haltAnim)
		{
			statsScript.LoadHighscore();
			Application.LoadLevel("gameOver");
			Playground.haltGame = true;
			return;
		}
		else if(gameOver)
			return;
		if(haltGame)
		{
			return;
		}

		if(currentTetramino != null && currentTetramino.isRapid())
			scoreManager.timerActive = false;
		else
			scoreManager.timerActive = true;

		// If we have no tetramino in play it means it fell down recently
		if(currentTetramino == null || currentTetramino.Equals(null))
		{
			if(movingTiles != 0)
				return;
			// Check for all the lines that should be cleared
			List<int> lines = gridUpdateManager.CheckForClearedLines();

			levelManager.AddLines(lines.Count);

			// Clear'em all
			foreach(int line in lines)
			{
				ClearLine(line);
			}

			if(lines.Count != 0 || haltGame)
				return;
			
			// As long as any lines are cleared, "cascade" the tiles and check for cleared lines again
			while(lineWasCleared)
			{
				// "Cascade" the grid
				gridUpdateManager.UpdateGrid();

				// Check for cleared lines and clear them (if any were found)
				lineWasCleared = false;
				
				if(scoreManager.currentCombo != 0)
				{
					int lineSound = Mathf.Min (scoreManager.currentCombo, 5);
					SoundManager.PlaySound("snd_lineClear" + lineSound.ToString());
				}

				if(movingTiles != 0)
					return;

				lines = gridUpdateManager.CheckForClearedLines();
				
				foreach(int line in lines)
				{
					ClearLine(line);
				}
				if(lines.Count != 0 || haltGame)
					return;
			}

			scoreManager.EndCombo();

			// Respawn the tetramino

			defaultFallTime = defaultTimeForLevels[levelManager.GetLevel() - 1];
			currentTetramino = tetraminoSpawner.SpawnTetramino();
			currentTetramino.DefaultFallTime = defaultFallTime;
			currentTetramino.RapidFallTime = rapidFallTime;	
			currentTetramino.moveWindow = this.moveWindow;
			currentTetramino.isDying = false;
			currentTetramino.rapidBlockTimer = this.rapidBlockTime;

			scoreManager.ResetTimer();
			scoreManager.timerActive = true;		

			gridUpdateManager.UpdateGrid();
		}

		// Update our moveCooldown
		if(moveCooldown >= 0.0f)
			moveCooldown -= Time.deltaTime;

		// Handle any input, no other class handles any input whatsoever
		inputHandler.HandleInput(currentTetramino, ref moveCooldown);

		scoreManager.addScoreOverTime = !currentTetramino.isDying;

		if(levelManager.GetLevel() != currentLevel)
		{
			++currentLevel;
			scoreManager.NextLevel();
			moveWindow -= 0.019f;
		}

		statsScript.score = scoreManager.CurrentScore;
		statsScript.linesCleared = levelManager.TotalLinesCleared;
		statsScript.level = levelManager.currentLevel;

		return;
	}

	// Create the grid arrays and fill them with emptiness and walls
	private static void InitializeGrid()
	{
		// Create the array
		tiles = new Tile[(int)gridDimensions.x + 2, (int)gridDimensions.y + 5];

		// Fill them all with 0/null just to be sure
		for(int x = 0; x < gridDimensions.x + 2; ++x)
			for(int y = 0; y < gridDimensions.y + 5; ++y)
			{
				tiles[x, y] = null;
			}

		return;
	}

	// Return family number of the tile on given position (-1 for walls, -2 for out of bounds)
	public static int isOccupied(Vector2 pos)
	{
		if(pos.x <= 0 || pos.x >= gridDimensions.x + 2 || pos.y <= 0 || pos.y >= gridDimensions.y + 5) 
			return -2;

		if(tiles[(int)pos.x, (int)pos.y] == null)
			return 0;

		return tiles[(int)pos.x, (int)pos.y].familyNumber;
	}

	// Set the tiles grid on given position to the given tile if there is no tile already there
	public static void addTile(Vector2 pos, Tile tile)
	{
		if(tiles[(int)pos.x, (int)pos.y] == null)
			tiles[(int)pos.x, (int)pos.y] = tile;

		return;
	}

	// Set the tile grid to null on given position
	public static void deleteTile(Vector2 pos)
	{
		tiles[(int)pos.x, (int)pos.y] = null;

		return;
	}

	// Return the Tile object from the given position. Return null when out of bounds or on walls
	public static Tile getTileAtPosition(Vector2 pos)
	{
		if(pos.y <= 0 || pos.y >= gridDimensions.y + 5 || pos.x <= 0 || pos.x >= gridDimensions.x + 2)
			return null;
		return tiles[(int)pos.x, (int)pos.y];
	}

	// Destroy all the tiles and reset the grids
	public static void GameOver()
	{			
		SoundManager.PlayMusic("snd_gameOver");
		Debug.LogWarning("Game Over");

		foreach(Tile tile in tiles)
			if(tile != null && tile.gameObject != null)
			{
				tile.destroyAnim.colour = new Vector3(255.0f, 0.0f, 0.0f);
				tile.destroyAnim.windUpTime = 0.2f;
				tile.destroyAnim.dissolveTime = 2.0f;
				tile.killMe ();
			}

		gameOver = true;

		return;
	}

	// Destroy all the tiles on given line and set lineWasCleared to true
	private void ClearLine(int line)
	{
		if(haltGame)
		{
			ClearLine(line);
			return;
		}
		scoreManager.ComboLine();

		lineWasCleared = true;

		for(int x = 1; x < Playground.gridDimensions.x + 1; ++x)
		{
			tiles[x, line].killMe();
			tiles[x, line] = null;
		}

		return;
	}

	public bool isCurrentlyRapid()
	{
		if(currentTetramino == null)
			return false;
		else
			return currentTetramino.isRapid();
	}

	public void Restart()
	{
		GameObject playerStats = GameObject.Find("GLOBAL_playerStats");
		PlayerStats statsScript = playerStats.GetComponent<PlayerStats>();

		statsScript.Restart();
		levelManager.Restart();
		tetraminoSpawner.Restart();
		scoreManager.Restart();
		SoundManager.Restart();

		moveWindow = moveWindowDefault;
	}
}