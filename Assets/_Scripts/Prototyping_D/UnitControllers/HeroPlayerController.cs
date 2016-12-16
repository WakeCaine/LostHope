using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroPlayerController : MovingObject
{
	public Text foodText;
	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;
	public AudioClip door;

	public bool canMove = true;
	public bool disabledMove = false;

	private SpriteRenderer sprite;
	private Animator animator;
	private GameplayManager gameManager;
	private int food;

	public float flashPowerLevel = 100f;
	public bool flashLight = false;

	private float dir = 1;
	public bool pickedFlashlight = false;

	GameObject batteryImage;
	private Grid_Manager gridManager;

	bool gameOverTrigger = false;

	// Use this for initialization

	void Awake ()
	{
		
	}

	protected override void Start ()
	{
		//animator = GetComponent<Animator> ();
		gameManager = GameObject.FindObjectOfType<GameplayManager> ();
		gridManager = GameObject.Find ("A_").GetComponent<Grid_Manager> ();
		sprite = GetComponent<SpriteRenderer> ();
		animator = this.GetComponent<Animator> ();
		food = GameplayManager.instance.playerFoodPoints;
		batteryImage = GameObject.Find ("Battery");
		batteryImage.SetActive (false);
		//foodText.text = "Food: " + food;

		base.Start ();
	}

	private void OnDisable ()
	{
		GameplayManager.instance.playerFoodPoints = food;
	}
	// Update is called once per frame
	void Update ()
	{
		if (disabledMove)
			return;
		if (!canMove)
			return;
		else if (gameManager.doingSetup)
			return;

		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");

		Vector2 movementVector = new Vector2 (horizontal, vertical);

		if (horizontal != 0)
			vertical = 0;

		if (movementVector != Vector2.zero) {
			AttemptMove<Wall> (horizontal, vertical);
		}
	}

	void LateUpdate ()
	{
		if (flashLight) {
			flashPowerLevel -= 0.05f;
		}
		if (flashPowerLevel < 0) {
			flashPowerLevel = 0;
		}

		if (Input.GetKeyDown ("f")) {
			if (pickedFlashlight)
			if (flashLight) {
				transform.GetChild (1).gameObject.SetActive (false);
				transform.GetChild (2).gameObject.SetActive (false);
				flashLight = false;
			} else {
				transform.GetChild (1).gameObject.SetActive (true);
				transform.GetChild (2).gameObject.SetActive (true);
				flashLight = true;
			}
		}
	}

	protected override void OnCantMove<T> (T component)
	{
		//Wall hitWall = component as Wall;
		//hitWall.DamageWall (wallDamage);
		//animator.SetTrigger ("playerChop");
	}

	private void Restart ()
	{
		BoardCycleManager m = GameplayManager.instance.boardScript;
		if (m.stackOfTemplates.Count > 0) {
			GameplayManager.instance.Level += 1;
			m.SwitchLevel (GameplayManager.instance.Level, this.transform.position);
			float x = this.transform.position.x + 1; //9,5
			float y = this.transform.position.y + 1; //3
			int rows = GameplayManager.instance.boardScript.rows;
			int columns = GameplayManager.instance.boardScript.columns;
			if (x < (int)(columns / 2) + 1.5 && x > (int)(columns / 2) - 1.5 && y > rows) {
				this.transform.position = new Vector3 ((int)(columns / 2), 0, 0);
			} else if (x < (int)(columns / 2) + 1.5 && x > (int)(columns / 2) - 1.5 && y < 1) {
				this.transform.position = new Vector3 ((int)(columns / 2), rows - 1, 0);
			} else if (y < (int)(rows / 2) + 1.5 && y > (int)(rows / 2) - 1.5 && x > columns) {
				this.transform.position = new Vector3 (0, (int)(rows / 2), 0);
			} else if (y < (int)(rows / 2) + 1.5 && y > (int)(rows / 2) - 1.5 && x < 1) {
				this.transform.position = new Vector3 (columns - 1, (int)(rows / 2), 0);
			} 
				
			enabled = true;
			gridManager.CreateGrid ();
		} else {
			batteryImage.SetActive (false);
			DemoOver ();
		}
	}

	private void OnTriggerEnter (Collider other)
	{
		Debug.LogWarning ("Hue hue");
		if (other.tag == "Exit") {
			enabled = false;
			other.gameObject.GetComponent<Animator> ().SetBool ("isOpen", true);
			SoundManager.instance.RandomizeSfx (0.2f, 0, false, true, door);
			Invoke ("Restart", restartLevelDelay);
		} else if (other.tag == "Food") {
			// Temp battery
			flashPowerLevel += 20f;

			if (flashPowerLevel > 100f) {
				flashPowerLevel = 100f;
			}
			//SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "Soda") {
			// Temp battery
			flashPowerLevel += 20f;

			if (flashPowerLevel > 100f) {
				flashPowerLevel = 100f;
			}
			//SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "Flashlight") {
			// Temp battery
			pickedFlashlight = true;
			batteryImage.SetActive (true);
			//SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "NPC") {
			//other.gameObject.transform.position = Vector3.MoveTowards (other.transform.position, new Vector3 (other.transform.position.x + 1, other.transform.position.y, other.transform.position.z), 1f);
		} 
	}

	protected override bool AttemptMove<T> (float xDir, float yDir)
	{

		base.AttemptMove<T> (xDir, yDir);
		if (xDir < 0) {
			sprite.flipX = true;
			dir = 2;
			animator.SetInteger ("direction", 0);

		} else if (xDir > 0) {
			sprite.flipX = false;
			dir = 1;
			animator.SetInteger ("direction", 0);
		}

		if (yDir < 0) {
			dir = 3;
			animator.SetInteger ("direction", 0);
		} else if (yDir > 0) {
			dir = 4;
			animator.SetInteger ("direction", 2);
		}

		RaycastHit hit;
		bool canMove = Move (xDir, yDir, out hit);
		if (canMove) {
			SoundManager.instance.RandomizeSfx (0.25f, 1.25f, true, moveSound1, moveSound2);
		}

		return canMove;
	}

	public void PlayerDead ()
	{
		gameManager.doingSetup = true;
		batteryImage.SetActive (false);
		SoundManager.instance.RandomizeSfx (0.5f, 1.85f, false, false, gameOverSound);
		Invoke ("GameOver", restartLevelDelay);
	}

	private void GameOver ()
	{
		gameManager.GameOverScreen ();
	}

	private void DemoOver ()
	{
		gameManager.DemoOverScreen ();
	}

	public float GetDirection ()
	{
		return dir;
	}
}