using UnityEngine;
using System.Collections;

public class pgirl : MonoBehaviour {

	private float moveX;
	private float moveY;

	public int batteriesCount;
	public float playerSpeed = 0.8f;
	private float speed = 3f;
	private bool facingRight;


	public Sprite girlUp;



	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {

		facingRight = true;

		transform.position = new Vector3 (0,0, 0);


		spriteRenderer = GetComponent<SpriteRenderer>(); // we are accessing the SpriteRenderer that is attached to the Gameobject

	

	}

	// Update is called once per frame
	void FixedUpdate () {


	
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");

		//GetComponent<Rigidbody2D>().velocity = new Vector2 (moveX * playerSpeed, moveY * playerSpeed);

		Flip (horizontal);

		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Translate(Vector2.right * playerSpeed * Time.deltaTime);
		
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Translate(-Vector2.left * playerSpeed * Time.deltaTime);  

		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.Translate(Vector2.up * playerSpeed * Time.deltaTime);

		}
		else if (Input.GetKey (KeyCode.DownArrow)) {
			transform.Translate(-Vector2.up * playerSpeed * Time.deltaTime);
			spriteRenderer.sprite = girlUp;

		}
			

	}



	private void Flip (float horizontal)

	{
		if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight) 
		{
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;

			theScale.x *=-1;
			transform.localScale = theScale;


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
				other.gameObject.transform.Translate (Vector2.right * playerSpeed*2 * Time.deltaTime);

			}
			else if (Input.GetKey (KeyCode.LeftArrow)) {
				other.gameObject.transform.Translate(-Vector2.right * playerSpeed*2 * Time.deltaTime);  

			}


			if (Input.GetKey (KeyCode.UpArrow)) {
				other.gameObject.transform.Translate (Vector2.up * playerSpeed * 2 * Time.deltaTime);
			}

			else if (Input.GetKey (KeyCode.DownArrow)) {
				transform.Translate(-Vector2.up * playerSpeed*2 * Time.deltaTime);

			}


		}






	}


}
