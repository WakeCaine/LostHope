using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

	public int boundary = 5;
	public int speed = 3;

	private GameObject character;

	public bool useCorrection = true;

	void Start ()
	{
		character = GameObject.FindGameObjectWithTag ("Player");
	}

	void LateUpdate ()
	{
		if (useCorrection) {
			if (character.transform.position.x > transform.position.x - boundary) {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z), speed * Time.deltaTime);
			}

			if (character.transform.position.x < transform.position.x + boundary) {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z), speed * Time.deltaTime);
			}

			if (character.transform.position.y > transform.position.y - boundary) {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, transform.position.y + speed * Time.deltaTime, transform.position.z), speed * Time.deltaTime);
			}

			if (character.transform.position.y < transform.position.y + boundary) {
				transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, transform.position.y - speed * Time.deltaTime, transform.position.z), speed * Time.deltaTime);
			}
		}

	}
}
