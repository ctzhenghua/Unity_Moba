using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerFollow : MonoBehaviour {
    public float m_Offset_Forword_Distance;
    public float m_Offset_Up_Distance;

    private Vector3 m_Offset_Original;
    private Vector3 m_Offset_Forword;
    private Vector3 m_Offset_Up;
    private Vector3 m_Offset;


    public Transform m_Player;

    void Start()
    {
        // Transform.position 表示相机位置，因为该脚本挂在摄像机上
    }

    void Update()
    {
        // 求取同向原点
        m_Offset_Forword = m_Player.forward * m_Offset_Forword_Distance;
        m_Offset_Up = Vector3.up * m_Offset_Up_Distance;
        m_Offset = m_Offset_Up + m_Offset_Forword;


        //m_Offset_Original = -m_Offset_Forword - m_Player.position;

        transform.position = Vector3.Lerp(transform.position, m_Player.position - m_Offset, Time.deltaTime * 5);
        transform.LookAt(m_Player);
    }
}
