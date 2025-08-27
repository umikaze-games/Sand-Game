using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[Serializable]
public class AudioEntry
{
	public string name;
	public AudioClip clip;
}
public class SEManager : MonoBehaviour
{
	public static SEManager instance;
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private List<AudioEntry> audioEntries;
	[SerializeField] private Dictionary<string,AudioClip> audioClips;

	private void Awake()
	{
		if (instance==null)
		{
			instance = this;
		}
		else Destroy(gameObject);

		audioClips = new Dictionary<string,AudioClip>();

		foreach (AudioEntry audioEntry in audioEntries)
		{
			audioClips[audioEntry.name] = audioEntry.clip;
		}
	}
	private void OnEnable()
	{
		SandSimulation.clearEvent += PlaySE;
		SandSimulation.dropEvent += PlaySE;
	}

	private void OnDisable()
	{
		SandSimulation.clearEvent -= PlaySE;
		SandSimulation.dropEvent -= PlaySE;
	}
	public void PlaySE(string name)
	{
		audioClips.TryGetValue(name, out AudioClip clip);
		if (clip)
		{
			audioSource.clip = clip;

			audioSource.Play();
		}
	}
}
