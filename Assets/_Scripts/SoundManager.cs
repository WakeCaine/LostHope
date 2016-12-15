using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
	public AudioSource efxSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	private int randomizeTimePlaying = 0;

	private int counterSeconds = 0;

	// Use this for initialization
	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

		randomizeTimePlaying = Random.Range (20, 40);
	}

	void Start ()
	{
		InvokeRepeating ("PlayAmbient", 0, 1);
	}

	void Update ()
	{
		if (efxSource != null) {
			if (efxSource.time > 0.5f) {
				efxSource.Stop ();
			}
		}
	}

	void PlayAmbient ()
	{
		if (!musicSource.isPlaying) {
			counterSeconds += 1;
			if (counterSeconds == randomizeTimePlaying) {
				musicSource.Play ();
				randomizeTimePlaying = Random.Range (20, 40);
				counterSeconds = 0;
			}
		}
	}

	public void PlaySingle (AudioClip clip)
	{
		efxSource.clip = clip;
		efxSource.Play ();
	}

	public void RandomizeSfx (float volume, float pitch, bool footsteps, bool random, params AudioClip[] clips)
	{
		if (!efxSource.isPlaying) {
			int randomIndex = Random.Range (0, clips.Length);
			float randomPitch = Random.Range (lowPitchRange, highPitchRange);

			efxSource.pitch = pitch;
			efxSource.volume = volume;
			efxSource.clip = clips [randomIndex];
			efxSource.Play ();
		}

		if (!footsteps) {
			int randomIndex = Random.Range (0, clips.Length);
			float randomPitch = Random.Range (lowPitchRange, highPitchRange);
			efxSource.pitch = random ? randomPitch : pitch;
			efxSource.volume = volume;
			efxSource.clip = clips [randomIndex];
			efxSource.Play ();
		}
	}
}
