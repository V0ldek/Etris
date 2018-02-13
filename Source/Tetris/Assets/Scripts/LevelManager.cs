using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour 
{
	public int[] linesForLevelUp;
	public int currentLevel;
	private int currentLinesCleared;
	private int totalLinesCleared;
	
	public int TotalLinesCleared 
	{
		get { return totalLinesCleared; }
	}

	public Text linesCleared;
	public Text linesRemaining;
	public Text level;

	// Use this for initialization
	void Awake () 
	{
		currentLevel = 1;
		currentLinesCleared = 0;
	}

	public void Restart()
	{
		currentLevel = 1;
		currentLinesCleared = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(currentLinesCleared >= linesForLevelUp [currentLevel - 1]) {
			
			if(currentLevel == 16)
			{
				Playground.gameOver = true;
				return;
			}

			currentLinesCleared -= linesForLevelUp [currentLevel - 1];

			currentLevel += 1;

			UpdateText();
		}
	}

	public int GetLevel()
	{
		return currentLevel;
	}

	public void AddLines(int lines)
	{
		currentLinesCleared += lines;
		totalLinesCleared += lines;
		UpdateText();
	}

	public void UpdateText()
	{
		level.text = currentLevel.ToString();
		linesCleared.text = totalLinesCleared.ToString();
		linesRemaining.text = ((linesForLevelUp[currentLevel - 1]) - currentLinesCleared).ToString();
	}
}
