using UnityEngine;
using System.Collections;

public class FlashAnim : MonoBehaviour 
{
	public SpriteRenderer mySpriteRenderer;
	public float windUpTime;
	public float dissolveTime;
	private float currWindUp;
	private float currDissolve;
	public bool haltGame;
	public Vector3 colour;

	// Use this for initialization
	void Start () 
	{
		mySpriteRenderer = this.GetComponent<SpriteRenderer>();
		currWindUp = windUpTime;
		currDissolve = dissolveTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(haltGame)
		{
			Playground.haltGame = true;
			Playground.haltAnim = true;
		}
		if(currWindUp > 0)
		{
			currWindUp -= Time.deltaTime;

			if(currWindUp < 0)
				currWindUp = 0;

			mySpriteRenderer.color = new Color(colour.x, colour.y, colour.z, (1 * (windUpTime - currWindUp) / windUpTime));

			return;
		}
		else if(currDissolve > 0)
		{
			currDissolve -= Time.deltaTime;
			
			if(currDissolve < 0)
				currDissolve = 0;

			mySpriteRenderer.color = new Color(colour.x, colour.y, colour.z, (1 * (currDissolve) / dissolveTime));
			
			return;
		}
		else
		{
			if(haltGame)
			{
				Playground.haltGame = false;
				Playground.haltAnim = false;
			}
			Destroy(this.gameObject);
		}
	}
}
