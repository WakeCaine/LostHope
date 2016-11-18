using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Random = UnityEngine.Random;

//TODO Comments below. CLEAN SHIT ~Darius
public class BoardCycleManager : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 4;
	public int rows = 4;
	public int nextLevelYPlacement = 10;
	public Count wallCount = new Count (5, 9);
	public Count foodCount = new Count (1, 5);
	public GameObject exit;

	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	public bool nextBoard = false;
	public bool randomGeneration = true;
	public int currentLevelTemplate = 0;

	private Transform boardHolder, boardHolder1;
	private Transform itemHolder, itemHolder1;
	private List<Vector3> gridPositions = new List<Vector3> ();
	private List<Vector3> gridPositions1 = new List<Vector3> ();
	private Stack<MapTemplate> stackOfTemplates = new Stack<MapTemplate> ();
	private int lastGeneratedLevel;

	void Start ()
	{
		RandomizeTemplates ();
	}

	void InitialiseList ()
	{
		(nextBoard ? gridPositions1 : gridPositions).Clear ();
		for (int x = 1; x < columns - 1; x++)
			for (int y = 1; y < rows - 1; y++) {
				(nextBoard ? gridPositions1 : gridPositions).Add (new Vector3 (x, y, 0f));
			}
	}

	void BoardSetup ()
	{
		if (nextBoard) {
			boardHolder1 = new GameObject ("Board" + lastGeneratedLevel).transform;
			itemHolder1 = new GameObject ("Items" + lastGeneratedLevel).transform;
		} else {
			boardHolder = new GameObject ("Board").transform;
			itemHolder = new GameObject ("Items").transform;
		}

		for (int x = -1; x < columns + 1; x++)
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x == -1 || x == columns || y == -1 || y == rows) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}
				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (nextBoard ? boardHolder1 : boardHolder);
			}
	}

	Vector3 RandomPosition ()
	{
		int randomIndex = Random.Range (0, gridPositions1.Count - 1);
		Vector3 randomPosition = nextBoard ? gridPositions1 [randomIndex] : gridPositions [randomIndex];
		(nextBoard ? gridPositions1 : gridPositions).RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		int objectCount = Random.Range (minimum, maximum + 1);
		for (int i = 0; i < objectCount; i++) {
			if ((nextBoard ? gridPositions1 : gridPositions).Count > 0) {
				Vector3 randomPosition = RandomPosition ();
				GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];
				GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
				if (instance.GetComponent<EnemyController> () != null) {
					instance.GetComponent<EnemyController> ().AddMeToList (nextBoard ? false : true);
				}
				instance.transform.SetParent (nextBoard ? itemHolder1 : itemHolder);
			} 
		}
	}

	public void SetupScene (int level, bool nextBoard)
	{
		this.nextBoard = nextBoard;
		this.lastGeneratedLevel = level;

		BoardSetup ();
		InitialiseList ();
		GenerateMapObjects (randomGeneration);

		GameObject instance = Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity) as GameObject;
		if (nextBoard) {
			instance.transform.SetParent (itemHolder1);
			Vector3 newPosition = boardHolder1.transform.position;
			newPosition.y += nextLevelYPlacement;
			boardHolder1.transform.position = newPosition;
			newPosition = itemHolder1.transform.position;
			newPosition.y += nextLevelYPlacement;
			itemHolder1.transform.position = newPosition;
		} else {
			instance.transform.SetParent (itemHolder);
		}

		if (nextBoard)
			nextBoard = false;
	}

	public void GenerateMapObjects (bool random)
	{
		if (random) {
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			int enemyCount = (int)Mathf.Log (lastGeneratedLevel, 2f);
			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		} else {
			GenerateTemplate (currentLevelTemplate);
			currentLevelTemplate++;
		}
	}

	public void SwitchLevel (int level)
	{
		GameplayManager.instance.DoingSetup (true);

		Destroy (boardHolder.gameObject);
		Destroy (itemHolder.gameObject);
		gridPositions = gridPositions1;
		boardHolder = boardHolder1;
		itemHolder = itemHolder1;

		Vector3 newPosition = boardHolder.transform.position;
		newPosition.y -= 10;
		boardHolder.transform.position = newPosition;
		newPosition = itemHolder.transform.position;
		newPosition.y -= 10;
		itemHolder.transform.position = newPosition;

		GameplayManager.instance.SwitchEnemyLists ();
		SetupScene (level, true);

		GameplayManager.instance.DoingSetup (false);
	}

	//TEMPLATE GENERATOR HERE
	//For now in code, we could put it as JSON file instead ~Darius

	public MapTemplate GenerateTemplate (int templateNumber)
	{
		//Template creation here
		List<Vector3> newGridPositions = new List<Vector3> ();
		for (int x = 1; x < columns - 1; x++)
			for (int y = 1; y < rows - 1; y++) {
				newGridPositions.Add (new Vector3 (x, y, 0f));
			}

		Stack<int[][]> objectAndPositon = new Stack<int[][]> ();

		//TODO change to JSON
		//Example 0 - enemy, 1- npc, 2- wall, 3- hideObject, 4- pushableObject
		switch (templateNumber) {
		case 0:
			{
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 2));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 3, 3));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 3));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 3, 2));
			}
		case 1:
			{
				objectAndPositon.Push (LayoutObjectAtPosition (2, 1, 1));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 1));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 3));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 3, 2));
			}
		}
		return new MapTemplate (templateNumber, "Board" + templateNumber, "Items" + templateNumber, objectAndPositon, gridPositions);
	}

	public void RandomizeTemplates ()
	{
		//TODO Make actual templates HERE ~Darius
		//Template pseudo randomization here
		//First two are tutorial
		stackOfTemplates.Push (GenerateTemplate (0));
		stackOfTemplates.Push (GenerateTemplate (1));
		//Next can be randomized
		/*int randNum = Random.Range (0, countTemplates - 1);
		stackOfTemplates.Push (GenerateTemplate (randNum));
		stackOfTemplates.Push (GenerateTemplate (randNum));
		stackOfTemplates.Push (GenerateTemplate (randNum));
		*/
		//...

	}

	//int represents chosen object
	int[] LayoutObjectAtPosition (int chosenObject, int x, int y)
	{
		//TODO rethink this logic ~Darius
		//I think i need to change logic of gridPositions. That is fucked.
		int chosenElement = (x > 1 ? x - 1 * rows : x - 1) + y - 1;
		int[] arrayOfObjects = new int[] { { chosenElement, chosenObject } };
		return arrayOfObjects;
	}

	//Represent template
	struct MapTemplate
	{
		public int number;
		public String boardHolder;
		public String itemHolder;
		public Stack<int[][]> objectAndPosition;
		private List<Vector3> gridPositions;
	}
}
