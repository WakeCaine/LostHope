using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{

	public Transform target;
	float speed = 20;
	Vector3[] path;
	int targetIndex;

	private Vector3 targetV;
	private Vector3 targetLastV;

	void Start ()
	{
		PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
	}

	void Update ()
	{
		targetLastV = targetLastV != null ? targetV : target.position;
		targetV = target.position;

		if (!targetLastV.Equals (targetV)) {
			PathRequestManager.RequestPath (transform.position, targetV, OnPathFound);
		}
	}

	public void OnPathFound (Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful && newPath.Length > 0) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}

	IEnumerator FollowPath ()
	{
		Vector3 currentWaypoint = path [0];

		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}

			transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos ()
	{
		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				Gizmos.color = Color.blue;
				Gizmos.DrawCube (path [i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				} else {
					Gizmos.DrawLine (path [i - 1], path [i]);
				}
			}
		}
	}
}
