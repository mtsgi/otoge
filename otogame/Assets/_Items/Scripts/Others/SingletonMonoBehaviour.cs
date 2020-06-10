using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour, new()
{
	private static T instance;
	public static T Instance 
	{
		get 
		{
			if (instance == null) 
			{
				instance = (T)FindObjectOfType (typeof(T));
				if (instance == null) 
				{
					Debug.LogWarning(typeof(T) + "is nothing! So Create It");
					var singleton = new GameObject
					{
						name = $"Singleton_{typeof(T)}"
					};

					instance = singleton.AddComponent<T>();
				}
			}
			return instance;
		}
	}
	
	protected void Awake()
	{
		CheckInstance();
	}
	
	protected bool CheckInstance()
	{

		if (this == Instance)
		{
			return true;
		}
		else
		{
			Destroy(this);
			return false;			
		}

	}

}