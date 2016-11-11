using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
	public float moveTime = 0.1f;
	public float speed = 6f;
	public LayerMask blockingLayer;

	private BoxCollider boxCollider;
	private Rigidbody rb2D;
	private float inverseMoveTime;

	private Vector3 s, e;



	// Use this for initialization
	protected virtual void Start ()
	{
		boxCollider = GetComponent<BoxCollider> ();
		rb2D = GetComponent<Rigidbody> ();
		//inverseMoveTime = 1f / moveTime;
	}

	public void OnDrawGizmos ()
	{
		Gizmos.DrawLine (s, e);
	}

	protected bool Move (float xDir, float yDir, out RaycastHit hitt)
	{
		Vector2 start = rb2D.transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);

		RaycastHit hit = new RaycastHit ();
		hitt = new RaycastHit ();

		boxCollider.enabled = false;
		s = new Vector3 (start.x, start.y, 0);
		e = new Vector3 (end.x, end.y, 0);

		Vector3 newPosition = new Vector3 ();
		newPosition.Set (xDir, yDir, 0);
		newPosition = newPosition.normalized * speed * Time.deltaTime;

		Vector3 pos = rb2D.transform.position + new Vector3 (xDir / 2, yDir / 2, 0);
		e = pos;

		bool gotHit = Physics.Linecast (new Vector3 (start.x, start.y, 0), new Vector3 (pos.x, pos.y, 0), out hit, blockingLayer);
		if (gotHit) {
			hitt = hit;
		}
		boxCollider.enabled = true;
		if (hit.transform == null) {
			rb2D.MovePosition (rb2D.transform.position + newPosition);
			return true;
		}

		return false;
	}

	protected virtual bool AttemptMove <T> (float xDir, float yDir)
		where T : Component
	{
		RaycastHit hit = new RaycastHit ();
		bool canMove = Move (xDir, yDir, out hit);

		if (hit.transform == null)
			return false;
		T hitComponent = hit.transform.GetComponent<T> ();

		if (!canMove && hitComponent != null) {
			OnCantMove (hitComponent);
		}

		return canMove;
	}

	protected abstract void OnCantMove <T> (T component)
		where T : Component;
}
