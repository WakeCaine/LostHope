using UnityEngine;
using System.Collections;

public class MovingCameraController : MonoBehaviour
{

	public int boundary = 5;
	public int speed = 3;
	[Range (0, 7)]
	public float closeCamera = 4;

	public bool useCorrection = true;

	private GameObject character;
	private Camera mainCamera;

	void Start ()
	{
		character = GameObject.FindGameObjectWithTag ("Player");
		mainCamera = GetComponent<Camera> ();
	}

	void LateUpdate ()
	{
		mainCamera.orthographicSize = (Screen.height / 100f) / closeCamera;
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
		} else {
			Vector3 charPos = character.transform.position;
			charPos.z = transform.position.z;
			transform.position = Vector3.Lerp (transform.position, charPos, 2f * Time.deltaTime);
		}

	}
}
