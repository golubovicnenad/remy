using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTriggers : MonoBehaviour {
    public Animator ChatAnimator;
    public Animator ProfileAnimator;
    public Animator SocialAnimator;

	public void ClearChatTrigger()
    {
        ChatAnimator.ResetTrigger("ChatFlyIn");
    }

    public void ClearProfileTrigger()
    {
        ProfileAnimator.ResetTrigger("ProfilePanelPopUp");
    }

    public void ClearSocialTrigger()
    {
        SocialAnimator.ResetTrigger("SocialFlyIn");
    }
}
