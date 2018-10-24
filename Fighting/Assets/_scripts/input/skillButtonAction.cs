using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skillButtonAction : MonoBehaviour {

    private animationPlayer anim;

    void Start () {
        anim = animationPlayer.getInstance();
	}
	
    public void OnAttackButtonClicked()
    {
        anim.SetCondition(ANIMATION_TYPE.ATTACK);
    }

    public void OnSkill_1_ButtonClicked()
    {

    }

    public void OnSkill_2_ButtonClicked()
    {

    }

    public void OnSkill_3_ButtonClicked()
    {

    }
}
