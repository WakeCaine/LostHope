using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

	GameObject[] pauseObjects, finishObjects;
	public SoundManagerMenu soundManagerMenu;

	private string levelToLoad = "";

	Animator anim;

	GameObject canvas;
	GameObject fader;

	//public bool isFinished;
	//public bool playerWon, enemyWon;
	// Use this for initialization
	void Start ()
	{
		canvas = GameObject.Find ("Canvas");
		fader = canvas.transform.Find ("ScreenFader").gameObject;
		anim = canvas.GetComponent<Animator> ();
		//pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
		//finishObjects = GameObject.FindGameObjectsWithTag("ShowOnFinish");
		//hideFinished();
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.Return)) {
			LoadLevel ("Does not matter");
		}

		if (soundManagerMenu.musicEnded && levelToLoad != "") {
			SceneManager.LoadScene (1);
		}
	}

	public void pauseControl ()
	{
		if (Time.timeScale == 1) {
			Time.timeScale = 0;
			showPaused ();
		} else if (Time.timeScale == 0) {
			Time.timeScale = 1;
			hidePaused ();
		}
	}

	public void showPaused ()
	{
		
	}

	public void hidePaused ()
	{
		
	}

	public void showFinished ()
	{
		
	}

	public void hideFinished ()
	{
		
	}

	public void LoadLevel (string level)
	{
		levelToLoad = level;
		fader.SetActive (true);
		anim.SetTrigger ("GameStart");	
		soundManagerMenu.SilenceMusic ();
	}

}
