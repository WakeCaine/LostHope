using UnityEngine;
using System.Collections;

public class ManageTextDialog : MonoBehaviour
{

	public TextAsset textFile;
	public int startLine = 0;
	public int endLine = 0;
	[Tooltip ("Tag of GameObject used to activate a dialog.")]
	public string triggerGameObjectTag = "Player";
	public string gameObjectIconName = "E";

	[Tooltip ("Needs GameObject with attached TextBoxManager script.")]
	public TextBoxManager textBoxManager;

	public bool waitForAction = false;
	//FIXIT:Change to just Collider
	private bool isBoxCollider2D = true;
	private bool isBoxCollider = true;

	// Use this for initialization
	void Start ()
	{
		textBoxManager = FindObjectOfType<TextBoxManager> ();
		if (gameObject.GetComponent<BoxCollider2D> () == null) {
			Debug.LogWarning ("No BoxCollider2D detected. Did you forget to attach it to GameObject?");
			isBoxCollider2D = false;
		} else if (gameObject.GetComponent<BoxCollider2D> () != null) {
			isBoxCollider2D = true;	
		}

		if (gameObject.GetComponent<BoxCollider> () == null) {
			if (isBoxCollider2D == false)
				Debug.LogWarning ("No BoxCollider detected. Did you forget to attach it to GameObject?");
			isBoxCollider = false;
		} else if (gameObject.GetComponent<BoxCollider> () != null) {
			isBoxCollider = true;	
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (waitForAction == true) {
			if (Input.GetKeyDown (KeyCode.E)) {
				waitForAction = false;
				textBoxManager.ReloadScript (textFile);
				textBoxManager.currentLine = startLine;
			}
		}
	}

	//Collider2D
	//------------------------------------------------------
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == triggerGameObjectTag) {
			waitForAction = true;
			Transform additionalIcon = gameObject.transform.Find (gameObjectIconName);
			if (additionalIcon != null)
				additionalIcon.gameObject.SetActive (true);
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{
		if (other.tag == triggerGameObjectTag) {
			waitForAction = false;
			Transform additionalIcon = gameObject.transform.Find (gameObjectIconName);
			if (additionalIcon != null)
				additionalIcon.gameObject.SetActive (false);
		}
	}
	//------------------------------------------------------

	//Collider
	//------------------------------------------------------
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == triggerGameObjectTag) {
			waitForAction = true;
			Transform additionalIcon = gameObject.transform.Find (gameObjectIconName);
			if (additionalIcon != null)
				additionalIcon.gameObject.SetActive (true);
		}
	}

	void OnTriggerExit (Collider other)
	{
		
		if (other.tag == triggerGameObjectTag) {
			waitForAction = false;
			Transform additionalIcon = gameObject.transform.Find (gameObjectIconName);
			if (additionalIcon != null)
				additionalIcon.gameObject.SetActive (false);
		}
	}
}
