using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetraminoSpawner : MonoBehaviour
{
	// Array of all tetramino types
	public Tetramino[] tetraminoTypes;

	// Number of current tetramino
	private int currentNumber;

	// The tetramino that will spawn next (displayed in the upper right corner
	private Tetramino nextTetramino;

	private int prevType;

	void Awake()
	{
		currentNumber = 1;
		prevType = -1;
	}

	void Start()
	{
		currentNumber = 1;
	}

	// Take the next tetramino and make it the current one
	public Tetramino SpawnTetramino()
	{	
		if(currentNumber == 0)
			++currentNumber;
		if(nextTetramino == null)
			GenerateNextTetramino();

		// Set the already generated next tetramino as the current one
		Tetramino currentTetramino = nextTetramino;
		
		// We just used it so it no longer is a next tetramino
		nextTetramino = null;
		
		// Set the position of the tetramino
		Vector3 offset = new Vector3(0, currentTetramino.dimensions.y * 0.5f, 0);
		
		if(currentTetramino.dimensions.x % 2 != 0)
			offset.x += -0.5f;
		
		currentTetramino.mytransform.position = new Vector3(0.0f, 20.5f, 0.0f) + offset;
		currentTetramino.UpdateTilePos();
		
		// Activate the tetramino
		currentTetramino.isActive = true;

		// Generate next tetramino since it's null now
		GenerateNextTetramino();
		
		return currentTetramino;
	}

	// Create a random tetramino and display it in the upper-right corner
	private void GenerateNextTetramino()
	{
		// Randomize the type of the tetramino
		int type = Random.Range (0, tetraminoTypes.Length);

		if(type == prevType)
		{
			prevType = -1;
			type = Random.Range (0, tetraminoTypes.Length);
		}

		prevType = type;

		// Instantiate the tetramino on the position
		nextTetramino = MonoBehaviour.Instantiate(tetraminoTypes[type], new Vector3(10.5f, 16f, 0), Quaternion.identity) as Tetramino;
		
		// Make sure it's static and not active
		nextTetramino.isActive = false;
		
		// Set its family number and increment the global one
		nextTetramino.myNumber = currentNumber++;
		
		return;
	}

	public void Restart()
	{
		currentNumber = 1;
		prevType = -1;
	}
}
