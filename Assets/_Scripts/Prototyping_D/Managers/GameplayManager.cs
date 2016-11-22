using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
	public float levelStartDelay = 3f;
	public float turnDelay = .3f;
	public static GameplayManager instance = null;
	public static bool LevelLoadCalled = false;
	public BoardCycleManager boardScript;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;
	public bool generateNextBoard = false;
	public bool changeBoard = false;

	private Text levelText;
	private GameObject levelImage;
	public bool doingSetup = true;
	private int level = 1;
	private List<EnemyController> enemies;
	private List<EnemyController> enemiesNext;
	private bool enemiesMoving;

	public int Level {
		get { return level; }
		set { level = value; }
	}
	// Use this for initialization
	void Awake ()
	{
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
		levelText.text = "???????????????????????????";
		levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();
		boardScript.SetupScene (level, generateNextBoard);
		level += 1;
		generateNextBoard = true;
		boardScript.SetupScene (level, generateNextBoard);
	}

	private void HideLevelImage ()
	{
		levelImage.SetActive (false);
		doingSetup = false;
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
		if (doingSetup) {
			return;
		}
		StartCoroutine (MoveEnemies ());
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
