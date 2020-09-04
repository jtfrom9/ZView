using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using Zenject;

namespace ZView
{
    public class DepthMeshItemListView : MonoBehaviour, IMeshDataListUIView
    {
        [SerializeField] ScrollRectController scrollRectController;

        [SerializeField] GameObject depthMeshItemPanelPrefab;

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

        IMeshDataUIView IMeshDataListUIView.Add(string key, string text)
        {
            var view = Instantiate(depthMeshItemPanelPrefab).GetComponent<DepthMeshItemPanelView>();
            view.Initialize(key, text);
            view.transform.SetParent(transform);
            return view;
        }
    }
}
