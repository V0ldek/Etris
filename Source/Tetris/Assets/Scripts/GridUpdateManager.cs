using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridUpdateManager {

	public int[,] group;
	public List<Tile>[] bottomTiles;
	public int[,] fallDistance;
	public int[] groupFallDistance;
	public List<int> groupOrder;
	public List<Tile>[] tiles;
	public bool[] ordered;

	private int numberGroups = 0;

	public void InitializeGroupArray()
	{
		fallDistance = new int[(int)Playground.gridDimensions.x + 2, (int)Playground.gridDimensions.y + 5];
		group = new int[(int)Playground.gridDimensions.x + 2, (int)Playground.gridDimensions.y + 5];
		
		for(int x = 0; x < Playground.gridDimensions.x + 2; ++x)
			group[x, 0] = -2;	
		
		for(int y = 0; y < Playground.gridDimensions.y + 5; ++y)
			group[0, y] = group[(int)Playground.gridDimensions.x + 1, y] = -2;
	}

	public void UpdateGrid()
	{	
		if(Playground.haltGame || Playground.movingTiles != 0)
			UpdateGrid();

		AssignGroups();
		FindDownmostTiles();
		OrderGroups();
		Cascade();
	}

	Vector2[] moves;

	public void DFS(Tile tile, int groupNum)
	{
		Vector2 pos = tile.positionInGrid;

		if(group[(int)pos.x, (int)pos.y] != -1)
			return;

		group[(int)pos.x, (int)pos.y] = groupNum;

		moves = new Vector2[4] { new Vector2(1.0f, 0.0f),
			new Vector2(-1.0f, 0.0f),
			new Vector2(0.0f, 1.0f),
			new Vector2(0.0f, -1.0f) };

		Tile tile2;

		for(int i = 0; i < moves.Length; ++i)
		{
			tile2 = Playground.getTileAtPosition(pos + moves[i]);

			if(tile2 != null && (moves[i].x == 0.0f || tile2.familyNumber == tile.familyNumber))
			   DFS(tile2, groupNum);
		}

		/*Tile tile2 = Playground.getTileAtPosition(pos + new Vector2(0.0f, -1.0f));

		if(tile2 != null)
			DFS(tile2, groupNum);

		tile2 = Playground.getTileAtPosition(pos + new Vector2(0.0f, 1.0f));

		if(tile2 != null)
			DFS(tile2, groupNum);

		tile2 = Playground.getTileAtPosition(pos + new Vector2(-1.0f, 0.0f));

		if(tile2 != null && tile2.familyNumber == tile.familyNumber)
			DFS(tile2, groupNum);

		tile2 = Playground.getTileAtPosition(pos + new Vector2(1.0f, 0.0f));

		if(tile2 != null && tile2.familyNumber == tile.familyNumber)
			DFS(tile2, groupNum);*/

		return;
	}

	public void AssignGroups()
	{
		for(int y = 1; y < Playground.gridDimensions.y + 5; ++y)
			for(int x = 1; x < Playground.gridDimensions.x + 2; ++x)
				group[x, y] = -1;

		int groupNum = 1;

		for(int y = 1; y < Playground.gridDimensions.y + 5; ++y)
		{
			for(int x = 1; x < Playground.gridDimensions.x + 2; ++x)
			{
				if(group[x, y] == -1)
				{
					Tile tile = Playground.getTileAtPosition(new Vector2(x, y));

					if(tile != null)
						DFS(tile, groupNum++);
				}
			}
		}

		numberGroups = groupNum - 1;

		tiles = new List<Tile>[numberGroups + 1];

		for(int i = 0; i <= numberGroups; ++i)
			tiles[i] = new List<Tile>();

		for(int y = 1; y < Playground.gridDimensions.y + 5; ++y)
		{
			for(int x = 1; x < Playground.gridDimensions.x + 2; ++x)
			{
				if(group[x, y] > 0)
					tiles[group[x, y]].Add(Playground.getTileAtPosition(new Vector2(x, y)));
			}
		}
	}
	
	public List<int> CheckForClearedLines()
	{
		List<int> lines = new List<int>();

		for(int y = 1; y < Playground.gridDimensions.y + 5; ++y)
		{
			bool full = true;

			for(int x = 1; x < Playground.gridDimensions.x + 2; ++x)
			{
				if(Playground.isOccupied(new Vector2(x, y)) == 0)
				{
					full = false;
					break;
				}
			}

			if(full)
				lines.Add (y);
		}

		return lines;
	}

	public void FindDownmostTiles()
	{
		bottomTiles = new List<Tile>[numberGroups + 1];

		for(int i = 0; i <= numberGroups; ++i)
			bottomTiles[i] = new List<Tile>();

		for(int x = 1; x < Playground.gridDimensions.x + 2; ++x)
		{
			Vector2[] downmostTiles = new Vector2[numberGroups + 1];

			for(int y = 1; y < Playground.gridDimensions.y + 5; ++y)
			{
				if(group[x, y] > 0 && downmostTiles[group[x, y]] == new Vector2())
					downmostTiles[group[x, y]] = new Vector2(x, y);
			}
			for(int i = 0; i <= numberGroups; ++i)
				if(downmostTiles[i] != Vector2.zero)
					bottomTiles[i].Add (Playground.getTileAtPosition(downmostTiles[i]));
		}
	}

	public void OrderGroups()
	{
		groupOrder = new List<int>();
		ordered = new bool[numberGroups + 1];

		for(int y = 1; y < Playground.gridDimensions.y + 5; ++y)
		{      
			for(int x = 1; x < Playground.gridDimensions.x + 2; ++x)
			{
				if(group[x, y] > 0 && !ordered[group[x, y]])
				{
					ordered[group[x, y]] = true;
					groupOrder.Add(group[x, y]);
				}
			}
		}
	}

	public void Cascade()
	{

		groupFallDistance = new int[numberGroups + 1];

		foreach(int index in groupOrder)
		{
			groupFallDistance[index] = 1000000009;
			
			foreach(Tile tile in bottomTiles[index])
			{
				if(tile == null)
				{
					Debug.Log ("Tile null in bottomTiles ??? at GridUpdateManager.Cascade()");
					return;
				}
				Vector2 pos = tile.positionInGrid;
				fallDistance[(int)pos.x, (int)pos.y] = tile.FindFallDistance();
				groupFallDistance[index] = Mathf.Min(groupFallDistance[index], fallDistance[(int)pos.x, (int)pos.y]);
			}
			
			foreach(Tile tile in tiles[index])
				tile.MoveMe(new Vector3(0.0f, -groupFallDistance[index], 0.0f));
		}

		for(int y = 1; y < Playground.gridDimensions.y + 5; ++y)                  
			for(int x = 1; x < Playground.gridDimensions.x + 2; ++x)
				if(group[x, y] > 0)
					fallDistance[x, y] = groupFallDistance[group[x, y]];
	}
}
