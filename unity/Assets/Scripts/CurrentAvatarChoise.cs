using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentAvatarChoise : MonoBehaviour {

	// Use this for initialization
	public void DisplayAvatarChoises () {
		GameObject.FindGameObjectWithTag ("AvatarChoisePanel").GetComponent<Animator> ().SetTrigger ("AvatarChoisesPopUp");
	}

}
