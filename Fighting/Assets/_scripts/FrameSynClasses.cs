using System.Collections.Generic;
namespace FrameSyn
{
	public abstract class FrameObjSBase
	{
		private List<FrameControlerDataBase> m_ControlerDataList;

		public int FrameIndex { protected set; get; }
		public int ArriveTime { protected set; get; }
		public List<FrameControlerDataBase> ControlerDataList { get { return m_ControlerDataList; } }

		abstract public bool CompareTo(FrameObjSBase frameObj);
		public void AddControlerData(FrameControlerDataBase controlerData)
		{
			m_ControlerDataList.Add(controlerData);
		}
	}

	public abstract class FrameControlerDataBase
	{

	}

	public abstract class FrameObjCBase
	{

	}

	public abstract class FrameSynAgentBase
	{
		abstract public void FrameTick(FrameObjSBase frameObj);
		abstract public void ReverToSnapShot(int frameIndex);
		abstract public void SaveToSnapShot(int frameIndex);
		abstract public void ClearSnapShot();
	}

	public interface ControlAgentBase
	{
		bool IsCanControl(FrameControlerDataBase frameObj);
		void HandlerControlData(FrameControlerDataBase controlData);
	}
}