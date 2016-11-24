using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MovingObject
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

	public bool canMove = true;

	private SpriteRenderer sprite;
	private Animator animator;
	private GameManager gameManager;
	private int food;

	private bool flashLight = false;
	public float flashPowerLevel = 100f;

	// Use this for initialization
	protected override void Start ()
	{
		animator = GetComponent<Animator> ();
		gameManager = GameObject.FindObjectOfType<GameManager> ();
		sprite = GetComponent<SpriteRenderer> ();
		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;

		base.Start ();
	}

	private void OnDisable ()
	{
		GameManager.instance.playerFoodPoints = food;
	}
	// Update is called once per frame
	void Update ()
	{
		//if (!GameManager.instance.playersTurn)
		//	return;
		if (flashLight) {
			flashPowerLevel -= 0.05f;
		}
		print (flashPowerLevel);

		flashLightDistance ();

		if (Input.GetKeyDown ("f")) {
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

	void FixedUpdate ()
	{
		if (!canMove)
			return;
		else if (gameManager.doingSetup)
			return;
		
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall> (horizontal, vertical);
		}
	}

	protected override void OnCantMove<T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	private void Restart ()
	{
		SceneManager.LoadScene (1);
	}

	public void LooseFood (int loss)
	{
		animator.SetTrigger ("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver ();
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "NPC") {
			//other.gameObject.transform.position = Vector3.MoveTowards (other.transform.position, new Vector3 (other.transform.position.x + 1, other.transform.position.y, other.transform.position.z), 1f);
		} else if (other.tag == "Batteries") {
			flashPowerLevel += 20f;

			if (flashPowerLevel > 100f) {
				flashPowerLevel = 100f;
			}
		}
	}

	protected override bool AttemptMove<T> (float xDir, float yDir)
	{
		//food--;
		//foodText.text = "Food: " + food;

		base.AttemptMove<T> (xDir, yDir);
		if (xDir < 0) {
			sprite.flipX = true;

			// Light rotation
			transform.GetChild (1).rotation = Quaternion.Euler (0, -60, 0);
			// End

			// Light_Trigger rotation and movement
			Transform lT = transform.GetChild(2);
			lT.rotation = Quaternion.Euler(0, 0, -90);
			lT.localPosition = new Vector3(-2.5f, 0.0f, 0.0f);
			// End

		} else if (xDir > 0) {
			sprite.flipX = false;

			// Light rotation
			transform.GetChild (1).rotation = Quaternion.Euler (0, 60, 0);
			// End

			// Light_Trigger rotation and movement
			Transform lT = transform.GetChild(2);
			lT.rotation = Quaternion.Euler(0, 0, 90);
			lT.localPosition = new Vector3(2.5f, 0.0f, 0.0f);
			// End
		}

		if (yDir < 0) {
			// Light rotation
			transform.GetChild (1).rotation = Quaternion.Euler (60, 0, 0);
			// End

			// Light_Trigger rotation and movement
			Transform lT = transform.GetChild(2);
			lT.rotation = Quaternion.Euler(0, 0, 0);
			lT.localPosition = new Vector3(0.0f, -2.5f, 0.0f);
			// End
		} else if (yDir > 0) {
			// Light rotation
			transform.GetChild (1).rotation = Quaternion.Euler (-60, 0, 0);
			// End

			// Light_Trigger rotation and movement
			Transform lT = transform.GetChild(2);
			lT.rotation = Quaternion.Euler(0, 0, 180);
			lT.localPosition = new Vector3(0.0f, 2.5f, 0.0f);
			// End
		}

		RaycastHit hit;
		bool canMove = Move (xDir, yDir, out hit);
		if (canMove) {
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}

		CheckIfGameOver ();
		return canMove;
		//GameManager.instance.playersTurn = false;
	}

	private void CheckIfGameOver ()
	{
		if (food <= 0) {
			SoundManager.instance.musicSource.Stop ();
			SoundManager.instance.RandomizeSfx (gameOverSound);
			GameManager.instance.GameOver ();
		}
	}

	private void flashLightDistance () {

	}
}
