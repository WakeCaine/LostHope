using UnityEngine;
using System.Collections;

public class boxCollider : MonoBehaviour {


	public float speed = 0.1f;


	void OnTriggerEnter(Collider collider) {

	
			if (collider.gameObject.CompareTag ("Player")) {

		collider.gameObject.transform.Translate(Vector2.right * speed * Time.deltaTime);



		}



}

}
// transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);