using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UpdateAvatar : MonoBehaviour { 

	public void UpdateAvatarChoise(){
		GameObject.FindGameObjectWithTag ("MainMenuUIScript").GetComponent<MainMenuUI> ().Updated_AvatarChoise = int.Parse(this.gameObject.name);
		GameObject.FindGameObjectWithTag ("CurrentAvatarChoise").GetComponent<Image> ().sprite = this.transform.parent.GetComponent<Image> ().sprite;
		GameObject.FindGameObjectWithTag ("AvatarChoisePanel").GetComponent<Animator> ().SetTrigger ("AvatarChoisesClose");
	}
}
