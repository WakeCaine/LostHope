using UnityEngine;
using System.Collections;

public class PlayerSeeker : MonoBehaviour
{

	public Transform target;
	float speed = 1;
	Vector3[] path;
	int targetIndex = 0;

	private Vector3 targetV;
	private Vector3 targetLastV;
	private Vector3 myV;
	private bool following = false;
	private Grid_Manager gridManager;
	private bool visible = false;

	private HeroPlayerController hero;
	int seekTime = 0;

	void Awake ()
	{
		gridManager = GameObject.Find ("A_").GetComponent<Grid_Manager> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		// Enemies start out as invisible
		GetComponent<SpriteRenderer> ().enabled = false;
		// End
	}

	void Start ()
	{
		//PathRequest_Manager.RequestPath (transform.position, target.position, OnPathFound);
		hero = GameObject.FindGameObjectWithTag ("Player").GetComponent<HeroPlayerController> ();
	}

	void Update ()
	{
		targetV = target.position;

		/*if (!following && !targetLastV.Equals (targetV)) {
			targetLastV = targetV;
			following = true;
			PathRequest_Manager.RequestPath (transform.position, targetV, OnPathFound);
		}*/
	}

	void FixedUpdate ()
	{
		if (visible) {
			GetComponent<SpriteRenderer> ().enabled = true;
		} else if (!visible) {
			GetComponent<SpriteRenderer> ().enabled = false;
		}
		visible = false;

		if (seekTime == 0) {
			StartCoroutine (Seek ());
		}

		if (seekTime > 3) {
			PathRequest_Manager.RequestPath (transform.position, myV, OnPathFound);
		}
	}

	IEnumerator Seek ()
	{
		for (int i = 0; i < 3; i++) {
			seekTime += 1;
			yield return new WaitForSeconds (1);
		}
	}

	public void OnPathFound (Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful && newPath.Length > 0 && this != null) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		} else {
			following = false;
		}
	}

	IEnumerator FollowPath ()
	{
		targetIndex = 0;
		seekTime = 0;
		Vector3 currentWaypoint = path [0];

		while (targetIndex != path.Length) {
			if (transform.position == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					following = false;
					targetIndex = 0;
					path = null;
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}

			transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
		following = false;
	}

	public void OnDrawGizmos ()
	{
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.blue;
				Gizmos.DrawCube (path [i], Vector3.one * ((gridManager.nodeRadius * 2) - .1f));

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				} else {
					Gizmos.DrawLine (path [i - 1], path [i]);
				}
			}
		}
	}

	private void OnTriggerEnter (Collider other)
	{
		myV = this.gameObject.transform.position;
		if (other.tag == "Hero_Ambient") {
			if (!following && !targetLastV.Equals (targetV)) {
				visible = true;
				targetLastV = targetV;
				following = true;
				PathRequest_Manager.RequestPath (transform.position, targetV, OnPathFound);
			}
		} else if (other.tag == "Light_Trigger") {
			if (!following && !targetLastV.Equals (targetV)) {
				visible = true;
				targetLastV = targetV;
				following = true;
				PathRequest_Manager.RequestPath (transform.position, targetV, OnPathFound);
			}
		} else if (other.tag == "PlayerBody") {
			hero.PlayerDead ();
		}
	}

	private void OnTriggerStay (Collider other)
	{
		if (other.tag == "Hero_Ambient") {
			visible = true;
		} else if (other.tag == "Light_Trigger") {
			visible = true;
		}
	}
}
