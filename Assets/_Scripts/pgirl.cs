using UnityEngine;
using System.Collections;

public class pgirl : MonoBehaviour {

	private float moveX;
	private float moveY;

	public int batteriesCount;
	public float playerSpeed = 0.8f;
	private float speed = 3f;
	private bool facingRight;
	private Animator animator;


	public Sprite girlUp;



	private SpriteRenderer spriteRenderer;

	void Start () {

		facingRight = true;

		transform.position = new Vector3 (0,0, 0);

		animator = this.GetComponent<Animator> ();
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
			animator.SetInteger("direction", 0);
		
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Translate(-Vector2.left * playerSpeed * Time.deltaTime);  
			animator.SetInteger("direction", 0);

		}

		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.Translate(Vector2.up * playerSpeed * Time.deltaTime);
			animator.SetInteger("direction", 2);

		}
		else if (Input.GetKey (KeyCode.DownArrow)) {
			transform.Translate(-Vector2.up * playerSpeed * Time.deltaTime);
			animator.SetInteger("direction", 0);
		

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
