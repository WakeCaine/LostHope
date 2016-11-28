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
	private float oRange;
	private Light flash;
	private float flashPos = 2.5f;

	private float dir = 1;

	// Use this for initialization
	protected override void Start ()
	{
		animator = GetComponent<Animator> ();
		gameManager = GameObject.FindObjectOfType<GameManager> ();
		sprite = GetComponent<SpriteRenderer> ();
		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;

		// Get Light component of flashlight and get its range
		flash = transform.GetChild (1).GetComponent<Light>();

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
		if (flashPowerLevel < 0) {
			flashPowerLevel = 0;
		}

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
			// Temp battery
			flashPowerLevel += 20f;

			if (flashPowerLevel > 100f) {
				flashPowerLevel = 100f;
			}
			other.gameObject.SetActive (false);
		} else if (other.tag == "NPC") {
			//other.gameObject.transform.position = Vector3.MoveTowards (other.transform.position, new Vector3 (other.transform.position.x + 1, other.transform.position.y, other.transform.position.z), 1f);
		} 
	}

	protected override bool AttemptMove<T> (float xDir, float yDir)
	{
		//food--;
		//foodText.text = "Food: " + food;

		base.AttemptMove<T> (xDir, yDir);
		if (xDir < 0) {
			sprite.flipX = true;
			dir = 2;

		} else if (xDir > 0) {
			sprite.flipX = false;
			dir = 1;
		}

		if (yDir < 0) {
			dir = 3;
		} else if (yDir > 0) {
			dir = 4;
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
		// Raycast
		RaycastHit hitInf;
		bool hit = Physics.Linecast(transform.position,transform.position, out hitInf, 1 << LayerMask.NameToLayer("BlockingLayer"));

		float distance = 0f;

		// get spotLight transform
		Transform fLight = transform.GetChild(1);
		// get Light_Collider transform
		Transform lT = transform.GetChild(2);

		// Update flashlight position, flashlight rotation and light collider rotation
		// 1 is right, 2 is left, 3 is down and 4 is up
		if (dir == 1) {
			hit = Physics.Linecast(transform.position,new Vector2 (transform.position.x + 10,transform.position.y), out hitInf, 1 << LayerMask.NameToLayer("BlockingLayer"));
			Debug.DrawLine (transform.position,new Vector2 (transform.position.x + 10,transform.position.y));

			if(hitInf.collider) {
				distance = hitInf.collider.transform.position.x - transform.position.x;
				print (distance);
			}
				
			lT.localPosition = new Vector3(flashPos, 0.0f, 0.0f);
			lT.rotation = Quaternion.Euler(0, 0, 90);
			fLight.rotation = Quaternion.Euler (0, 60, 0);
		} else if (dir == 2) {
			hit = Physics.Linecast(transform.position,new Vector2 (transform.position.x - 10,transform.position.y), out hitInf, 1 << LayerMask.NameToLayer("BlockingLayer"));
			Debug.DrawLine (transform.position,new Vector2 (transform.position.x - 10,transform.position.y));

			if(hitInf.collider) {
				distance = transform.position.x - hitInf.collider.transform.position.x;
				print (distance);
			}

			lT.rotation = Quaternion.Euler(0, 0, -90);
			lT.localPosition = new Vector3(-flashPos, 0.0f, 0.0f);
			fLight.rotation = Quaternion.Euler (0, -60, 0);
		} else if (dir == 3) {
			hit = Physics.Linecast(transform.position,new Vector2 (transform.position.x,transform.position.y - 10), out hitInf, 1 << LayerMask.NameToLayer("BlockingLayer"));
			Debug.DrawLine (transform.position,new Vector2 (transform.position.x,transform.position.y - 10));

			if(hitInf.collider) {
				distance = transform.position.y - hitInf.collider.transform.position.y;
				print (distance);
			}

			lT.localPosition = new Vector3 (0.0f, -flashPos, 0.0f);
			lT.rotation = Quaternion.Euler(0, 0, 0);
			fLight.rotation = Quaternion.Euler (60, 0, 0);
		} else if (dir == 4) {
			hit = Physics.Linecast(transform.position,new Vector2 (transform.position.x,transform.position.y + 10), out hitInf, 1 << LayerMask.NameToLayer("BlockingLayer"));

			if(hitInf.collider) {
				distance = hitInf.collider.transform.position.y - transform.position.y;
				print (distance);
			}

			lT.localPosition = new Vector3 (0.0f, flashPos, 0.0f);
			lT.rotation = Quaternion.Euler(0, 0, 180);
			fLight.rotation = Quaternion.Euler (-60, 0, 0);
		}
		print (hit);



		// Get flashlight range
		oRange = flash.range;

		// Update flashlight range, light collider size and light collider position
		if (flashPowerLevel > 75) {
			oRange = 10;
			lT.localScale = new Vector3 (1, 4, 1);
			flashPos = 2.5f;
		} else if (flashPowerLevel <= 75 && flashPowerLevel > 50) {
			oRange = 7.5f;
			lT.localScale = new Vector3 (1, 3, 1);
			flashPos = 2.0f;
		} else if (flashPowerLevel <= 50 && flashPowerLevel > 25) {
			oRange = 5f;
			lT.localScale = new Vector3 (1, 2, 1);
			flashPos = 1.5f;
		} else {
			oRange = 2f;
			lT.localScale = new Vector3 (1, 1, 1);
			flashPos = 1.0f;
		}

		if (hitInf.collider && hitInf.collider.tag != "Enemy" && distance < oRange) {
			oRange = distance;
		}
		flash.range = oRange;
	}
}
