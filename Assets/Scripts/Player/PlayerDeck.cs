using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PlayerDeck
{
    public List<UnitData> currentDeck = new List<UnitData>();
    public int maxDeckSize = 5;

    public void Initialize(UnitData startingUnit)
    {
        currentDeck.Add(startingUnit);
        CardManager.OnDeckInitialized?.Invoke();
    }

    public void AddUnitToDeck(UnitData unit)
    {
        if (currentDeck.Count >= maxDeckSize) return;
        currentDeck.Add(unit);
        CardManager.OnCardAddedEvent?.Invoke();
    }

    public void RemoveUnitFromDeck(UnitData unit)
    {
        currentDeck.Remove(unit);
        CardManager.OnCardRemovedEvent?.Invoke();
    }

    public void ReplaceUnitFromDeck(UnitData oldUnit, UnitData newUnit)
    {
        int index = currentDeck.IndexOf(oldUnit);
        if (index == -1) return;
        currentDeck[index] = newUnit;
        CardManager.OnCardReplacedEvent?.Invoke();

    }
}