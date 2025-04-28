using System;
using System.Collections.Generic;
using UnityEngine;

public class DirectEffect : BaseEffect
{
    public IReadOnlyDictionary<string, object> ValueChanges => valueChanges;

    private readonly Dictionary<string, object> valueChanges = new();

    public bool IsPercentage { get; private set; } = false;

    public DirectEffect(Enum id) : base(id)
    {
    }

    public DirectEffect AddChange<T>(string fieldName, T value)
    {
        valueChanges[fieldName] = value;
        return this;
    }

    public DirectEffect AsPercentage(bool isPercentage = true)
    {
        IsPercentage = isPercentage;
        return this;
    }

    public new DirectEffect When(Func<bool> condition)
    {
        base.When(condition);
        return this;
    }

    public override void ApplyTo(IBaseEntity target)
    {
        var model = target.GetBaseModel();
        var rxFields = model.GetAllRxFields();

        foreach (var (fieldName, value) in ValueChanges)
        {
            bool fieldFound = false;

            foreach (var field in rxFields)
            {
                if (field is IRxField rxField &&
                    string.Equals(rxField.FieldName, fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    ApplyValueChange(field, value);
                    fieldFound = true;
                    break;
                }
            }

            if (!fieldFound)
            {
                Debug.LogWarning($"[DirectEffect] Field '{fieldName}' not found on target {target}");
            }
        }
    }

    private void ApplyValueChange(RxBase field, object value)
    {
        try
        {
            if (field is RxVar<int> intVar)
            {
                int change = Convert.ToInt32(value);
                if (IsPercentage)
                {
                    int percentChange = (int)(intVar.Value * change / 100f);
                    intVar.SetValue(intVar.Value + percentChange);
                }
                else
                {
                    intVar.SetValue(intVar.Value + change);
                }
            }
            else if (field is RxVar<float> floatVar)
            {
                float change = Convert.ToSingle(value);
                if (IsPercentage)
                {
                    float percentChange = floatVar.Value * change / 100f;
                    floatVar.SetValue(floatVar.Value + percentChange);
                }
                else
                {
                    floatVar.SetValue(floatVar.Value + change);
                }
            }
            else if (field is RxVar<long> longVar)
            {
                long change = Convert.ToInt64(value);
                if (IsPercentage)
                {
                    long percentChange = (long)(longVar.Value * change / 100f);
                    longVar.SetValue(longVar.Value + percentChange);
                }
                else
                {
                    longVar.SetValue(longVar.Value + change);
                }
            }
            else if (field is RxVar<double> doubleVar)
            {
                double change = Convert.ToDouble(value);
                if (IsPercentage)
                {
                    double percentChange = doubleVar.Value * change / 100d;
                    doubleVar.SetValue(doubleVar.Value + percentChange);
                }
                else
                {
                    doubleVar.SetValue(doubleVar.Value + change);
                }
            }
            else if (field is RxVar<bool> boolVar)
            {
                bool newValue = Convert.ToBoolean(value);
                boolVar.SetValue(newValue);
            }
            else if (field is RxVar<string> stringVar)
            {
                string newValue = Convert.ToString(value);
                stringVar.SetValue(newValue);
            }
            else
            {
                Debug.LogWarning($"[DirectEffect] Unsupported field type: {field.GetType().Name}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DirectEffect] Error applying value change: {ex.Message}");
        }
    }

    public override void RemoveFrom(IBaseEntity target)
    {

    }
}