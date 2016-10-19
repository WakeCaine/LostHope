using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{
	public GameObject textBox;
	public Text dialogText;
	public TextAsset textFile;
	public string[] textLines;

	public int currentLine = 0;
	public int endAtLine = 0;

	public Player player;

	private GameObject camera;

	private bool isZoomed = false;

	private bool isActive = false;
	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<Player> ();
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!isActive) {
			return;
		}

		if (isZoomed == true) {
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 40, Time.deltaTime * 5.0f);
		} else if (isZoomed == false) {
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 84.4f, Time.deltaTime * 5.0f);
		}

		dialogText.text = textLines [currentLine];

		if (Input.GetKeyDown (KeyCode.E) && currentLine < endAtLine) {
			currentLine += 1;
		} else if (Input.GetKeyDown (KeyCode.E) && currentLine >= endAtLine) {
			textBox.SetActive (false);
			DisableTextBox ();
			isZoomed = false;
		}
	}

	public void EnableTextBox ()
	{
		textBox.SetActive (true);
		player.canMove = false;
	}

	public void DisableTextBox ()
	{
		textBox.SetActive (false);
		player.canMove = true;
	}

	public void ReloadScript (TextAsset inputText)
	{
		if (inputText != null) {
			//camera.transform = 
			isZoomed = true;
			textLines = new string[1];
			textFile = inputText;
			textLines = (inputText.text.Split ('\n'));
			isActive = true;

			if (endAtLine == 0) {
				endAtLine = textLines.Length - 1;
			}

			if (isActive) {
				EnableTextBox ();
			} else {
				DisableTextBox ();
			}
		}


	}
}
