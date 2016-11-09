using UnityEngine;
using System.Collections;

public class boxOpen : MonoBehaviour {


	private Animator animator;
	public Rigidbody2D myRigidbody;
	public BoxCollider2D coxColl;


	void Start (){

		animator = GetComponent<Animator> ();
		myRigidbody = GetComponent<Rigidbody2D>();
		coxColl = GetComponent<BoxCollider2D> ();
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			animator.SetBool("boxOpen", true);

		}

	}

	void OnTriggerExit2D(Collider other){
		if (other.gameObject.tag == "Player") {
			animator.SetBool ("boxOpen", false);
		}
	}
}
