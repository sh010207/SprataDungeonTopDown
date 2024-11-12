using System.Collections.Generic;
using UnityEngine;

public class PickupStatModifier : PickupItem
{
    [SerializeField] List<CharacterStat> statsModifier = new List<CharacterStat>();

    protected override void OnPickedUp(GameObject go)
    {
        CharacterStatHandler statHandler = go.GetComponent<CharacterStatHandler>();

        foreach(CharacterStat modifier in statsModifier)
        {
            statHandler.AddStatModifiers(modifier);
        }

        HealthSystem healthSystem = go.GetComponent<HealthSystem>();
        healthSystem.ChangeHealth(0);
    }
}
