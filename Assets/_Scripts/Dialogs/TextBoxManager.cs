using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{
	[Tooltip ("TextBox for messages is required.")]
	public GameObject textBox;
	public Text dialogText;
	public TextAsset textFile;
	public string[] textLines;

	[Range (0f, 200f)]
	public float zoomNumber = 40f;

	[Range (0f, 2000f)]
	public float textBoxWidth = 210f;

	public int currentLine = 0;
	public int endAtLine = 0;

	public HeroPlayerController player;

	[Tooltip ("0 - Zoom + dialog box, 1 - Background distorted + characters avatars on screen")]
	public int dialogueMode = 0;

	private GameObject camera;
	private float currentZoom;

	private bool isZoomed = false;

	private bool isActive = false;
	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<HeroPlayerController> ();
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		currentZoom = (camera.GetComponent<Camera> () as Camera).fieldOfView;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!isActive) {
			return;
		}

		if (isZoomed == true) {
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, zoomNumber, Time.deltaTime * 5.0f);
		} else if (isZoomed == false) {
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, currentZoom, Time.deltaTime * 5.0f);
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
		Rect newRect = textBox.GetComponent<RectTransform> ().rect;
		textBox.GetComponent<RectTransform> ().sizeDelta = new Vector2 (textBoxWidth, newRect.height);
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
