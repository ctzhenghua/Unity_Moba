using System;
using System.Collections.Generic;
using UnityEngine;

namespace OfflineServer
{
	public class OfflineServerManager
	{
		public class FrameObj
		{
			private List<FrameControlData> m_ControlerDataList;

			public int FrameIndex { protected set; get; }
			public int SendTime { protected set; get; }
			public List<FrameControlData> ControlerDataList { get { return m_ControlerDataList; } }

			public void AddControlerData(FrameControlData controlerData)
			{
				m_ControlerDataList.Add(controlerData);
			}
		}

		public class FrameControlData
		{

		}

		public class ControlAgent
		{

		}

		private float m_FrameInterval = 50f / 1000f;
		private float m_LastSendFrameTime = 0;
		private bool m_IsStart = false;
		private int m_CurrentFrameIndex = 0;
		private Dictionary<ControlAgent, FrameControlData> m_ControlDataList;

		public void Tick()
		{
			if (m_IsStart == false) return;
			if (Time.time -  m_LastSendFrameTime >= m_FrameInterval)
			{
				m_LastSendFrameTime = Time.time;
				FrameObj frame = new FrameObj();
				foreach (var conrolData in m_ControlDataList.Values)
				{
					frame.AddControlerData(controlerData);
				}
			}

			SendFrame(frame);
		}

		/// <summary>
		/// 发送数据并模拟网络状况
		/// </summary>
		/// <param name="frame"></param>
		public void SendFrame(FrameObj frame)
		{

		}

		public void RiceveControl(FrameControlData controlData)
		{

		}
	}
}

