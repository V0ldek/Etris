using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour 
{
	public int scorePerLine;
	public int scorePerLineIncrease;
	public int scoreTimeDecrease;
	public float decreaseInTimePerLvl;
	public float timeDecreaseFirstStepDefault;
	public float timeDecreaseNextStepsDefault;
	public float timeDecreaseFirstStep;
	public float timeDecreaseNextSteps;
	public float rapidFallMultiplier;
	private float timer;

	private int linesCleared;
	private int currentScore;
	public int currentCombo;
	private int currentComboScore;
	private float scoreStorage;

	public int LinesCleared 
	{
		get { return linesCleared; }
	}

	public int CurrentScore
	{
		get { return currentScore; }
	}

	public Text scoreText;
	public Text comboText;
	public Text comboStaticText;
	public float comboTime;
	private float comboTimer;

	public bool timerActive;
	public bool addScoreOverTime;

	// Use this for initialization
	void Start () 
	{
		timeDecreaseFirstStep = timeDecreaseFirstStepDefault;
		timeDecreaseNextSteps = timeDecreaseNextStepsDefault;
		currentScore = 0;
		currentCombo = 0;
		currentComboScore = 0;
		addScoreOverTime = true;
		ResetTimer();
	}

	public void Restart()
	{
		timeDecreaseFirstStep = timeDecreaseFirstStepDefault;
		timeDecreaseNextSteps = timeDecreaseNextStepsDefault;
		currentScore = 0;
		currentCombo = 0;
		currentComboScore = 0;
		addScoreOverTime = true;
		ResetTimer();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(comboTimer > 0.0f)
			comboTimer -= Time.deltaTime;

		if(timerActive && addScoreOverTime)
		{
			timer -= Time.deltaTime;

			if(timer <= 0.0f)
			{
				currentScore -= scoreTimeDecrease;
				timer = timeDecreaseNextSteps;
			}
			UpdateText();
		}
		else if(addScoreOverTime)
		{
			scoreStorage += 10 * Time.deltaTime;

			if(scoreStorage >= 1)
			{
				currentScore += (int)scoreStorage;
				scoreStorage -= 1;
			}

			UpdateText();
		}
		if(comboText.color.a != 0.0f && comboTimer <= 0.0f)
		{
			comboText.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			comboStaticText.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
		}
		
	}

	public void ResetTimer()
	{
		timer = timeDecreaseFirstStep;
	}

	public void ComboLine()
	{
		++currentCombo;
		currentComboScore += (2 * scorePerLine * (linesCleared++)) + scorePerLine;
		comboText.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		comboStaticText.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		comboText.text = "x" + currentCombo.ToString ();
		comboTimer = comboTime;

		return;
	}

	public void EndCombo()
	{
		currentScore += currentComboScore;
		UpdateText();
		currentComboScore = 0;
		linesCleared = 0;
		currentCombo = 0;
	}

	public void UpdateText()
	{
		scoreText.text = (10 * currentScore).ToString();
	}

	public void NextLevel()
	{
		timeDecreaseFirstStep -= decreaseInTimePerLvl;
		timeDecreaseNextSteps -= decreaseInTimePerLvl;
	}
}
