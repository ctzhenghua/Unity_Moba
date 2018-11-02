using System.Collections.Generic;
using UnityEngine;

namespace FrameSyn
{
	public class FrameSynManager
	{
		private float m_FrameInterval = 50;
		private int m_CurrentFrameIndex = 0;
		private Dictionary<int, FrameObjSBase> m_FrameObjSDic;
		private List<FrameSynAgentBase> m_FrameSynAgentList;
		private List<ControlAgentBase> m_ContolAgentList;
		private bool m_IsStart = false;
		private List<FrameSynAgentBase> m_ControlFrameObjCDList;
		private int m_ForecastFrameNum = 0;
		private Dictionary<int, FrameObjSBase> m_ForecastFrameObjSDic;
		private float m_LastHandleFrameTime = 0;

		/// <summary>
		/// 处理逻辑帧，由外部驱动，比如Unity的Update
		/// </summary>
		/// <param name="num">-1：跟服务器差多少帧就处理多少帧，否则按num处理</param>
		public void HandleFrame(int num = -1)
		{
			//获取服务器发过来的帧索引
			int currentForcastIndex = m_ForecastFrameObjSDic.Count;
			int serverFrameIndex = m_CurrentFrameIndex - currentForcastIndex;
			int frameCountOffset = m_FrameObjSDic.Count - serverFrameIndex;

			if (num == -1)
				num = frameCountOffset;
			else
				num = num <= frameCountOffset ? num : 1;

			for (var i = 0; i < num; ++i)
			{
				currentForcastIndex = m_ForecastFrameObjSDic.Count;
				serverFrameIndex = m_CurrentFrameIndex - currentForcastIndex;
				frameCountOffset = m_FrameObjSDic.Count - serverFrameIndex;

				FrameObjSBase frameObjS;
				if (m_FrameObjSDic.TryGetValue(serverFrameIndex, out frameObjS))
				{
					if (m_ForecastFrameNum > 0)
					{
						FrameObjSBase forcastFrameObjS;
						if (m_ForecastFrameObjSDic.TryGetValue(serverFrameIndex, out forcastFrameObjS))
						{
							if (forcastFrameObjS.CompareTo(frameObjS))
							{
								m_ForecastFrameObjSDic.Remove(forcastFrameObjS.FrameIndex);
								//当做啥事都没发生，继续预测帧
							}
							else
							{
								//服务器帧跟预测帧不一致，则回退快照
								currentForcastIndex = 0;
								m_ForecastFrameObjSDic.Clear();
								m_CurrentFrameIndex = serverFrameIndex;
								m_FrameSynAgentList.ForEach(frameAgent =>
								{
									frameAgent.ReverToSnapShot(m_CurrentFrameIndex - 1);
									frameAgent.ClearSnapShot();
								});

								CommonHandleFrameImp(frameObjS);
							}
						}
						else
						{
							Debug.LogError("no forecast frame!");
						}
					}
					else
					{
						CommonHandleFrameImp(frameObjS);
					}
				}

				if (m_ForecastFrameNum > 0)
				{
					//每一个服务器帧间隔时间就做一次预测
					if (Time.time - m_LastHandleFrameTime > m_FrameInterval)
					{
						FrameObjSBase forcastFrameObjS;
						if (!m_ForecastFrameObjSDic.TryGetValue(m_CurrentFrameIndex, out forcastFrameObjS))
						{
							forcastFrameObjS = m_FrameObjSDic[serverFrameIndex - 1];
							m_ForecastFrameObjSDic.Add(m_CurrentFrameIndex, forcastFrameObjS);
						}
						m_FrameSynAgentList.ForEach(frameAgent =>
						{
							frameAgent.SaveToSnapShot(m_CurrentFrameIndex - 1);
						});
						CommonHandleFrameImp(forcastFrameObjS);
					}
				}
			}
		}

		private void CommonHandleFrameImp(FrameObjSBase frameObjS)
		{
			//先进行操作帧处理
			frameObjS.ControlerDataList.ForEach(controlerData =>
			{
				m_ContolAgentList.ForEach(agent =>
				{
					if (agent.IsCanControl(controlerData))
					{
						agent.HandlerControlData(controlerData);
					}
				});
			});

			//再tick
			m_FrameSynAgentList.ForEach(frameAgent =>
			{
				frameAgent.FrameTick(frameObjS);
			});
			m_LastHandleFrameTime = Time.time;
			m_CurrentFrameIndex++;
		}

		public void RegisterFrameSynAgent(FrameSynAgentBase frameAgent)
		{

		}

		public void RemoveFrameSynAgent(FrameSynAgentBase frameAgent)
		{

		}

		public void RegisterControlAgent(ControlAgentBase controlAgent)
		{

		}

		public void RemoveControlAgent(ControlAgentBase controlAgent)
		{

		}

		/// <summary>
		/// 从服务器获取帧数据，放在HandleFrame前
		/// </summary>
		/// <param name="frameList"></param>
		public void AddFrame(List<FrameObjSBase> frameList)
		{
			frameList.ForEach(frameData =>
			{
				if (m_FrameObjSDic.ContainsKey(frameData.FrameIndex))
					m_FrameObjSDic[frameData.FrameIndex] = frameData;
				else
					m_FrameObjSDic.Add(frameData.FrameIndex, frameData);
			});
		}
	}

}
