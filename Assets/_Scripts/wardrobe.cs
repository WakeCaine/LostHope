using UnityEngine;
using System.Collections;

public class wardrobe : MonoBehaviour {


	private Animator animator;
	public Rigidbody2D myRigidbody;
	public BoxCollider2D coxColl;

	public bool isOpen;


	void Start (){

		animator = GetComponent<Animator> ();
		myRigidbody = GetComponent<Rigidbody2D>();
		coxColl = GetComponent<BoxCollider2D> ();
	}
		

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			animator.SetBool("isOpen", true);
		
		}
	
	
	}
		

	void OnTriggerExit2D(Collider2D other){
		animator.SetBool ("isOpen", false);
		}


}
