using UnityEngine;
using System.Collections;

public class door : MonoBehaviour {

	private Animator animator;
	public Rigidbody2D myRigidbody;
	public BoxCollider2D coxColl;

	public bool openDoor;


	void Start (){

		animator = GetComponent<Animator> ();
		coxColl = GetComponent<BoxCollider2D> ();

		openDoor = false;
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player") {
			animator.SetBool("openDoor", true);

		}


	}
}
