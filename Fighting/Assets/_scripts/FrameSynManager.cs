using System.Collections.Generic;

public class FrameSynManager
{
	public abstract class IFrameObjS
	{

	}

	public abstract class IFrameObjC
	{

	}

	public abstract class IFrameSynAgent
	{

	}

	private int m_CurrentFrameIndex = 0;
	private Dictionary<int, IFrameObjS> m_FrameObjSDic;
	private List<IFrameSynAgent> m_FrameSynAgentList;
	private bool m_IsStart = false;
	private List<IFrameSynAgent> m_ControlFrameObjCDList;
	private int m_ForecastFrameNum = 0;
	private Dictionary<int, IFrameObjS> m_ForecastFrameObjSDic;
	private long m_LastFrameTime = 0;

	public void HandleFrame(int num)
	{

	}

	public void RegisterAgent(IFrameSynAgent agent)
	{

	}

	public void RemoveAgent(IFrameSynAgent agent)
	{

	}

	public void AddFrame(List<IFrameObjS> frameList)
	{

	}
}
