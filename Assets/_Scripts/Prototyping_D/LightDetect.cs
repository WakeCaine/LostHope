using UnityEngine;
using System.Collections;

public class LightDetect : MonoBehaviour
{

	private GameObject player;
	private float dir;
	private float oRange;
	private float flashPos = 2.5f;
	private Light flash;
	private Vector3 playerPosition;
	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		flash = player.transform.GetChild (1).GetComponent<Light> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		flashLightDistance ();
	}

	private void OnTriggerEnter (Collider other)
	{
		Debug.LogWarning ("Hue hue");
	}

	private void flashLightDistance ()
	{
		playerPosition = player.transform.position;
		dir = player.GetComponent<HeroPlayerController> ().GetDirection ();
		// Raycast
		RaycastHit hitInf;
		bool hit = Physics.Linecast (playerPosition, playerPosition, out hitInf, 1 << LayerMask.NameToLayer ("BlockingLayer"));

		float distance = 0f;

		// get spotLight transform
		Transform fLight = player.transform.GetChild (1);
		// get Light_Collider transform
		Transform lT = player.transform.GetChild (2);

		// Update flashlight position, flashlight rotation and light collider rotation
		// 1 is right, 2 is left, 3 is down and 4 is up
		if (dir == 1) {
			hit = Physics.Linecast (playerPosition, new Vector2 (playerPosition.x + 10, playerPosition.y), out hitInf, 1 << LayerMask.NameToLayer ("BlockingLayer"));
			Debug.DrawLine (playerPosition, new Vector2 (playerPosition.x + 10, playerPosition.y));

			if (hitInf.collider) {
				distance = hitInf.collider.transform.position.x - playerPosition.x;
			}

			lT.localPosition = new Vector3 (flashPos, 0.0f, 0.0f);
			lT.rotation = Quaternion.Euler (0, 0, 90);
			fLight.rotation = Quaternion.Euler (0, 60, 0);
		} else if (dir == 2) {
			hit = Physics.Linecast (playerPosition, new Vector2 (playerPosition.x - 10, playerPosition.y), out hitInf, 1 << LayerMask.NameToLayer ("BlockingLayer"));
			Debug.DrawLine (playerPosition, new Vector2 (playerPosition.x - 10, playerPosition.y));

			if (hitInf.collider) {
				distance = playerPosition.x - hitInf.collider.transform.position.x;
			}

			lT.rotation = Quaternion.Euler (0, 0, -90);
			lT.localPosition = new Vector3 (-flashPos, 0.0f, 0.0f);
			fLight.rotation = Quaternion.Euler (0, -60, 0);
		} else if (dir == 3) {
			hit = Physics.Linecast (playerPosition, new Vector2 (playerPosition.x, playerPosition.y - 10), out hitInf, 1 << LayerMask.NameToLayer ("BlockingLayer"));
			Debug.DrawLine (playerPosition, new Vector2 (playerPosition.x, playerPosition.y - 10));

			if (hitInf.collider) {
				distance = playerPosition.y - hitInf.collider.transform.position.y;
			}

			lT.localPosition = new Vector3 (0.0f, -flashPos, 0.0f);
			lT.rotation = Quaternion.Euler (0, 0, 0);
			fLight.rotation = Quaternion.Euler (60, 0, 0);
		} else if (dir == 4) {
			hit = Physics.Linecast (transform.position, new Vector2 (playerPosition.x, playerPosition.y + 10), out hitInf, 1 << LayerMask.NameToLayer ("BlockingLayer"));

			if (hitInf.collider) {
				distance = hitInf.collider.transform.position.y - playerPosition.y;
			}

			lT.localPosition = new Vector3 (0.0f, flashPos, 0.0f);
			lT.rotation = Quaternion.Euler (0, 0, 180);
			fLight.rotation = Quaternion.Euler (-60, 0, 0);
		}

		// Get flashlight range
		oRange = flash.range;


		float flashPowerLevel = player.GetComponent<HeroPlayerController> ().flashPowerLevel;
		// Update flashlight range, light collider size and light collider position
		if (flashPowerLevel > 75) {
			oRange = 10;
			lT.localScale = new Vector3 (1, 4, 1);
			flashPos = 2.5f;
		} else if (flashPowerLevel <= 75 && flashPowerLevel > 50) {
			oRange = 7.5f;
			lT.localScale = new Vector3 (1, 3, 1);
			flashPos = 2.0f;
		} else if (flashPowerLevel <= 50 && flashPowerLevel > 25) {
			oRange = 5f;
			lT.localScale = new Vector3 (1, 2, 1);
			flashPos = 1.5f;
		} else {
			oRange = 2f;
			lT.localScale = new Vector3 (1, 1, 1);
			flashPos = 1.0f;
		}
		if (hitInf.collider && hitInf.collider.tag != "Enemy" && distance < oRange) {
			oRange = distance;
		}
		flash.range = oRange;
	}
}
