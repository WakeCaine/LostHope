using UnityEngine;
using System.Collections;

public class hideAction : MonoBehaviour
{

	public GameObject girl;
	private Animator animator;
	public BoxCollider coxColl;

	public bool isOpen;
	public bool playerVisible;
	private bool isTouching;


	void Start ()
	{

		animator = GetComponent<Animator> ();
		coxColl = GetComponent<BoxCollider> ();
		girl = GameObject.FindGameObjectWithTag ("Player");
		isTouching = false;
		playerVisible = true;
	}


	void Update ()
	{
		if (isTouching) {
		
			if (Input.GetKeyDown (KeyCode.Space) && playerVisible && !animator.GetBool ("isOpen")) {
				SoundManager.instance.RandomizeSfx (0.2f, 0, false, true, girl.GetComponent<HeroPlayerController> ().door);
				ItemOpen ();
				playerVisible = false;

			} else if (Input.GetKeyDown (KeyCode.Space) && !playerVisible && !animator.GetBool ("isOpen")) {
				SoundManager.instance.RandomizeSfx (0.2f, 0, false, true, girl.GetComponent<HeroPlayerController> ().door);
				ItemOpen ();	
				playerVisible = true;
			
			}
		}
	}


	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Player") {
			isTouching = true;

		}
			
	}


	void OnTriggerExit (Collider other)
	{
		isTouching = false;

	}


	void ItemOpen ()
	{

		if (playerVisible) {

			girl.SetActive (false);
		
		} else {
			girl.SetActive (true);
		
		}

		animator.SetBool ("isOpen", true);
		StartCoroutine (countTime ());

	
	}

	private IEnumerator countTime ()
	{
		yield return new WaitForSeconds (1);
		//yield return new WaitForSecondsRealtime (1);
		animator.SetBool ("isOpen", false);
	}
}
