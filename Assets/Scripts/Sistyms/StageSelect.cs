using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public GameSession session;
    public Image stageImage;
    public Text stageName;

    int stageIndex = 0;

    void Awake()
    {
        if (!session) session = FindAnyObjectByType<GameSession>();
    }

    void OnEnable()
    {
        stageIndex = session.stageIndex;
        Refresh();
    }

    void Refresh()
    {
        if (session.stageThumbs.Length == 0) return;
        if (stageImage) stageImage.sprite = session.stageThumbs[stageIndex];
        if (stageName)  stageName.text   = session.stageNames[stageIndex];
    }

    public void Prev() { stageIndex = (stageIndex - 1 + session.stageNames.Length) % session.stageNames.Length; Refresh(); }
    public void Next() { stageIndex = (stageIndex + 1) % session.stageNames.Length; Refresh(); }

    public void Play()
    {
        session.SetStage(stageIndex);
        session.GoFight();
    }

    public void Back() => session.GoCharSelect();
}
