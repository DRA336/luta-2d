using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MatchUI : MonoBehaviour
{
    [Header("Referências")]
    public TMP_Text timerText;        // 00:60
    public TMP_Text announceText;     // "Fight!", "KO!", etc.
    public TMP_Text scoreText;        // "P1 1 - 0 P2"

    public void ShowTimer(float timeLeft)
    {
        if (!timerText) return;
        int t = Mathf.CeilToInt(timeLeft);
        timerText.text = t.ToString("00");
    }

    public void UpdateScore(int p1Wins, int p2Wins)
    {
        if (!scoreText) return;
        scoreText.text = $"P1 {p1Wins}  -  {p2Wins} P2";
    }

    public void ShowRoundIntro(string msg)
    {
        if (!announceText) return;
        announceText.gameObject.SetActive(true);
        announceText.text = msg;
    }

    public void ShowAnnounce(string msg)
    {
        if (!announceText) return;
        announceText.gameObject.SetActive(true);
        announceText.text = msg;
    }

    public void HideAnnounce()
    {
        if (!announceText) return;
        announceText.gameObject.SetActive(false);
    }
}
