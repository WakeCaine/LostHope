using UnityEngine;
using System.Collections;

public class PlayerGirl : MonoBehaviour {

	[SerializeField]
	private float speed = 3f;
	private float moveSpeed = 1f;
	public bool toOpen = false;

	public Sprite girl;
	public Sprite girlUp;
	public Sprite girlLeft;

	private Rigidbody2D myRigidbody;
	private SpriteRenderer spriteRenderer;
	private int batteriesCount;


	void Start ()
	{

		myRigidbody = GetComponent<Rigidbody2D>();

		spriteRenderer = GetComponent<SpriteRenderer>(); // we are accessing the SpriteRenderer that is attached to the Gameobject

		if (spriteRenderer.sprite == null) // if the sprite on spriteRenderer is null then
			spriteRenderer.sprite = girl; // set the sprite to sprite


	}
		

	void FixedUpdate ()

	{

		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Translate(Vector2.right * speed * Time.deltaTime);
			spriteRenderer.sprite = girl;

		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Translate(-Vector2.right * speed * Time.deltaTime);  
			spriteRenderer.sprite = girlLeft;
		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.Translate(Vector2.up * speed * Time.deltaTime);
			spriteRenderer.sprite = girlUp;
		}
		else if (Input.GetKey (KeyCode.DownArrow)) {
			transform.Translate(-Vector2.up * speed * Time.deltaTime);
			spriteRenderer.sprite = girl;
		}
			

	}
		


	void OnTriggerEnter2D(Collider2D other){
	
		if (other.gameObject.CompareTag ("Batteries")) 
		{
			other.gameObject.SetActive (false);
			batteriesCount++;

		}

		if (other.gameObject.CompareTag ("box")) {

	
			if (Input.GetKey (KeyCode.RightArrow)) {
				other.gameObject.transform.Translate (Vector2.right * speed*2 * Time.deltaTime);

			}
			else if (Input.GetKey (KeyCode.LeftArrow)) {
				other.gameObject.transform.Translate(-Vector2.right * speed*2 * Time.deltaTime);  

			}


			if (Input.GetKey (KeyCode.UpArrow)) {
				other.gameObject.transform.Translate (Vector2.up * speed * 2 * Time.deltaTime);
			}

			else if (Input.GetKey (KeyCode.DownArrow)) {
				transform.Translate(-Vector2.up * speed*2 * Time.deltaTime);
			
			}

	
		}


	

	
	
	}





	}

