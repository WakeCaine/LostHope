using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;
	public int playerDamage;

	private Animator animator;
	private Transform target;
	private bool skipMove;

	// Use this for initialization
	protected override void Start ()
	{
		GameManager.instance.AddEnemyToList (this);
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		base.Start ();
	}

	public void MoveEnemy ()
	{
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.y - transform.position.y) > 0.5)
			yDir = target.position.y > transform.position.y ? 1 : -1;

		if (Mathf.Abs (target.position.x - transform.position.x) > 0.5)
			xDir = target.position.x > transform.position.x ? 1 : -1;

		if (AttemptMove<PlayerController> (xDir, yDir) == false) {
			if (yDir != 0) {
				xDir = 0;
			} 
			if (AttemptMove<PlayerController> (xDir, yDir) == false) {
				if (xDir != 0) {
					yDir = 0;
				}
			}
		}
	}

	protected override void OnCantMove<T> (T component)
	{
		PlayerController hitPlayer = component as PlayerController;
		animator.SetTrigger ("enemyAttack");
		SoundManager.instance.RandomizeSfx (enemyAttack1, enemyAttack2);
		hitPlayer.LooseFood (playerDamage);
	}

	protected override bool AttemptMove<T> (float xDir, float yDir)
	{
		//if (skipMove) {
		//	skipMove = false;
		//	return;
		//}

		bool canMove = base.AttemptMove<T> (xDir, yDir);
		return canMove;
		//skipMove = true;
	}
}
