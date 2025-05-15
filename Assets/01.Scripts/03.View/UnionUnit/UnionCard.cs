using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnionCard : MonoBehaviour
{
    [SerializeField]
    private FavoriteElement favoriteElement;

    [SerializeField]
    private Image unitImage;

    [SerializeField]
    private TextMeshProUGUI unitName;

    [SerializeField]
    private TextMeshProUGUI completionRateText;

    private float completionRate;

    private UnitModel unitModel;

    private UnionTableSO unionData;

    public void SetUnionModel(string rcode)
    {
        unionData = DataManager.Instance.GetData<UnionTableSO>(rcode);
        unitModel = new UnitModel(DataManager.Instance.GetData<UnitDataSO>(unionData.unitRcode));
    }

    public void SetDetailCard()
    {

    }

    public void SetCompletionRate()
    {
        if (completionRateText != null)
        {
            completionRateText.text = $"조합률 : {completionRate}%";
        }
    }

    public void SetUnitName(string name)
    {
        if (unitName != null)
        {
            unitName.text = name;
        }
    }

    public void SetUnitImage(Sprite sprite)
    {
        unitImage.sprite = sprite;
    }

    public void OnclickCard()
    {
        if (unionData != null)
        {
            UnionTableMain main = GetComponentInParent<UnionTableMain>();
            main.ShowTableDetail();
            main.SetTableDetail(unionData);
            main.SetUnitInfo(DataManager.Instance.GetData<UnitDataSO>(unionData.unitRcode));
        }
    }
}
