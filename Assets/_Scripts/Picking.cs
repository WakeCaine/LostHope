using UnityEngine;
using System.Collections;

public class Picking : MonoBehaviour {

	void OnTriggerEnter(Collider collider) {

		switch(collider.gameObject.name) {

		case "Player" :

			Destroy (this.gameObject);


			break;


		}

	}


}
