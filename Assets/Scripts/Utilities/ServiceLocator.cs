using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator instance;

    [SerializeField] private SoundManager soundManager;
    [SerializeField] private UIService uiService;

    public SoundManager SoundManager => soundManager;
    public UIService UIService => uiService;

    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
