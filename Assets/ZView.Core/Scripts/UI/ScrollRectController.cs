using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZView
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectController : MonoBehaviour
    {
        ScrollRect scrollRect;

        void Start()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        public void Select(RectTransform child)
        {
            var viewPortHeight = scrollRect.viewport.rect.height;
            var contentPosY = scrollRect.content.anchoredPosition.y;

            var refY = -child.offsetMax.y;
            var visible = (contentPosY <= refY && (refY + child.rect.height) <= contentPosY + viewPortHeight);

            if (!visible)
            {
                var newY = refY + child.rect.height - viewPortHeight;
                scrollRect.content.anchoredPosition = new Vector2
                {
                    x = scrollRect.content.anchoredPosition.x,
                    y = (newY < 0) ? refY : newY
                };
                // Debug.Log($">> new contentPosY={scrollRect.content.anchoredPosition.y.ToString("F")} = {refY} + {child.rect.height} - {viewPortHeight}");
            }
        }
    }
}
