using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCardDesign : MonoBehaviour {

	// Use this for initialization
	public void UpdateCard(){
		GameObject.FindGameObjectWithTag("MainMenuUIScript").GetComponent<MainMenuUI>().Updated_CardDesignChoise = int.Parse(this.gameObject.name);
		GameObject.FindGameObjectWithTag ("CardChoiseOutline").transform.position = this.transform.position;
	}
}
