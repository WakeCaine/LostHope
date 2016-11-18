using UnityEngine;
using System.Collections;

public class PrototypeLoader : MonoBehaviour
{
	public GameObject gameManager;
	// Use this for initialization
	void Awake ()
	{
		if (GameplayManager.instance == null) {
			Instantiate (gameManager);
		}	
	}
}
