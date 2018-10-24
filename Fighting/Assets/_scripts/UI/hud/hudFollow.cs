using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hudFollow : MonoBehaviour {

    [SerializeField]
    private Transform m_Player;
    [SerializeField]
    private Transform m_PlayerOpposite;

    [SerializeField]
    private float m_HightOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = Camera.main.WorldToScreenPoint(m_Player.position + new Vector3(0, m_HightOffset, 0));
        this.transform.position = pos;
	}
}
