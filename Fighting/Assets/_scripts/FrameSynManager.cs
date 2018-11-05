using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrameSyn
{
	public class FrameSynManager
	{
		private float m_FrameInterval = 50f/1000f;
		private int m_CurrentFrameIndex = 0;
		private int m_HanderNumPerFrame = -1; //-1：跟服务器差多少帧就处理多少帧，否则按num处理
		private Dictionary<int, FrameObjSBase> m_FrameObjSDic;
		private List<FrameSynAgentBase> m_FrameSynAgentList;
		private List<ControlAgentBase> m_ContolAgentList;
		private bool m_IsStart = false;
		private Dictionary<ControlAgentBase, FrameControlerDataBase> m_ControlFrameObjCDList;
		private int m_ForecastFrameNum = 0;
		private Dictionary<int, FrameObjSBase> m_ForecastFrameObjSDic;
		private float m_LastHandleFrameTime = 0;
		private float m_LastSendContorlTime = 0;
		private Action<Dictionary<ControlAgentBase, FrameControlerDataBase>> m_OnSendContorl;

		#region  属性
		public int ForecastFrameNum
		{
			set
			{
				m_ForecastFrameNum = value;
			}
			get
			{
				return m_ForecastFrameNum;
			}
		}

		public int HanderNumPerFrame
		{
			set
			{
				m_HanderNumPerFrame = value;
			}
			get
			{
				return m_HanderNumPerFrame;
			}
		}

		public Action<Dictionary<ControlAgentBase, FrameControlerDataBase>> OnSendContorl
		{
			set
			{
				m_OnSendContorl = value;
			}
			get
			{
				return m_OnSendContorl;
			}
		}
		#endregion

		/// <summary>
		/// 主循环，由外部驱动，比如Unity的Update
		/// </summary>
		public void Tick(int num = -1)
		{
			if (m_IsStart == false) return;
			HandleServerFrame();
			HandleSendControlFrame();
		}

		private void HandleServerFrame()
		{
			//获取服务器发过来的帧索引
			int currentForcastIndex = m_ForecastFrameObjSDic.Count;
			int serverFrameIndex = m_CurrentFrameIndex - currentForcastIndex;
			int frameCountOffset = m_FrameObjSDic.Count - serverFrameIndex;

			if (m_HanderNumPerFrame == -1)
				m_HanderNumPerFrame = frameCountOffset;
			else
				m_HanderNumPerFrame = m_HanderNumPerFrame <= frameCountOffset ? m_HanderNumPerFrame : 1;

			for (var i = 0; i < m_HanderNumPerFrame; ++i)
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

		/// <summary>
		/// 玩家的每次操作都调用这个创建一个操作帧,一个逻辑帧内只能有一次操作
		/// </summary>
		/// <param name="controlFrame"></param>
		public void CreateContorlFrame(ControlAgentBase controlAgent, FrameControlerDataBase controlFrame)
		{
			if (!m_ControlFrameObjCDList.ContainsKey(controlAgent))
				m_ControlFrameObjCDList.Add(controlAgent, controlFrame);
		}

		private void HandleSendControlFrame()
		{
			//检测频率为MAX频率
			if (m_ControlFrameObjCDList.Count > 0)
			{
				//发送频率为服务器频率
				if (Time.time - m_LastSendContorlTime >= m_FrameInterval)
				{
					m_LastSendContorlTime = Time.time;
					//外部操作，处理并发送操作帧
					if (m_OnSendContorl != null)
						m_OnSendContorl(m_ControlFrameObjCDList);

					//如果开启了预测则直接做预测帧
					if (m_ForecastFrameNum > 0)
					{
						//先获取最近一帧数据然后修改里面如果有的agent的操作为目前的操作，其他人的用默认
						int currentForcastIndex = m_ForecastFrameObjSDic.Count;
						int serverFrameIndex = m_CurrentFrameIndex - currentForcastIndex;
						FrameObjSBase forcastFrameObjS = m_FrameObjSDic[serverFrameIndex - 1];
						foreach (var controlFrameObj in m_ControlFrameObjCDList)
						{
							forcastFrameObjS.ControlerDataList.RemoveAll(conrolData =>controlFrameObj.Key.IsCanControl(conrolData));
							forcastFrameObjS.ControlerDataList.Add(controlFrameObj.Value);
						}

						m_ForecastFrameObjSDic.Add(m_CurrentFrameIndex, forcastFrameObjS);

						//作为预测的操作为了达到实时响应肯定要马上做帧处理
						m_LastHandleFrameTime -= m_FrameInterval;
					}
				}
				m_ControlFrameObjCDList.Clear();
			}
		}

		public void RegisterFrameSynAgent(FrameSynAgentBase frameAgent)
		{
			if (!m_FrameSynAgentList.Contains(frameAgent))
				m_FrameSynAgentList.Add(frameAgent);
		}

		public void RemoveFrameSynAgent(FrameSynAgentBase frameAgent)
		{
			if (m_FrameSynAgentList.Contains(frameAgent))
				m_FrameSynAgentList.Remove(frameAgent);
		}

		public void RegisterControlAgent(ControlAgentBase controlAgent)
		{
			if (!m_ContolAgentList.Contains(controlAgent))
				m_ContolAgentList.Add(controlAgent);
		}

		public void RemoveControlAgent(ControlAgentBase controlAgent)
		{
			if (m_ContolAgentList.Contains(controlAgent))
				m_ContolAgentList.Remove(controlAgent);
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
