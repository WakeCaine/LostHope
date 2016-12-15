using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

	public bool doingSetup = true;

	private Text levelText;
	private Text dialogText;
	private string nextdialogText;
	private HeroPlayerController player;
	public GameObject levelImage;
	private GameObject dialogObject;
	private int level = 1;
	private List<EnemyController> enemies;
	private List<EnemyController> enemiesNext;
	private bool enemiesMoving;
	private bool firstDialog = false;
	private bool isPlayerDead = false;
	private bool enemyAppear = true;

	Animator anim;
	public bool batterylow;
	private Image batteryLevel;
	public SpriteRenderer spr;
	public float flashPowerLevel;
	public Sprite[] batterySprites;

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
		LevelLoadCalled = false;
		enemies = new List<EnemyController> ();
		enemiesNext = new List<EnemyController> ();
		boardScript = GetComponent<BoardCycleManager> ();

	
		GameObject battery = GameObject.Find ("Battery");
		batteryLevel = battery.GetComponent<Image> ();
		anim = battery.GetComponent<Animator> ();


		dialogObject = GameObject.Find ("NewDialog");
		InitGame ();
	}

	void FixedUpdate ()
	{
		if (isPlayerDead) {
			if (Input.anyKey || Input.anyKeyDown) {
				ReturnToMenu ();
			}
		}

		if (boardScript.levell == 2) {
			boardScript.levell += 3;
			dialogText.text = "It is pretty dark in here. I think there should be a flashlight somewhere.";
			dialogObject.GetComponent<Animator> ().SetTrigger ("StartDialog");
			doingSetup = true;
		}

		if (enemies.Count > 0 && enemyAppear) {
			enemyAppear = false;
			dialogText.text = "I feel there is something hiding in the shadows. QUICK, we need to find an EXIT!";
			dialogObject.GetComponent<Animator> ().SetTrigger ("StartDialog");
			doingSetup = true;
		}

		BatteryLevelChanger ();
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
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<HeroPlayerController> ();

		levelText.text = "";
		startingLevelMessage = "Where am i? Mom? Where are you...";
		dialogText.text = "Hey. It seems that you are lost. I will help you. Just follow me through this door.";

		levelImage.SetActive (true);
		StartCoroutine (TypeText ());

		//Create two boards
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

	IEnumerator TypeEndingText ()
	{
		foreach (char letter in startingLevelMessage.ToCharArray()) {
			levelText.text += letter;
			//if (typeSound1 && typeSound2)
			//SoundManager.instance.RandomizeSfx (typeSound1, typeSound2);
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
	}

	private void HideLevelImage ()
	{
		levelImage.SetActive (false);
		dialogObject.GetComponent<Animator> ().SetTrigger ("StartDialog");
		doingSetup = true;
		firstDialog = true;
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
				int tips = Random.Range (0, 7);
				if (tips == 0) {
					nextdialogText = "When you find the flashlight, press F to turn it on.";
				} else if (tips == 1) {
					nextdialogText = "Pressing Esc means escape and amnesia.";
				} else if (tips == 2) {
					nextdialogText = "No more tips, silly!";
				} else if (tips == 3) {
					nextdialogText = "A lot of people get lost in here. Maybe you will find one of them.";
				} else if (tips == 4) {
					nextdialogText = "It is not safe here, let's move on.";
				} else if (tips == 5) {
					nextdialogText = "Some objects are moveable, some don't.";
				} else if (tips == 6) {
					nextdialogText = "Sometimes there are places you can hide in. Press Space to hide.";
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

	void BatteryLevelChanger ()
	{
		float flashPowerLevel = player.flashPowerLevel;
		// Update flashlight range, light collider size and light collider position
		if (flashPowerLevel > 75) {
			anim.enabled = false;
			batteryLevel.sprite = batterySprites [0];

		} else if (flashPowerLevel <= 75 && flashPowerLevel > 50) {

			anim.enabled = false;
			batteryLevel.sprite = batterySprites [1];

		} else if (flashPowerLevel <= 50 && flashPowerLevel > 25) {

			anim.enabled = false;
			batteryLevel.sprite = batterySprites [2];

		} else if (flashPowerLevel <= 25 && flashPowerLevel > 0) {

			anim.enabled = true;
			anim.SetBool ("batterylow", true);

		} else {

			anim.enabled = false;
			batteryLevel.sprite = batterySprites [3];
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

	public void GameOverScreen ()
	{
		levelText.text = "";
		startingLevelMessage = "GAME OVER\n\n...mom...where are you...";
		levelImage.SetActive (true);
		GameObject.Find ("Canvas").GetComponent<Animator> ().SetTrigger ("GameOver");
		StartCoroutine (TypeEndingText ());
		isPlayerDead = true;
	}

	public void DemoOverScreen ()
	{
		levelText.text = "";
		startingLevelMessage = "GAME \"DEMO\" OVER.\n\nThank you for playing.\n\n\nPress any button...";
		levelImage.SetActive (true);
		GameObject.Find ("Canvas").GetComponent<Animator> ().SetTrigger ("GameOver");
		StartCoroutine (TypeEndingText ());
		isPlayerDead = true;
	}

	public void ReturnToMenu ()
	{
		SoundManager.instance.StopPlaying ();
		SceneManager.LoadScene (0);
	}
}
