using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	GameObject[] pauseObjects, finishObjects;
	public SoundManagerMenu soundManagerMenu;

	private string levelToLoad = "";

	Animator anim;

	GameObject canvas;
	GameObject fader;

	GameObject button1;
	GameObject button2;
	GameObject keySheet;

	//public bool isFinished;
	//public bool playerWon, enemyWon;
	// Use this for initialization
	void Start ()
	{
		canvas = GameObject.Find ("Canvas");
		fader = canvas.transform.Find ("ScreenFader").gameObject;
		anim = canvas.GetComponent<Animator> ();
		button1 = canvas.transform.Find ("Button1").gameObject;
		button2 = canvas.transform.Find ("Button2").gameObject;
		keySheet = canvas.transform.Find ("KeySheet").gameObject;
		keySheet.SetActive (false);
		Text text = keySheet.GetComponent<Text> ();
		text.text = "CONTROLS:\n\n\nSpace -> Interaction\n\nE -> Dialog\n\nF -> Flashlight \n\n\nPress RETURN\\ENTER key to go back...";
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Return) && button1.activeSelf) {
			if (button1.GetComponent<Button> ().colors.normalColor.a > 0.5f) {
				LoadLevel ("Does not matter");
			} else {
				showKeys ();
			}
		} else if (Input.GetKeyDown (KeyCode.Return) && !button1.activeSelf) {
			button1.SetActive (true);
			button2.SetActive (true);
			keySheet.SetActive (false);
		} else if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}

		if ((Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown (KeyCode.UpArrow)) && button1.activeSelf && button2.activeSelf) {
			Button button11 = button1.GetComponent<Button> ();
			Button button22 = button2.GetComponent<Button> ();
			Color colors1 = button1.GetComponent<Button> ().colors.normalColor;
			Color colors2 = button2.GetComponent<Button> ().colors.normalColor;
			ColorBlock colors11 = button1.GetComponent<Button> ().colors;
			ColorBlock colors22 = button2.GetComponent<Button> ().colors;
			if (button11.colors.normalColor.a > 0.5f) {
				colors1.a = 0.166f;
				colors11.normalColor = colors1;
			} else if (button11.colors.normalColor.a < 0.5f) {
				colors1.a = 1f;
				colors11.normalColor = colors1;
			}
			if (button22.colors.normalColor.a > 0.5f) {
				colors2.a = 0.166f;
				colors22.normalColor = colors2;
			} else if (button22.colors.normalColor.a < 0.5f) {
				colors2.a = 1f;
				colors22.normalColor = colors2;
			}

			button11.colors = colors11;
			button22.colors = colors22;
		}

		if (soundManagerMenu.musicEnded && levelToLoad != "") {
			SceneManager.LoadScene (1);
		}
	}

	public void showKeys ()
	{
		button1.SetActive (false);
		button2.SetActive (false);
		keySheet.SetActive (true);
	}

	public void LoadLevel (string level)
	{
		levelToLoad = level;
		fader.SetActive (true);
		anim.SetTrigger ("GameStart");	
		soundManagerMenu.SilenceMusic ();
	}

}
