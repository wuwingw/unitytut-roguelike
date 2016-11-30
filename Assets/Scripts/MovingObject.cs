using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer; // for collision checking

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start () { // virtual functions can be overrided
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1 / moveTime;
	}

	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position; // vector3 -> vector2 discard z
		Vector2 end = start + new Vector2(xDir,yDir);

		boxCollider.enabled = false; // so we don't hit ourself
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true; // renable it

		if (hit.transform == null) {
			StartCoroutine (SmoothMovement (end));
			return true;
		} else {
			return false;
		}
	}

	protected virtual void AttemptMove <T> (int xDir, int yDir)
		where T : Component
	{
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);

		if (hit.transform == null)
			return;

		T hitComponent = hit.transform.GetComponent<T> ();

		if (!canMove && hitComponent != null)
			OnCantMove (hitComponent);
	}

	protected IEnumerator SmoothMovement (Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon) {
			// move a vector towards a point (from current position towards end by an amount)
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}

	}

	protected abstract void OnCantMove<T> (T Component)
		where T : Component;
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
