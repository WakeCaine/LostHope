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
		if (soundManagerMenu.musicEnded && levelToLoad != "") {
			SceneManager.LoadScene (1);
		}
		/*if(Application.loadedLevel == 1)
		{
			if (rightBound.enemyScore >= 7 && !isFinished)
			{
				isFinished = true;
				enemyWon = true;
				playerWon = false;
			}
			else if (leftBound.playerScore >= 7 && !isFinished)
			{
				isFinished = true;
				enemyWon = false;
				playerWon = true;
			}

			if (isFinished)
			{
				showFinished();
			}
		}

		if (Input.GetKeyDown(KeyCode.P) && !isFinished)
		{
			pauseControl();
		}

		if(Time.timeScale == 0)
		{
			foreach(GameObject g in pauseObjects)
			{
				if(g.name == "PauseText")
				{
					g.SetActive(true);
				}
			}
		}
		else
		{
			foreach (GameObject g in pauseObjects)
			{
				if(g.name == "PauseText")
				{
					g.SetActive(false);
				}
			}
		}*/
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
		/*foreach(GameObject g in pauseObjects)
		{
			g.SetActive(true);
		}*/
	}

	public void hidePaused ()
	{
		/*foreach(GameObject g in pauseObjects)
		{
			g.SetActive(false);
		}*/
	}

	public void showFinished ()
	{
		/*foreach (GameObject g in pauseObjects) {
			g.SetActive (true);
		}

		foreach (GameObject g in finishObjects) {
			g.SetActive (true);
		}*/
	}

	public void hideFinished ()
	{
		/*foreach (GameObject g in finishObjects) {
			g.SetActive (false);
		}*/
	}

	public void LoadLevel (string level)
	{
		levelToLoad = level;
		fader.SetActive (true);
		anim.SetTrigger ("GameStart");	
		soundManagerMenu.SilenceMusic ();
	}
}
