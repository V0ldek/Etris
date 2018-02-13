using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	public AudioClip[] audioClips;
	public string[] audioNames;
	public float[] audioVolume;
	private static Dictionary<string, AudioClip> audioDictionary;
	private static Dictionary<string, float> volumeDictionary;
	private static List<GameObject> musicObjects;

	// Use this for initialization
	void Awake () 
	{
		GameObject.DontDestroyOnLoad(this.gameObject);
		musicObjects = new List<GameObject>();
		audioDictionary = new Dictionary<string, AudioClip>();
		volumeDictionary = new Dictionary<string, float>();

		for(int i = 0; i < audioClips.Length; ++i)
		{
			audioDictionary.Add (audioNames[i], audioClips[i]);
			volumeDictionary.Add (audioNames[i], audioVolume[i]);
		}

		return;
	}

	public static void PlaySound(string name)
	{
		if(!audioDictionary.ContainsKey(name))
			return;

		AudioSource.PlayClipAtPoint(audioDictionary[name], Vector3.zero, volumeDictionary[name]);
	}

	public static void PlayMusic(string name)
	{
		if(!audioDictionary.ContainsKey(name))
			return;

		GameObject musicObject = new GameObject();
		musicObject.AddComponent<AudioSource>();
		GameObject.DontDestroyOnLoad(musicObject);
		AudioSource audioSource = musicObject.GetComponent<AudioSource>();
		audioSource.clip = audioDictionary[name];
		audioSource.volume = volumeDictionary[name];
		audioSource.Play();
		musicObjects.Add (musicObject);
	}

	public static void Restart()
	{
		foreach(GameObject obj in musicObjects)
		{
			obj.GetComponent<AudioSource>().Stop();
			Destroy (obj);
		}
		musicObjects.Clear();
	}
}
