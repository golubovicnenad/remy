using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterButton_StartAnimation : MonoBehaviour {
	public Login_Register LC;
	public void RegisterButton_Animation(){
		LC.isRegFormActive = true;
		GameObject.FindGameObjectWithTag ("LoginScreen").GetComponent<Animator> ().SetTrigger ("Registration_Form_Enable");
	}
}
