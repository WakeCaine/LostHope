using UnityEngine;
using System.Collections;

public class SoundManagerMenu : MonoBehaviour
{

	public AudioSource efxSource;
	public AudioSource musicSource;

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;
	// Use this for initialization
	public bool musicEnded = false;
	public float audio1Volume = 1.0f;
	public float audio2Volume = 0.0f;

	private bool silenceMusic = false;

	void Awake ()
	{
		audio1Volume = musicSource.volume;
	}

	public void PlaySingle (AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play ();
	}

	public void RandomizeSfx (params AudioClip[] clips)
	{
		int randomIndex = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);

		efxSource.pitch = randomPitch;
		efxSource.clip = clips [randomIndex];
		efxSource.Play ();
	}

	public void SilenceMusic ()
	{
		silenceMusic = true;
	}

	void fadeOut ()
	{
		if (audio1Volume >= 0.0f) {
			audio1Volume -= 0.1f * Time.deltaTime;
			musicSource.volume = audio1Volume;
		} else {
			musicEnded = true;
			silenceMusic = false;
		}
	}

	void Update ()
	{
		if (silenceMusic == true)
			fadeOut ();
	}
}
