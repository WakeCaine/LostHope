using UnityEngine;
using System.Collections;

public class Light : MonoBehaviour {
	private bool visible = false;

	Collider enemy_hit;

	private void OnTriggerStay (Collider other){
		if (other.tag == "Enemy") {
			enemy_hit = other;
			visible = true;
			print ("In");
		} else {
			visible = false;
			print ("Out");
		}
	}

	void FixedUpdate() {
		if (visible) {
			enemy_hit.GetComponent<SpriteRenderer> ().enabled = true;
		} else {
			enemy_hit.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
