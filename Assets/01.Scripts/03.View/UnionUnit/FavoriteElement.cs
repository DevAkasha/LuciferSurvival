using UnityEngine;
using UnityEngine.UI;

public class FavoriteElement : MonoBehaviour
{
    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite onSprite;

    [SerializeField]
    private Sprite offSprite;

    private bool favoriteStatus = false;

    public bool Favorite
    {
        get { return favoriteStatus; }
    }

    public void TriggerFavoriteStatus()
    {
        favoriteStatus = !favoriteStatus;
        icon.sprite = favoriteStatus ? onSprite : offSprite;
    }
}
