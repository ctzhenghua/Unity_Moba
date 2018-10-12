using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ANIMATION_TYPE{
    RUN,
    STAY,
    ATTACK,
    DEAD
}
public class animationPlayer
{
    [SerializeField]
    private static Animator m_AnimationController;

    private static animationPlayer m_Instance = null;
    private animationPlayer()
    {
        m_AnimationController = GameObject.Find("Player_Chan").GetComponent<Animator> ();
    }

    public static animationPlayer getInstance()
    {
        if (m_Instance == null)
            m_Instance = new animationPlayer();

        return m_Instance;
    }

    public void SetCondition(ANIMATION_TYPE type)
    {
        switch(type)
        {
            case ANIMATION_TYPE.RUN:
                    m_AnimationController.SetBool("Run", true);
                break;
            case ANIMATION_TYPE.STAY:
                    m_AnimationController.SetBool("Run", false);
                break;
            case ANIMATION_TYPE.ATTACK:
                     m_AnimationController.SetBool("Spinkick", true);
                break;
            case ANIMATION_TYPE.DEAD:
                break;
        }
    }
}
