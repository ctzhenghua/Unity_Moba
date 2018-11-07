using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OfflineServer
{
	public class OfflineServerManager : Singleton<OfflineServerManager>
	{
		public class FrameObj
		{
			private List<FrameControlData> m_ControlDataList;

			public int FrameIndex { protected set; get; }
			public int SendTime { protected set; get; }
			public List<FrameControlData> ControlDataList
			{
				get
				{
					if (m_ControlDataList == null)
						m_ControlDataList = new List<FrameControlData>();
					return m_ControlDataList;
				}
			}

			public void AddControlData(FrameControlData controlData)
			{
				m_ControlDataList.Add(controlData);
			}
		}

		public class FrameControlData
		{

		}

		private float m_FrameInterval = 50f / 1000f;
		private float m_LastSendFrameTime = 0;
		private bool m_IsStart = false;
		private int m_CurrentFrameIndex = 0;
		private List<FrameControlData> m_ControlDataList;
		private float m_NetConditionParam = 0f;

		#region 属性
		public bool IsStart
		{
			set
			{
				m_IsStart = value;
			}
			get
			{
				return m_IsStart;
			}
		}

		public float NetConditionParam
		{
			set
			{
				m_NetConditionParam = value;
			}
			get
			{
				return m_NetConditionParam;
			}
		}
		#endregion

		public void Tick()
		{
			if (m_IsStart == false) return;
			if (Time.time -  m_LastSendFrameTime >= m_FrameInterval)
			{
				m_LastSendFrameTime = Time.time;
				FrameObj frame = new FrameObj();
				foreach (var conrolData in m_ControlDataList)
				{
					frame.AddControlData(conrolData);
				}
				SendFrame(frame);
				m_ControlDataList.Clear();
			}
		}

		/// <summary>
		/// 发送数据并模拟网络状况
		/// </summary>
		/// <param name="frame"></param>
		public IEnumerator SendFrame(FrameObj frame)
		{
			float netConditionOffset = m_NetConditionParam * Mathf.Sin(Time.time);
			if (netConditionOffset >=0 )
				yield return new WaitForSeconds(netConditionOffset);

		}

		public void RiceveControl(FrameControlData controlData)
		{
			m_ControlDataList.Add(controlData);
		}
	}
}

