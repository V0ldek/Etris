using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverScreen : MonoBehaviour 
{
	public Text endGameMessage;

	public Text score;
	public Text linesCleared;
	public Text level;

	public Text highscore;
	public Text highlinesCleared;
	public Text highlevel;

	PlayerStats statsScript;

	void Awake()
	{
		GameObject playerStats = GameObject.Find("GLOBAL_playerStats");
		statsScript = playerStats.GetComponent<PlayerStats>();

		if(statsScript.level == 16)
			endGameMessage.text = "YOU WON!";
		else
			endGameMessage.text = "GAME OVER";	

		score.text = (10 * statsScript.score).ToString();
		linesCleared.text = statsScript.linesCleared.ToString();
		level.text = statsScript.level.ToString();

		if(statsScript.score > statsScript.highscore)
		{
			statsScript.highscore = statsScript.score;
			highscore.color = new Color(0.0f, 255.0f, 0.0f);
		}
		if(statsScript.level > statsScript.highlevel)
		{
			statsScript.highlevel = statsScript.level;
			highlevel.color = new Color(0.0f, 255.0f, 0.0f);
		}
		if(statsScript.linesCleared > statsScript.highlinesCleared)
		{
			statsScript.highlinesCleared = statsScript.linesCleared;
			highlinesCleared.color = new Color(0.0f, 255.0f, 0.0f);
		}

		highscore.text = (10 * statsScript.highscore).ToString();
		highlinesCleared.text = statsScript.highlinesCleared.ToString();
		highlevel.text = statsScript.highlevel.ToString();

		Destroy (playerStats.gameObject);
	}

	void Start () 
	{
		
	}

	void Update () 
	{
	
	}

	public void InintializeGameOver()
	{
		
	}

	public void Restart()
	{
		Playground.gameOver = false;
		statsScript.SaveHighscore();
		SoundManager.Restart ();
		Application.LoadLevel("tetris");
		return;
	}

	public void CloseGame()
	{
		statsScript.SaveHighscore();
		Application.Quit();
	}
}
