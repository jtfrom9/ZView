using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

namespace ZView
{
    public class DepthMeshItemListView : MonoBehaviour
    {
        [SerializeField] ScrollRectController scrollRectController;

        public void OnSelected(DepthMeshItemPanelView child, RectTransform rectTransform)
        {
            foreach (var c in GetComponentsInChildren<DepthMeshItemPanelView>())
            {
                if (c != child)
                    c.ClearSelected();
            }
            if(scrollRectController!=null) {
                scrollRectController.Select(rectTransform);
            }
        }
    }
}
