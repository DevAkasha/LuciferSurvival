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

    private bool isFavorite = false;

    public bool Favorite
    {
        get { return isFavorite; }
    }

    public void TriggerFavoriteStatus()
    {
        isFavorite = !isFavorite;
        icon.sprite = isFavorite ? onSprite : offSprite;
    }
}
