using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid_Manager : MonoBehaviour
{
	public bool displayGridGizmos;
	public Transform player;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	_Node[,] grid;

	void Awake ()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);
	}

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	public void CreateGrid ()
	{
		grid = new _Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
				Collider[] whatCollides = Physics.OverlapSphere (worldPoint, nodeRadius, unwalkableMask);
				bool isPlayer = false;
				foreach (Collider what in whatCollides) {
					isPlayer = what.CompareTag ("Player");
					isPlayer = what.CompareTag ("Enemy") ? true : isPlayer;
				}
				bool walkable = isPlayer ? true : !(Physics.CheckSphere (worldPoint, nodeRadius, unwalkableMask));
				grid [x, y] = new _Node (walkable, worldPoint, x, y);
			}
		}
	}

	public List<_Node> GetNeighbours (_Node node)
	{
		List<_Node> neighbours = new List<_Node> ();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add (grid [checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	public _Node NodeFromWorldPoint (Vector3 worldPosition)
	{ 
		float precentX = ((worldPosition.x + gridWorldSize.x / 2) - transform.position.x) / gridWorldSize.x;
		float precentY = ((worldPosition.y + gridWorldSize.y / 2) - transform.position.y) / gridWorldSize.y;
		precentX = Mathf.Clamp01 (precentX);
		precentY = Mathf.Clamp01 (precentY);

		int x = Mathf.RoundToInt ((gridSizeX - 1) * precentX);
		int y = Mathf.RoundToInt ((gridSizeY - 1) * precentY);
		return grid [x, y];
	}

	public List<_Node> path;

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, gridWorldSize.y, 1));
		if (grid != null && displayGridGizmos) {
			_Node playerNode = NodeFromWorldPoint (player.position);
			foreach (_Node n in grid) {
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				if (playerNode == n) {
					Gizmos.color = Color.cyan;
				}
				Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}
}
