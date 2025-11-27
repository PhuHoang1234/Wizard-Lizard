using UnityEngine;

public class DoorDisappear : MonoBehaviour
{
    // Drag the key pickup object (the one with PickupObject / PickedUpKey) here
    public PickupObject keyPickup;

    // Drag the chest loot object (ChestV2_Loot with PickupObject / PickedUpTreasure) here
    public PickupObject chestLootPickup;

    private bool opened = false;

    void Update()
    {
        if (opened) return;
        if (keyPickup == null || chestLootPickup == null) return;

        // When BOTH have been picked up
        if (keyPickup.HasBeenPickedUp && chestLootPickup.HasBeenPickedUp)
        {
            opened = true;
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        // Play door opening sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDoorOpen();
        }
        
        // Easiest version: just hide the door
        gameObject.SetActive(false);

        // If later you want an animation instead, you can replace this
        // with moving/rotating the door instead of disabling it.
    }
}
