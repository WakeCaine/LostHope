using UnityEngine;
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
	private GameObject player;
	private GameObject levelImage;
	GameObject dialogObject;
	public bool doingSetup = true;
	private int level = 1;
	private List<EnemyController> enemies;
	private List<EnemyController> enemiesNext;
	private bool enemiesMoving;

	Animator anim;
	public bool batterylow;
	private Image batteryLevel;
	public SpriteRenderer spr;
	public float flashPowerLevel;
	public Sprite bate1;
	public Sprite bate2;
	public Sprite bate3;
	public Sprite bate5;
	public Sprite bate4;


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

	
		GameObject battery = GameObject.Find ("batteryLevel");
		batteryLevel = battery.GetComponent<Image> ();
		anim = battery.GetComponent<Animator>();


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
		levelText.text = "";
		startingLevelMessage = "Where am I? Mom? Where are you...";
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
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		} else if (Input.GetKeyDown (KeyCode.P)) {
			dialogObject.GetComponent<Animator> ().SetTrigger ("StartDialog");
		}
		if (doingSetup) {
			return;
		}
		StartCoroutine (MoveEnemies ());



		float flashPowerLevel = GameObject.Find("Hero").GetComponent<HeroPlayerController> ().flashPowerLevel;
		// Update flashlight range, light collider size and light collider position
		if (flashPowerLevel > 75) {

			anim.enabled = false;
			batteryLevel.sprite = bate1;
		
		} else if (flashPowerLevel <= 75 && flashPowerLevel > 50) {

			anim.enabled = false;
			batteryLevel.sprite = bate2;

		} else if (flashPowerLevel <= 50 && flashPowerLevel > 25) {

			anim.enabled = false;
			batteryLevel.sprite = bate5;

		} else if (flashPowerLevel <= 25 && flashPowerLevel > 0) {

			anim.enabled = true;
			anim.SetBool ("batterylow", true);
	
		} else {
		
			anim.enabled = false;
			batteryLevel.sprite = bate4;
		}
			

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
