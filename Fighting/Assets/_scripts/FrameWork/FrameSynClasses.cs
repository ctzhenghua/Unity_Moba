using System;
using System.Collections.Generic;
namespace FrameSyn
{
	/// <summary>
	/// 服务器发过来的帧封装，需要外部继承
	/// </summary>
	public abstract class FrameObjSBase
	{
		private List<FrameControlDataBase> m_ControlDataList;

		public int FrameIndex { protected set; get; }
		public int ArriveTime { protected set; get; }
		public List<FrameControlDataBase> ControlDataList
		{
			get
			{
				if (m_ControlDataList == null)
					m_ControlDataList = new List<FrameControlDataBase>();
				return m_ControlDataList;
			}
		}

		abstract public bool CompareTo(FrameObjSBase frameObj);
		public void AddControlData(FrameControlDataBase controlData)
		{
			m_ControlDataList.Add(controlData);
		}
	}

	/// <summary>
	/// 每个玩家的操作数据封装，需要外部继承
	/// </summary>
	public abstract class FrameControlDataBase
	{
		
	}

	public abstract class FrameSynAgentBase
	{
		private List<Action<FrameObjSBase>> m_AuxiliaryFrameTickFuncList;
		public List<Action<FrameObjSBase>> AuxiliaryFrameTickFuncList
		{
			set
			{
				AuxiliaryFrameTickFuncList = value;
			}
			get
			{
				if (AuxiliaryFrameTickFuncList == null)
					AuxiliaryFrameTickFuncList = new List<Action<FrameObjSBase>>();
				return AuxiliaryFrameTickFuncList;
			}
		}
		abstract public void FrameTick(FrameObjSBase frameObj);
		abstract public void ReverToSnapShot(int frameIndex);
		abstract public void SaveToSnapShot(int frameIndex);
		abstract public void ClearSnapShot();
	}

	public interface ControlAgentBase
	{
		bool IsCanControl(FrameControlDataBase frameObj);
		void HandlerControlData(FrameControlDataBase controlData);
	}
}