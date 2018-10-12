using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class inputController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private GameObject m_Player;
    [SerializeField]
    private GameObject m_Controller;
    [SerializeField]
    private float m_Speed = 0.5f;

    private Vector3 m_ControllerOriginPosition;
    private Vector3 m_Dir = Vector3.zero; // Z值默认为0，不参与计算
    private bool m_IsDrag = false;

    public float m_ControllerRidus = 50;

    private animationPlayer anim;                      

    private Quaternion tempQ = Quaternion.identity;

    void Awake()
    {
        m_ControllerOriginPosition = m_Controller.transform.position;
        anim = animationPlayer.getInstance();
    }

    void Update()
    {
        // 更新玩家位置
        if (m_IsDrag)
        {
            UpdatePlayerPositon(new Vector2(m_Dir.x, m_Dir.y));
            //tempQ = m_Player.transform.localRotation;
        }
            
    }


    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        m_IsDrag = false;
        anim.SetCondition(ANIMATION_TYPE.RUN);
        //anim.SetBool("Run", true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        // 左下角控制方向按钮
        Vector3 temp = new Vector3(eventData.position.x,eventData.position.y, 0);
        m_Dir = Vector3.Normalize(temp- m_ControllerOriginPosition);
        float distance = Vector3.Distance(temp,m_ControllerOriginPosition);
        if (distance > m_ControllerRidus)
            m_Controller.transform.position = m_ControllerOriginPosition + m_Dir * m_ControllerRidus;
        else m_Controller.transform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0);

        m_IsDrag = true;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        m_IsDrag = false;

        m_Controller.transform.position = m_ControllerOriginPosition;
        anim.SetCondition(ANIMATION_TYPE.STAY);
        //anim.SetBool("Run", false);

        //tempQ = m_Player.transform.localRotation;
    }


    void UpdatePlayerPositon(Vector2 delta)
    {
        delta *= m_Speed;
        Vector3 dir = new Vector3(delta.x, 0, delta.y);
        
        m_Player.transform.localPosition += dir;
        m_Player.transform.localRotation = Quaternion.LookRotation(dir.normalized);
        //m_Player.transform.localRotation = tempQ * Quaternion.LookRotation(dir.normalized);
        //m_Player.transform.localPosition  += m_Player.transform.forward*m_Speed;
    }
}
