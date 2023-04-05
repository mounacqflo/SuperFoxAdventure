using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public string leaderboardName = "";
    public GameObject leaderboardEntryPrefab = null;
    private List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();
    public bool isFloatExpected = false;
    public bool isReversed = false;

    void OnEnable()
    {
        if (this.leaderboardEntryPrefab != null)
            this.leaderboardEntryPrefab.gameObject.SetActive(false);
        this.RefreshLeaderboard();
    }

    public void RefreshLeaderboard()
    {
        if (this.leaderboardEntryPrefab == null)
            return;
        int startPosition = 0;
        int maxEntries = 10;
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest
        {
            StartPosition = startPosition,
            MaxResultsCount = maxEntries,
            StatisticName = leaderboardName
        }, OnGetLeaderboardSuccess, OnGetLeaderboardError);
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        this.ClearExistingEntries();
        foreach (var dataEntry in result.Leaderboard)
        {
            string username = dataEntry.DisplayName;
            int statValue = dataEntry.StatValue;
            GameObject leaderboardEntryGameobjectCopy = GameObject.Instantiate(this.leaderboardEntryPrefab, this.leaderboardEntryPrefab.transform.parent);
            if (leaderboardEntryGameobjectCopy != null) {
                leaderboardEntryGameobjectCopy.gameObject.SetActive(true);
                LeaderboardEntry leaderboardEntry = leaderboardEntryGameobjectCopy.GetComponent<LeaderboardEntry>();
                if (leaderboardEntry != null)
                {
                    if (isReversed == true)
                        statValue *= -1;
                    leaderboardEntry.SetValue(username, (isFloatExpected == true ? ((float)statValue / 100.0f).ToString("0.00") : statValue.ToString()));
                    if (this.leaderboardEntries == null)
                        this.leaderboardEntries = new List<LeaderboardEntry>();
                    this.leaderboardEntries.Add(leaderboardEntry);
                }
                else
                {
                    GameObject.Destroy(leaderboardEntryGameobjectCopy);
                }
            }
        }
    }

    private void OnGetLeaderboardError(PlayFabError error)
    {
        Debug.LogError($"Leaderboard.OnGetLeaderboardError() - Error: {error.ErrorMessage}");
    }

    public void ClearExistingEntries()
    {
        if (this.leaderboardEntries != null) {
            while (this.leaderboardEntries.Count > 0)
            {
                if (this.leaderboardEntries[0] != null)
                {
                    GameObject.Destroy(this.leaderboardEntries[0].gameObject);
                }
                this.leaderboardEntries.RemoveAt(0);
            }
        }
    }
}