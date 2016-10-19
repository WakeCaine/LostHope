using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour
{

	public float letterPause = 0.2f;
	//public AudioClip typeSound1;
	//public AudioClip typeSound2;

	string message;
	Text textComp;

	// Use this for initialization
	void Start ()
	{
		textComp = GameObject.Find ("MainText").GetComponent <Text> ();
		textComp.transform.position = new Vector3 (textComp.transform.position.x, textComp.transform.position.y + (Screen.height / 4), textComp.transform.position.z);
		message = textComp.text;
		textComp.text = "";
		StartCoroutine (TypeText ());
	}

	IEnumerator TypeText ()
	{
		foreach (char letter in message.ToCharArray()) {
			textComp.text += letter;
			//if (typeSound1 && typeSound2)
			//SoundManager.instance.RandomizeSfx (typeSound1, typeSound2);
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
	}
}
