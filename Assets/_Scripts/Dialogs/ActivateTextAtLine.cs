using UnityEngine;
using System.Collections;

public class ActivateTextAtLine : MonoBehaviour
{

	public TextAsset textFile;
	public int startLine = 0;
	public int endLine = 0;

	public TextBoxManager textBoxManager;

	private bool waitForAction = false;
	// Use this for initialization
	void Start ()
	{
		textBoxManager = FindObjectOfType<TextBoxManager> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (waitForAction == true) {
			if (Input.GetKeyDown (KeyCode.E)) {
				textBoxManager.ReloadScript (textFile);
				textBoxManager.currentLine = startLine;
				waitForAction = false;
			}
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			waitForAction = true;
			gameObject.transform.Find ("E").gameObject.SetActive (true);
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == "Player") {
			waitForAction = false;
			gameObject.transform.Find ("E").gameObject.SetActive (false);
		}
	}
}
