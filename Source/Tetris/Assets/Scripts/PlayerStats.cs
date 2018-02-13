using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
	public int score;
	public int level;
	public int linesCleared;
	public int highestCombo;
	public int highscore;
	public int highlevel;
	public int highlinesCleared;
	private string directory;

	private char[] XOR = new char[9] { 'H', 'P', '[', '#', '%', ',', '?', ' ', 'Z' };

	void Awake()
	{
		UnityEngine.Random.seed = (int)DateTime.UtcNow.ToBinary();
		directory = Directory.GetCurrentDirectory();
		GameObject.DontDestroyOnLoad(this.gameObject);
		Restart();
	}

	public void Restart()
	{
		score = 0;
		level = 0;
		linesCleared = 0;
		highestCombo = 0;
		highscore = 0;
		highlevel = 0;
		highlinesCleared = 0;
	}

	public void LoadHighscore()
	{
		int xorIndex = 0;

		if(!File.Exists(directory + "\\save\\highscore.xde"))
		{
			Debug.Log ("File does not exist");
			return;
		}
		StreamReader file = new StreamReader(directory + "\\save\\highscore.xde");

		for(int i = 0; i < 1639; ++i)
		{
			file.Read();
		}
		xorIndex = 0;
		string temp = file.ReadLine();
		string xored = "";
		StringBuilder sb = new StringBuilder();
		
		for(int i = 0; i < temp.Length; ++i)
		{
			sb.Append ((char) (((int)temp[i] ^ (int)XOR[xorIndex]) - 5000));
			++xorIndex;
			xorIndex %= XOR.Length;
		}
		
		xored = sb.ToString();
		
		Debug.Log (xored);

		highscore = Int32.Parse(xored);

		for(int i = 0; i < 2348; ++i)
		{
			file.Read();
		}
		xorIndex = 0;
		temp = file.ReadLine();
		xored = "";

		sb = new StringBuilder();
		
		for(int i = 0; i < temp.Length; ++i)
		{
			sb.Append ((char) (((int)temp[i] ^ (int)XOR[xorIndex]) - 5000));
			++xorIndex;
			xorIndex %= XOR.Length;
		}
		
		xored = sb.ToString();
		
		Debug.Log (xored);

		highlevel = Int32.Parse(xored);

		for(int i = 0; i < 3331; ++i)
		{
			file.Read();
		}
		xorIndex = 0;
		temp = file.ReadLine();
		xored = "";
		
		sb = new StringBuilder();
		
		for(int i = 0; i < temp.Length; ++i)
		{
			sb.Append ((char) (((int)temp[i] ^ (int)XOR[xorIndex]) - 5000));
			++xorIndex;
			xorIndex %= XOR.Length;
		}
		
		xored = sb.ToString();

		Debug.Log (xored);

		highlinesCleared = Int32.Parse(xored);

		for(int i = 0; i < 1234; ++i)
		{
			file.Read();
		}

		file.Close();
	}

	public void SaveHighscore()
	{
		if(File.Exists(directory + "\\save\\highscore.xde"))
		{
			Debug.Log ("File did exist, so it was brutally murdered");
			File.Delete(directory + "\\save\\highscore.xde");
		}

		StreamWriter file = new StreamWriter(directory + "\\save\\highscore.xde");

		int randNum;
		char randChar;
		int xorIndex = 0;

		for(int i = 0; i < 1639; ++i)
		{
			randNum = UnityEngine.Random.Range(0, 10000);
			randChar = (char)(randNum ^ (int)XOR[xorIndex]);
			++xorIndex;
			xorIndex %= XOR.Length;
			file.Write (randChar);
		}

		xorIndex = 0;
		string temp = highscore.ToString ();
		string xored = "";
		StringBuilder sb = new StringBuilder();

		for(int i = 0; i < temp.Length; ++i)
		{
			sb.Append ((char) ((int)(temp[i] + 5000) ^ (int)XOR[xorIndex]));
			++xorIndex;
			xorIndex %= XOR.Length;
		}

		xored = sb.ToString();

		Debug.Log (xored);

		file.WriteLine(xored);

		for(int i = 0; i < 2348; ++i)
		{
			randNum = UnityEngine.Random.Range(0, 10000);
			randChar = (char)(randNum ^ (int)XOR[xorIndex]);
			++xorIndex;
			xorIndex %= XOR.Length;
			file.Write (randChar);
		}

		xorIndex = 0;
		temp = highlevel.ToString();
		xored = "";
		sb = new StringBuilder();
		
		for(int i = 0; i < temp.Length; ++i)
		{
			sb.Append ((char) ((int)(temp[i] + 5000) ^ (int)XOR[xorIndex]));
			++xorIndex;
			xorIndex %= XOR.Length;
		}
		
		xored = sb.ToString();

		file.WriteLine(xored);

		for(int i = 0; i < 3331; ++i)
		{
			randNum = UnityEngine.Random.Range(0, 10000);
			randChar = (char)(randNum ^ (int)XOR[xorIndex]);
			++xorIndex;
			xorIndex %= XOR.Length;
			file.Write (randChar);
		}
		xorIndex = 0;

		temp = highlinesCleared.ToString ();
		xored = "";

		sb = new StringBuilder();
		
		for(int i = 0; i < temp.Length; ++i)
		{
			sb.Append ((char) ((int)(temp[i] + 5000) ^ (int)XOR[xorIndex]));
			++xorIndex;
			xorIndex %= XOR.Length;
		}

		xorIndex = 0;
		xored = sb.ToString();

		file.WriteLine(xored);

		for(int i = 0; i < 1234; ++i)
		{
			randNum = UnityEngine.Random.Range(0, 10000);
			randChar = (char)(randNum ^ (int)XOR[xorIndex]);
			++xorIndex;
			xorIndex %= XOR.Length;
			file.Write (randChar);
		}

		file.Close();
	}
}
