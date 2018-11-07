public abstract class Singleton<T>
{
	static T s_Instance;

	protected Singleton()
	{

	}

	public static T instance
	{
		get
		{
			if (s_Instance == null)
				s_Instance = System.Activator.CreateInstance<T>();
			return s_Instance;
		}
	}
}

