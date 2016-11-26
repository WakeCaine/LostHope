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

	enum GameObjectType
	{
		InnerWall,
		Item,
		Exit,
		Enemy,
		Npc,
		Hero
	}

	enum MapObjectType
	{
		InnerWall,
		Item,
		Exit,
		Enemy,
		Npc,
		Hero,
		Interactable,
		Pushable
	}

	public int columns = 9;
	public int rows = 5;
	public int nextLevelYPlacement = 10;
	public Count wallCount = new Count (5, 9);
	public Count foodCount = new Count (1, 5);
	public GameObject exitOrginal;

	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	public bool nextBoard = false;
	public bool randomGeneration = false;
	public int currentLevelTemplate = 1;

	private Transform boardHolder, boardHolder1;
	private Transform itemHolder, itemHolder1;
	private List<Vector3> gridPositions = new List<Vector3> ();
	private List<Vector3> gridPositions1 = new List<Vector3> ();
	private Stack<MapTemplate> stackOfTemplates = new Stack<MapTemplate> ();
	private int lastGeneratedLevel;
	private Vector3 lastPlayerPosition;
	private Light roomLight;

	void Start ()
	{
		RandomizeTemplates ();
		nextLevelYPlacement = columns + 1;
		roomLight = GameObject.FindGameObjectWithTag ("Light").GetComponent<Light> ();
	}

	void Update ()
	{
		if (lastGeneratedLevel > 2 && roomLight.intensity > 0)
			roomLight.intensity = 0;
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

		GameObject exitInstance = null, exitInstance1 = null, exitInstance2 = null, exitInstance3 = null;
		//0 - top, 1- right, 2- down, 3- left ------- going with the clock
		List<int> alaviableExits = new List<int> ();
		for (int i = 0; i < 4; i++)
			alaviableExits.Add (i);


		int numberOfExits = 0;
		for (int i = 0; i < 10; i++) {
			numberOfExits = Random.Range (1, 4);
			if (i > 6 && numberOfExits > 3)
				numberOfExits = Random.Range (1, 4);
		}

		GameObject exit = exitOrginal;
		if (!nextBoard) {
			if (randomGeneration) {
				switch (numberOfExits) {
				case 1:
					exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
					exit = outerWallTiles [outerWallTiles.Length - 1];
					exitInstance = ChooseExitPlacement (alaviableExits, exit, false);
					exitInstance2 = ChooseExitPlacement (alaviableExits, exit, false);
					exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
					break;
				case 2:
					exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
					exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
					exit = outerWallTiles [outerWallTiles.Length - 1];
					exitInstance = ChooseExitPlacement (alaviableExits, exit, false);
					exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
					break;
				case 3:
					exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
					exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
					exitInstance3 = ChooseExitPlacement (alaviableExits, exit, true);
					exit = outerWallTiles [outerWallTiles.Length - 1];
					exitInstance = ChooseExitPlacement (alaviableExits, exit, false);
					break;
				}
			} else {
				switch (lastGeneratedLevel) {
				case 1:
					exitInstance = ChooseExitPlacement (alaviableExits, exit, true, 1);
					exit = outerWallTiles [outerWallTiles.Length - 1];
					exitInstance1 = ChooseExitPlacement (alaviableExits, exit, false);
					exitInstance2 = ChooseExitPlacement (alaviableExits, exit, false);
					exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
					break;
				default:
					switch (numberOfExits) {
					case 1:
						exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
						exit = outerWallTiles [outerWallTiles.Length - 1];
						exitInstance2 = ChooseExitPlacement (alaviableExits, exit, false);
						exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
						exitInstance = ChooseExitPlacement (alaviableExits, exit, false);
						break;
					case 2:
						exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
						exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
						exit = outerWallTiles [outerWallTiles.Length - 1];
						exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
						exitInstance = ChooseExitPlacement (alaviableExits, exit, false);
						break;
					case 3:
						exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
						exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
						exitInstance3 = ChooseExitPlacement (alaviableExits, exit, true);
						exit = outerWallTiles [outerWallTiles.Length - 1];
						exitInstance = ChooseExitPlacement (alaviableExits, exit, false);
						break;
					}
					break;				
				}
			}
		}
			
		if (exitInstance != null)
			exitInstance.transform.SetParent (nextBoard ? itemHolder1 : itemHolder);
		if (exitInstance1 != null)
			exitInstance1.transform.SetParent (nextBoard ? itemHolder1 : itemHolder);
		if (exitInstance2 != null)
			exitInstance2.transform.SetParent (nextBoard ? itemHolder1 : itemHolder);
		if (exitInstance3 != null)
			exitInstance3.transform.SetParent (nextBoard ? itemHolder1 : itemHolder);

		for (int x = -1; x < columns + 1; x++)
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
				if ((x == -1 || x == columns) && y == (rows / 2)) {
					toInstantiate = null;
				} else if (x == (int)(columns / 2) && (y == -1 || y == rows)) {
					toInstantiate = null;
				} else if (y == rows && (x > -1 && x != (int)(columns / 2) && x < columns)) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length - 2)];
				} else if ((x == -1 || x == columns || y == -1) && x != (int)(columns / 2)) {
					toInstantiate = outerWallTiles [outerWallTiles.Length - 1];
				}
				GameObject instance = null;
				if (toInstantiate != null)
					instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				if (instance != null)
					instance.transform.SetParent (nextBoard ? boardHolder1 : boardHolder);
			}
	}

	Vector3 RandomPosition ()
	{
		int randomIndex = Random.Range (0, nextBoard ? gridPositions1.Count : gridPositions.Count);
		Vector3 randomPosition = nextBoard ? gridPositions1 [randomIndex] : gridPositions [randomIndex];
		(nextBoard ? gridPositions1 : gridPositions).RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom (GameObject[] tileArray, GameObjectType objectType, int minimum, int maximum)
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

		if (nextBoard) {
			Vector3 newPosition = boardHolder1.transform.position;
			newPosition.y += nextLevelYPlacement;
			boardHolder1.transform.position = newPosition;
			newPosition = itemHolder1.transform.position;
			newPosition.y += nextLevelYPlacement;
			itemHolder1.transform.position = newPosition;
			nextBoard = false;
		}
	}

	public void GenerateMapObjects (bool random)
	{
		if (random) {
			LayoutObjectAtRandom (wallTiles, GameObjectType.InnerWall, wallCount.minimum, wallCount.maximum);
			LayoutObjectAtRandom (foodTiles, GameObjectType.Item, foodCount.minimum, foodCount.maximum);
			int enemyCount = (int)Mathf.Log (lastGeneratedLevel, 2f);
			LayoutObjectAtRandom (enemyTiles, GameObjectType.Enemy, enemyCount, enemyCount);
		} else {
			if (stackOfTemplates.Count > 0)
				SpawnObjectsAtPosition (stackOfTemplates);
		}
	}

	public void SwitchLevel (int level, Vector3 lastPosition)
	{
		
		GameplayManager.instance.DoingSetup (true);

		lastPlayerPosition = lastPosition;
		Destroy (boardHolder.gameObject);
		Destroy (itemHolder.gameObject);

		gridPositions = gridPositions1;
		boardHolder = boardHolder1;
		itemHolder = itemHolder1;

		Vector3 newPosition = boardHolder.transform.position;
		newPosition.y -= nextLevelYPlacement;
		boardHolder.transform.position = newPosition;
		newPosition = itemHolder.transform.position;
		newPosition.y -= nextLevelYPlacement;
		itemHolder.transform.position = newPosition;

		//------------------------------------------------
		float xx = lastPlayerPosition.x + 1;
		float yy = lastPlayerPosition.y + 1;

		GameObject exitInstance = null, exitInstance1 = null, exitInstance2 = null, exitInstance3 = null;

		//0 - top, 1- right, 2- down, 3- left ------- going with the clock
		List<int> alaviableExits = new List<int> ();
		for (int i = 0; i < 4; i++)
			alaviableExits.Add (i);

		int numberOfExits = 0;
		for (int i = 0; i < 10; i++) {
			numberOfExits = Random.Range (1, 4);
			if (i > 6 && numberOfExits > 3)
				numberOfExits = Random.Range (1, 4);
		}

		GameObject exit = exitOrginal;

		if (xx < (int)(columns / 2) + 1.5 && xx > (int)(columns / 2) - 1.5 && yy > rows) {
			exitInstance = outerWallTiles [outerWallTiles.Length - 2];
			exitInstance = Instantiate (exitInstance, new Vector3 ((int)(columns / 2), -1, 0), Quaternion.identity) as GameObject;
			alaviableExits.Remove (2);
		} else if (xx < (int)(columns / 2) + 1.5 && xx > (int)(columns / 2) - 1.5 && yy < 1) {
			exitInstance = outerWallTiles [outerWallTiles.Length - 2];
			exitInstance = Instantiate (exitInstance, new Vector3 ((int)(columns / 2), rows, 0), Quaternion.identity) as GameObject;
			alaviableExits.Remove (0);
		} else if (yy < (int)(rows / 2) + 1.5 && yy > (int)(rows / 2) - 1.5 && xx > columns) {
			exitInstance = outerWallTiles [outerWallTiles.Length - 2];
			exitInstance = Instantiate (exitInstance, new Vector3 (-1, (int)(rows / 2), 0), Quaternion.identity) as GameObject;
			alaviableExits.Remove (3);
		} else if (yy < (int)(rows / 2) + 1.5 && yy > (int)(rows / 2) - 1.5 && xx < 1) {
			exitInstance = outerWallTiles [outerWallTiles.Length - 2];
			exitInstance = Instantiate (exitInstance, new Vector3 (columns, (int)(rows / 2), 0), Quaternion.identity) as GameObject;
			alaviableExits.Remove (1);
		}

		if (randomGeneration) {
			switch (numberOfExits) {
			case 1:
				exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
				exit = outerWallTiles [outerWallTiles.Length - 1];
				exitInstance2 = ChooseExitPlacement (alaviableExits, exit, false);
				exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
				break;
			case 2:
				exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
				exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
				exit = outerWallTiles [outerWallTiles.Length - 1];
				exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
				break;
			case 3:
				exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
				exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
				exitInstance3 = ChooseExitPlacement (alaviableExits, exit, true);
				break;
			}
		} else {
			switch (lastGeneratedLevel) {
			case 2:
				exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true, 1);
				exit = outerWallTiles [outerWallTiles.Length - 1];
				exitInstance2 = ChooseExitPlacement (alaviableExits, exit, false);
				exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
				break;
			default:
				RandomExit (ref exitInstance1, ref exitInstance2, ref exitInstance3, alaviableExits, exit, numberOfExits);
				break;
				
			}
		}

		nextBoard = true;
		if (nextBoard) {
			if (exitInstance != null)
				exitInstance.transform.SetParent (itemHolder);
			if (exitInstance1 != null)
				exitInstance1.transform.SetParent (itemHolder);
			if (exitInstance2 != null)
				exitInstance2.transform.SetParent (itemHolder);
			if (exitInstance3 != null)
				exitInstance3.transform.SetParent (itemHolder);
		}

		//--------------------------------------------

		GameplayManager.instance.SwitchEnemyLists ();
		SetupScene (level, true);

		GameplayManager.instance.DoingSetup (false);
	}

	void RandomExit (ref GameObject exitInstance1, ref GameObject exitInstance2, ref GameObject exitInstance3, List<int> alaviableExits, GameObject exit, int numberOfExits)
	{
		switch (numberOfExits) {
		case 1:
			exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
			exit = outerWallTiles [outerWallTiles.Length - 1];
			exitInstance2 = ChooseExitPlacement (alaviableExits, exit, false);
			exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
			break;
		case 2:
			exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
			exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
			exit = outerWallTiles [outerWallTiles.Length - 1];
			exitInstance3 = ChooseExitPlacement (alaviableExits, exit, false);
			break;
		case 3:
			exitInstance1 = ChooseExitPlacement (alaviableExits, exit, true);
			exitInstance2 = ChooseExitPlacement (alaviableExits, exit, true);
			exitInstance3 = ChooseExitPlacement (alaviableExits, exit, true);
			break;
		}
	}

	GameObject ChooseExitPlacement (List<int> alaviableExitss, GameObject exit, bool isExit)
	{
		//0 - top, 1- right, 2- down, 3- left ------- going with the clock
		int chosenExit = Random.Range (0, alaviableExitss.Count);
		int found = alaviableExitss [chosenExit];
		alaviableExitss.RemoveAt (chosenExit);
		GameObject localExit = exit;
		Vector3 pos = Vector3.zero;
		switch (found) {
		case 0:
			if (!isExit)
				localExit = outerWallTiles [Random.Range (0, outerWallTiles.Length - 2)];
			pos = new Vector3 ((int)(columns / 2), rows, 0);
			break;
		case 1:
			pos = new Vector3 (columns, (int)(rows / 2), 0);
			break;
		case 2:
			pos = new Vector3 ((int)(columns / 2), -1, 0);
			break;
		case 3:
			pos = new Vector3 (-1, (int)(rows / 2), 0);
			break;
		default:
			pos = new Vector3 (-1, (int)(rows / 2), 0);
			break;
		}

		return Instantiate (localExit, pos, Quaternion.identity) as GameObject;
	}

	GameObject ChooseExitPlacement (List<int> alaviableExitss, GameObject exit, bool isExit, int whatExit)
	{
		//0 - top, 1- right, 2- down, 3- left ------- going with the clock
		int chosenExit = whatExit;
		int found = alaviableExitss [chosenExit];
		alaviableExitss.RemoveAt (chosenExit);
		GameObject localExit = exit;
		Vector3 pos = Vector3.zero;
		switch (found) {
		case 0:
			if (!isExit)
				localExit = outerWallTiles [Random.Range (0, outerWallTiles.Length - 2)];
			pos = new Vector3 ((int)(columns / 2), rows, 0);
			break;
		case 1:
			pos = new Vector3 (columns, (int)(rows / 2), 0);
			break;
		case 2:
			pos = new Vector3 ((int)(columns / 2), -1, 0);
			break;
		case 3:
			pos = new Vector3 (-1, (int)(rows / 2), 0);
			break;
		default:
			pos = new Vector3 (-1, (int)(rows / 2), 0);
			break;
		}

		return Instantiate (localExit, pos, Quaternion.identity) as GameObject;
	}

	//TEMPLATE GENERATOR HERE
	//For now in code, we could put it as JSON file instead ~Darius

	MapTemplate GenerateTemplate (int templateNumber)
	{
		//Template creation here
		List<Vector3> newGridPositions = new List<Vector3> ();
		for (int x = 1; x < columns - 1; x++)
			for (int y = 1; y < rows - 1; y++) {
				newGridPositions.Add (new Vector3 (x, y, 0f));
			}

		Stack<int[]> objectAndPositon = new Stack<int[]> ();

		//TODO change to JSON
		//Example 0 - enemy, 1- npc, 2- wall, 3- hideObject, 4- pushableObject, 5- pickup
		switch (templateNumber) {
		case 0:
			{
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 2));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 3, 3));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 3));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 3, 2));
				break;
			}
		case 1:
			{
				objectAndPositon.Push (LayoutObjectAtPosition (2, 1, 1));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 1));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 2, 3));
				objectAndPositon.Push (LayoutObjectAtPosition (2, 3, 2));
				break;
			}
		}
		return new MapTemplate (templateNumber, objectAndPositon, gridPositions);
	}

	public void RandomizeTemplates ()
	{
		//TODO Make actual templates HERE ~Darius
		//Template pseudo randomization here
		//First two are tutorial
		stackOfTemplates.Push (GenerateTemplate (1));
		currentLevelTemplate++;
		stackOfTemplates.Push (GenerateTemplate (1));
		currentLevelTemplate++;
		stackOfTemplates.Push (GenerateTemplate (0));
		currentLevelTemplate++;
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
		int chosenElement = (x > 1 ? (x - 1) * rows : x - 1) + y - 1;
		int[] arrayOfObjects = new int[] { chosenElement, chosenObject };
		return arrayOfObjects;
	}

	void SpawnObjectsAtPosition (Stack<MapTemplate> tileStack)
	{
		MapTemplate temp = tileStack.Pop ();
		if (nextBoard) {
			gridPositions = temp.gridPositions;
		} else {
			gridPositions1 = temp.gridPositions;
		}


		while (temp.objectAndPosition.Count > 0) {
			int[] arr = temp.objectAndPosition.Pop ();
			Vector3 randomPosition = (nextBoard ? gridPositions1 : gridPositions) [arr [0]];

			GameObject[] tileArray = new GameObject[0];
			switch (arr [1]) {
			case 0:
				tileArray = enemyTiles;
				break;
			case 1:
				break;
			case 2:
				tileArray = wallTiles;
				break;
			case 5:
				tileArray = foodTiles;
				break;
			}
			GameObject tileChoice = tileArray [Random.Range (0, tileArray.Length)];

			GameObject instance = Instantiate (tileChoice, randomPosition, Quaternion.identity) as GameObject;
			if (instance.GetComponent<EnemyController> () != null) {
				instance.GetComponent<EnemyController> ().AddMeToList (nextBoard ? false : true);
			}
			instance.transform.SetParent (nextBoard ? itemHolder1 : itemHolder);
		}
	}

	//Represent template
	struct MapTemplate
	{
		public int number;
		public Stack<int[]> objectAndPosition;
		public List<Vector3> gridPositions;

		public MapTemplate (int number, Stack<int[]> objectAndPosition, List<Vector3> gridPositions)
		{
			this.number = number;
			this.objectAndPosition = objectAndPosition;
			this.gridPositions = gridPositions;
		}
	}
}
