using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshProfilePanel : MonoBehaviour {

	public void RefreshPanel(){
		GameObject.FindGameObjectWithTag ("MainMenuUIScript").GetComponent<MainMenuUI> ().RefreshProfile ();
	}
}
