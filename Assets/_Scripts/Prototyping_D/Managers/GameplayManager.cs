﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
	public float letterPause = 0.05f;
	public float levelStartDelay = 3f;
	public float turnDelay = .3f;
	public static GameplayManager instance = null;
	public static bool LevelLoadCalled = false;
	public BoardCycleManager boardScript;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;
	public bool generateNextBoard = false;
	public bool changeBoard = false;
	public string startingLevelMessage;

	private Text levelText;
	private Text dialogText;
	private string nextdialogText;
	private GameObject levelImage;
	GameObject dialogObject;
	public bool doingSetup = true;
	private int level = 1;
	private List<EnemyController> enemies;
	private List<EnemyController> enemiesNext;
	private bool enemiesMoving;
	private bool firstDialog = false;

	public int Level {
		get { return level; }
		set { level = value; }
	}
	// Use this for initialization
	void Awake ()
	{
		doingSetup = true;
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
		LevelLoadCalled = false;
		enemies = new List<EnemyController> ();
		enemiesNext = new List<EnemyController> ();
		boardScript = GetComponent<BoardCycleManager> ();
		dialogObject = GameObject.Find ("NewDialog");
		InitGame ();
	}

	private void OnLevelWasLoaded (int index)
	{
		//##Comment this if you have problems with level incrementation while testing##
		//It is to prevent second loading when changing two different scenes, 
		//it bugs out while testing same scene
		if (!LevelLoadCalled) {
			print ("LevelLoaded called");
			LevelLoadCalled = true;//flip the flag
			return;
		}
		//-----------------------------------------

		level++;
		InitGame ();
	}

	void InitGame ()
	{
		doingSetup = true;
		levelImage = GameObject.Find ("LevelImage");
		levelText = levelImage.GetComponentInChildren<Text> ();
		dialogText = GameObject.Find ("DialogText").GetComponent<Text> ();
		levelText.text = "";
		startingLevelMessage = "Where am i? Mom? Where are you...";
		dialogText.text = "Press F to turn on the flashlight. The more you use it. The more power is drained.\n Press P for more tips.";
		levelImage.SetActive (true);
		StartCoroutine (TypeText ());

		enemies.Clear ();
		boardScript.SetupScene (level, generateNextBoard);
		level += 1;
		generateNextBoard = true;
		boardScript.SetupScene (level, generateNextBoard);
	}

	IEnumerator TypeText ()
	{
		foreach (char letter in startingLevelMessage.ToCharArray()) {
			levelText.text += letter;
			//if (typeSound1 && typeSound2)
			//SoundManager.instance.RandomizeSfx (typeSound1, typeSound2);
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
		Invoke ("HideLevelImage", levelStartDelay);
	}

	private void HideLevelImage ()
	{
		levelImage.SetActive (false);
		dialogObject.GetComponent<Animator> ().SetTrigger ("StartDialog");
		doingSetup = true;
		firstDialog = true;
	}

	public void GameOver ()
	{
		levelText.text = "After " + level + " days, you starved.";
		levelImage.SetActive (true);
		enabled = false;
	}

	IEnumerator MoveEnemies ()
	{
		
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++) {
			enemies [i].MoveEnemy ();
			yield return new WaitForSeconds (enemies [i].moveTime);
		}
	}
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		} else if (Input.GetKeyDown (KeyCode.P) && firstDialog) {
			if (doingSetup) {
				doingSetup = false;
				int tips = Random.Range (0, 5);
				if (tips == 0) {
					nextdialogText = "Press F to turn on the flashlight. The more you use it. The more power is drained.";
				} else if (tips == 1) {
					nextdialogText = "Press Esc to exit the game.";
				} else if (tips == 2) {
					nextdialogText = "No more tips, silly!";
				} else if (tips == 3) {
					nextdialogText = "NPCs don't exist yet. Sorry.";
				} else if (tips == 4) {
					nextdialogText = "Level templates are in construction. Puzzles, enemies and even more fun...";
				}
				nextdialogText += " \nPress P for more tips."; 
			} else if (!doingSetup) {
				doingSetup = true;
			}
			dialogText.text = doingSetup ? nextdialogText : dialogText.text;
			dialogObject.GetComponent<Animator> ().SetTrigger ("StartDialog");
		}
		if (doingSetup) {
			return;
		}
		//StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList (EnemyController script)
	{
		enemies.Add (script);
	}

	public void AddEnemyToNextList (EnemyController script)
	{
		enemiesNext.Add (script);
	}

	public void SwitchEnemyLists ()
	{
		enemies.Clear ();
		enemies = enemiesNext;
		enemiesNext = new List<EnemyController> ();
	}

	public void DoingSetup (bool value)
	{
		doingSetup = value;
	}
}
