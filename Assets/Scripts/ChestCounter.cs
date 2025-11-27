using UnityEngine;

public class ChestCounter : MonoBehaviour
{
    [Header("Chests to collect")]
    public PickupObject[] chests;     // drag the 3 chest loot objects here

    [Header("Ladder that should appear")]
    public GameObject ladder;         // drag the ladder here

    [Header("UI Message")]
    public GameObject ladderMessageUI;  // UI text object (disabled at start)
    public float messageDuration = 3f;  // how long to show the message

    private bool ladderShown = false;

    void Start()
    {
        // Make sure ladder starts hidden
        if (ladder != null)
            ladder.SetActive(false);

        // Make sure message starts hidden
        if (ladderMessageUI != null)
            ladderMessageUI.SetActive(false);
    }

    void Update()
    {
        if (ladderShown) return;
        if (chests == null || chests.Length == 0) return;

        // Check if all chests are picked up
        foreach (var chest in chests)
        {
            if (chest == null) return;           // safety
            if (!chest.HasBeenPickedUp) return; // still missing one
        }

        // If we got here, all chests are collected
        ladderShown = true;

        if (ladder != null)
            ladder.SetActive(true);  // reveal ladder

        if (ladderMessageUI != null)
        {
            ladderMessageUI.SetActive(true);     // show text
            StartCoroutine(HideMessageAfterDelay());
        }

        Debug.Log("All chests collected – ladder revealed!");
    }

    System.Collections.IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);

        if (ladderMessageUI != null)
            ladderMessageUI.SetActive(false);
    }
}
