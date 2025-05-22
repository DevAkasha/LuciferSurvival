using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackableUnitModel
{
    public UnitModel unitModel;
    public int count;

    public StackableUnitModel(UnitModel model)
    {
        unitModel = model;
        count = 1;
    }

    public void AddCount(int amount = 1)
    {
        count += amount;
    }

    public void RemoveCount(int amount = 1)
    {
        count -= amount;
        if (count < 0)
            count = 0;
    }
}