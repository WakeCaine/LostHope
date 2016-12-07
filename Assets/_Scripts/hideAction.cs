using UnityEngine;
using System.Collections;

public class hideAction : MonoBehaviour {


	public GameObject girl;
	private Animator animator;
	public Rigidbody2D myRigidbody;
	public BoxCollider2D coxColl;

	public bool isOpen;
	public bool playerVisible;
	private bool isTouching;


	void Start (){

		animator = GetComponent<Animator> ();
		myRigidbody = GetComponent<Rigidbody2D>();
		coxColl = GetComponent<BoxCollider2D> ();
		girl = GameObject.Find ("girl");

		isTouching = false;
		playerVisible = true;
	}


	void Update() 
	{
		if (isTouching) {

		
			if (Input.GetKeyDown (KeyCode.Space) && playerVisible) {
				ItemOpen ();
				playerVisible=false;
			
			} 

		
		else if(Input.GetKeyDown (KeyCode.Space) && !playerVisible) {
			animator.SetBool ("isOpen", false);
			girl.SetActive (true);
			playerVisible=true;
			
			}
		}
	}



	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player"){
			isTouching = true;

		}


	}


	void OnTriggerExit2D(Collider2D other){
		isTouching = false;

		//animator.SetBool ("isOpen", false);
	}


	void ItemOpen(){
		animator.SetBool ("isOpen", true);
		girl.SetActive (false);
	
	}

	//void ItemClosed(){
	//	animator.SetBool ("isOpen", false);
	//	GameObject.Find ("girl").SetActive (true);
	
	//}



}
