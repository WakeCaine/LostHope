using UnityEngine;
using System.Collections;

public class EnemyController : MovingObject
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
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		base.Start ();
	}

	public void AddMeToList (bool first)
	{
		Start ();
		if (first)
			GameplayManager.instance.AddEnemyToList (this);
		else
			GameplayManager.instance.AddEnemyToNextList (this);
	}

	public void MoveEnemy ()
	{
		if (target == null) {
			target = GameObject.FindGameObjectWithTag ("Player").transform;
		}
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.y - this.transform.position.y) > 0.5)
			yDir = target.position.y > this.transform.position.y ? 1 : -1;

		if (Mathf.Abs (target.position.x - this.transform.position.x) > 0.5)
			xDir = target.position.x > this.transform.position.x ? 1 : -1;

		if (AttemptMove<HeroPlayerController> (xDir, yDir) == false) {
			if (yDir != 0) {
				xDir = 0;
			} 
			if (AttemptMove<HeroPlayerController> (xDir, yDir) == false) {
				if (xDir != 0) {
					yDir = 0;
				}
			}
		}
	}

	protected override void OnCantMove<T> (T component)
	{
		HeroPlayerController hitPlayer = component as HeroPlayerController;
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
