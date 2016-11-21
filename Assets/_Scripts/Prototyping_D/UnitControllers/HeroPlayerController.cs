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

	public bool canMove = true;

	private SpriteRenderer sprite;
	private Animator animator;
	private GameplayManager gameManager;
	private int food;


	// Use this for initialization
	protected override void Start ()
	{
		animator = GetComponent<Animator> ();
		gameManager = GameObject.FindObjectOfType<GameplayManager> ();
		sprite = GetComponent<SpriteRenderer> ();
		food = GameplayManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;

		base.Start ();
	}

	private void OnDisable ()
	{
		GameplayManager.instance.playerFoodPoints = food;
	}
	// Update is called once per frame
	void Update ()
	{
		
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
		BoardCycleManager m = GameplayManager.instance.boardScript;
		GameplayManager.instance.Level += 1;
		m.SwitchLevel (GameplayManager.instance.Level);
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
			enabled = false;
			Invoke ("Restart", restartLevelDelay);
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
		}
	}

	protected override bool AttemptMove<T> (float xDir, float yDir)
	{

		base.AttemptMove<T> (xDir, yDir);
		if (xDir < 0) {
			sprite.flipX = true;
		} else if (xDir > 0) {
			sprite.flipX = false;
		}

		RaycastHit hit;
		bool canMove = Move (xDir, yDir, out hit);
		if (canMove) {
			SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}

		CheckIfGameOver ();
		return canMove;
	}

	private void CheckIfGameOver ()
	{
		if (food <= 0) {
			SoundManager.instance.musicSource.Stop ();
			SoundManager.instance.RandomizeSfx (gameOverSound);
			GameplayManager.instance.GameOver ();
		}
	}
}