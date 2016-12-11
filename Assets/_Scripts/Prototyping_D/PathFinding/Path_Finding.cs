using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Path_Finding : MonoBehaviour
{
	Grid_Manager gridManager;
	PathRequest_Manager requestManager;

	public int costVH = 10;
	public int cost = 14;

	void Awake ()
	{
		gridManager = GetComponent<Grid_Manager> ();
		requestManager = GetComponent<PathRequest_Manager> ();
	}

	public void StartFindPath (Vector3 startPos, Vector3 targetPos)
	{
		StartCoroutine (FindPath (startPos, targetPos));
	}

	IEnumerator FindPath (Vector3 startPos, Vector3 targetPos)
	{
		Stopwatch sw = new Stopwatch ();
		sw.Start ();

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		_Node startNode = gridManager.NodeFromWorldPoint (startPos);
		_Node targetNode = gridManager.NodeFromWorldPoint (targetPos);

		if (startNode.walkable && targetNode.walkable) {
			Heap<_Node> openSet = new Heap<_Node> (gridManager.MaxSize);
			HashSet<_Node> closedSet = new HashSet<_Node> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				_Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				if (currentNode == targetNode) {
					sw.Stop ();
					print ("Path found: " + sw.ElapsedMilliseconds + " ms");
					pathSuccess = true;
					break;
				}

				foreach (_Node neighbour in gridManager.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
						} else {
							openSet.UpdateItem (neighbour);
						}
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode);
		}
		requestManager.FinishedProcessingPath (waypoints, pathSuccess);
	}

	Vector3[] RetracePath (_Node startNode, _Node endNode)
	{
		List<_Node> path = new List<_Node> ();
		_Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath (path);

		Array.Reverse (waypoints);

		return waypoints;
	}

	Vector3[] SimplifyPath (List<_Node> path)
	{
		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);
			if (directionNew != directionOld) {
				waypoints.Add (path [i].worldPosition);
			}
			directionOld = directionNew;
		}

		return waypoints.ToArray ();
	}

	int GetDistance (_Node nodeA, _Node nodeB)
	{
		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (dstX > dstY) {
			return cost * dstY + costVH * (dstX - dstY);
		} else {
			return cost * dstX + costVH * (dstY - dstX);
		}
	}
}
