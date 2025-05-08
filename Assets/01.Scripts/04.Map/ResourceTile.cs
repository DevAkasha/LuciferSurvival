using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceTile : BaseInteractable
{
    public override void Interact(PlayerEntity player)
    {
        GatherTile();
    }

    public void GatherTile()
    {
        RewardManager.Instance.TryGatherResource();
    }
}
