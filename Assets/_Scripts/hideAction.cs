using UnityEngine;
using System.Collections;

public class hideAction : MonoBehaviour {


	public GameObject girl;
	private Animator animator;
	public Rigidbody2D myRigidbody;
	public BoxCollider2D coxColl;

	public bool isOpen;

	private bool isTouching;


	void Start (){

		animator = GetComponent<Animator> ();
		myRigidbody = GetComponent<Rigidbody2D>();
		coxColl = GetComponent<BoxCollider2D> ();

		isTouching = false;
	}


	void Update() 
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (isTouching)
				ItemOpen ();
		
		}

	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player"){
			isTouching = true;

			//animator.SetBool("isOpen", true);

		}


	}


	void OnTriggerExit2D(Collider2D other){
		isTouching = false;

		//animator.SetBool ("isOpen", false);
	}


	void ItemOpen(){
		animator.SetBool ("isOpen", true);
	//	GameObject.Find("girl").GetComponent().enabled = false;

	}



}
