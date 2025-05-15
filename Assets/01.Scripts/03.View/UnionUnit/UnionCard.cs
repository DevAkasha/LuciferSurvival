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

    private UnionTableSO unionData;

    public void SetUnionData(string rcode)
    {
        unionData = DataManager.Instance.GetData<UnionTableSO>(rcode);
    }

    //조합률 표시(유닛 이름과 위치가 겹쳐 둘중 하나만 사용할 필요가 있음)
    public void SetCompletionRate(int completionRate)
    {
        if (completionRateText != null)
        {
            completionRateText.text = $"조합률 : {completionRate}%";
        }
    }

    //유닛 이름 표시(조합률과 위치가 겹쳐 둘중 하나만 사용할 필요가 있음)
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
