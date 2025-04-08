using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow.Convenience;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using System.Diagnostics.Tracing;
using static UnityEngine.UI.Button;
using Ironcow;
using Ironcow.Resource;

namespace Ironcow.WorldObjectBase
{
    [Serializable]
    public class AnimationSpriteGroup
    {
        public string key;
        public List<Sprite> sprites;
        public bool isLoop = true;
        public ButtonClickedEvent endCallback;
        public string resourceType;
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class SpriteAnimation :
#if USE_AUTO_CACHING
    MonoAutoCaching
#else
        MonoBehaviour
#endif
    {
        [SerializeField] private List<AnimationSpriteGroup> animations;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] protected CircleCollider2D col;
        [SerializeField] private float moveToFrame = 20;
        public bool isLeft { get => !spriteRenderer.flipX; }
        private float nowFrame;
        private int nowIndex;
        private AnimationSpriteGroup nowAnimation;

        private void OnEnable()
        {
            
        }

        public void Init(int sortingOrder = 5)
        {
            bool isEmptySlot = animations.Find(obj => obj.sprites.Count == 0) != null;
            if (isEmptySlot)
            {
                var sprites = ResourceManager.instance.LoadAssets<Sprite>(name.Replace("(Clone)", ""), animations[0].resourceType);
                if (sprites != null && sprites.Count > 0)
                {
                    foreach (var ani in animations)
                    {
                        ani.sprites = sprites.FindAll(obj => obj.name.Contains(ani.key));
                    }
                }
            }

            if (animations.Count > 0)
            {
                ChangeAnimation(animations[0].key);
            }
            spriteRenderer.sortingOrder = sortingOrder;
        }

        public void Init(string rcode, string type, bool isLoop, bool isEndDestroy, int sortingOrder)
        {
            var sprites = ResourceManager.instance.LoadAssets<Sprite>(rcode.Replace("(Clone)", ""), type);
            var keys = new List<string>();
            if (sprites != null && sprites.Count > 0)
            {
                foreach (var sprite in sprites)
                {
                    var key = sprite.name.Split('_')[1];
                    if (!keys.Contains(key))
                        keys.Add(key);
                }
                foreach(var key in keys)
                {
                    animations.Add(new AnimationSpriteGroup() { key = key, isLoop = isLoop, resourceType = type });
                }
                foreach (var ani in animations)
                {
                    ani.sprites = sprites.FindAll(obj => obj.name.Contains(ani.key));
                }
            }

            if (animations.Count > 0)
            {
                ChangeAnimation(animations[0].key);
            }
            spriteRenderer.sortingOrder = sortingOrder;
        }

        private void Update()
        {
            if (nowAnimation == null) return;
            if (nowAnimation.sprites.Count == 0) return;
            nowFrame++;
            if (nowFrame > moveToFrame)
            {
                nowFrame = 0;
                nowIndex = Util.Next(nowIndex, 0, nowAnimation.sprites.Count, false);
                if (nowIndex == 0 && !nowAnimation.isLoop)
                {
                    nowAnimation.endCallback.Invoke();
                    nowAnimation = null;
                }
                else
                {
                    spriteRenderer.sprite = nowAnimation.sprites[nowIndex];
                }
                Rect croppedRect = new Rect(
                (spriteRenderer.sprite.textureRectOffset.x - spriteRenderer.sprite.rect.width / 2f) / spriteRenderer.sprite.pixelsPerUnit,
                (spriteRenderer.sprite.textureRectOffset.y - spriteRenderer.sprite.rect.height / 2f) / spriteRenderer.sprite.pixelsPerUnit,
                spriteRenderer.sprite.textureRect.width / spriteRenderer.sprite.pixelsPerUnit,
                spriteRenderer.sprite.textureRect.height / spriteRenderer.sprite.pixelsPerUnit);

                col.radius = (croppedRect.size.x + croppedRect.size.y) / 4f;
            }
        }

        public void ChangeAnimation(string key)
        {
            nowAnimation = animations.Find(obj => obj.key == key);
            if (nowAnimation != null && nowAnimation.sprites.Count > 0)
            {
                nowFrame = 0;
                nowIndex = 0;
                spriteRenderer.sprite = nowAnimation.sprites[nowIndex];
                if (nowAnimation.sprites.Count > 10)
                {
                    //moveToFrame = 20f - 20f / ((float)nowAnimation.sprites.Count - 10f);
                    moveToFrame = 20f / ((float)nowAnimation.sprites.Count - 10f) + 2;
                }
            }
        }

        public bool IsAnim(string key)
        {
            return nowAnimation.key == key;
        }

        public void SetFlip(bool isFlip)
        {
            spriteRenderer.flipX = isFlip;
        }
    }
}