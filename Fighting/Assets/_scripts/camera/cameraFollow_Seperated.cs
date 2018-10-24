using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow_Seperated : MonoBehaviour
{
    public Material m_Material_Transparent;
    private Material m_Material_Original;
    private GameObject m_Material_GameObj;
    private bool m_IsChangedMaterial = false;

    private Vector3 m_Offset;
    private float m_Distance_CameraToPlayer;
    private Ray m_Ray_CameraToPlayer;
    private RaycastHit m_CameraSheltedTestInfo; // 摄像机遮蔽检测信息
    private int m_Layer;

    [SerializeField]
    private Transform m_Player;

    void Awake()
    {
        m_Offset = transform.position - m_Player.position;
        m_Distance_CameraToPlayer = m_Offset.magnitude-1f;
        m_Layer = 1 << 8;
    }

    void Start()
    {

    }

    void Update()
    {
        Vector3 cameraTargetPosition = m_Player.position + m_Offset;
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
        else if (m_IsChangedMaterial)
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
        if (Physics.Raycast(m_Ray_CameraToPlayer, out m_CameraSheltedTestInfo, m_Distance_CameraToPlayer, m_Layer))
        {
            //Debug.Log("Hitted!");
            return true;
        }
        return false;
    }
}
