using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该脚本挂在摄像机上
public class cameraFollow_Raycast :MonoBehaviour 
{
    [SerializeField]
    private float m_Offset_Forword_Distance;
    [SerializeField]
    private float m_Offset_Down_Distance;

    public Material m_Material_Transparent;
    private Material m_Material_Original;
    private GameObject m_Material_GameObj;
    private bool m_IsChangedMaterial = false;

    private Vector3 m_Offset_Original;
    private Vector3 m_Offset_Forword;
    private Vector3 m_Offset_Down;
    private Vector3 m_Offset;
    private float m_Distance_CameraToPlayer;
    private Ray m_Ray_CameraToPlayer;
    private RaycastHit m_CameraSheltedTestInfo; // 摄像机遮蔽检测信息
    private int m_Layer;

    [SerializeField]
    private Transform m_Player;

    void Awake()
    {
        // 求取同向原点
        m_Offset_Forword = m_Player.forward * m_Offset_Forword_Distance;
        m_Offset_Original = -m_Offset_Forword + m_Player.position;
        transform.position = new Vector3(m_Offset_Original.x, m_Offset_Down_Distance+m_Offset_Original.y , m_Offset_Original.z);

        m_Offset_Down = m_Offset_Original - transform.position;
        m_Offset_Forword = m_Player.position - m_Offset_Original;
        m_Offset = m_Offset_Down + m_Offset_Forword;
        m_Distance_CameraToPlayer = m_Offset.magnitude-0.5f;

        m_Layer = 1 << 8;
    }

    void Start()
    {

    }

    void Update()
    {
        m_Offset_Forword = m_Player.forward * m_Offset_Forword_Distance;
        m_Offset_Original = -m_Offset_Forword + m_Player.position;
        Vector3 cameraTargetPosition = new Vector3(m_Offset_Original.x, m_Offset_Down_Distance + m_Offset_Original.y, m_Offset_Original.z);
        transform.position = Vector3.Lerp(transform.position, cameraTargetPosition, Time.deltaTime * 4);
        transform.LookAt(m_Player);
        m_Ray_CameraToPlayer = new Ray(transform.position, m_Player.position - transform.position);

        if (IsBeenShelted())
        {
            if (!m_IsChangedMaterial)
            {
                m_Material_GameObj = m_CameraSheltedTestInfo.transform.gameObject;
                m_Material_Original = m_Material_GameObj.GetComponent<Renderer>().material;
                m_Material_GameObj.GetComponent<Renderer>().material = m_Material_Transparent;
                m_IsChangedMaterial = true;
            }
        }
        else if(m_IsChangedMaterial)
        {
            m_IsChangedMaterial = false;
            m_Material_GameObj.GetComponent<Renderer>().material = m_Material_Original;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (m_Player.position - transform.position).normalized * m_Distance_CameraToPlayer);
    }



    bool IsBeenShelted()
    {
        if (Physics.Raycast(m_Ray_CameraToPlayer,out m_CameraSheltedTestInfo, m_Distance_CameraToPlayer, m_Layer))
        {
            Debug.Log("Hitted!");
            return true;
        }
        return false;
    }

}
