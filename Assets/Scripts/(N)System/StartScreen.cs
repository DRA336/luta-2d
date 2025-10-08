using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public GameSession session;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
    }

    void Update()
    {
        if (Input.anyKeyDown)
            session?.GoMainMenu();
    }
}
