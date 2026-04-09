public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance is null)
        {
            _instance = this as T;
        }
        if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
