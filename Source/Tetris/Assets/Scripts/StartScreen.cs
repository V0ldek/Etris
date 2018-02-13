using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour {

	public void StartGame()
	{
		Playground.gameOver = false;
		Application.LoadLevel("tetris");
		return;
	}

	public void CloseGame()
	{
		Application.Quit();
	}
}
