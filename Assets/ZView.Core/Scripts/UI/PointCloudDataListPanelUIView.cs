using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using Zenject;

namespace ZView
{
    public class PointCloudDataListPanelUIView : MonoBehaviour, IPointCloudDataListUIView
    {
        [SerializeField] ScrollRectController scrollRectController;

        [SerializeField] GameObject depthMeshItemPanelPrefab;

        public void OnSelected(PointCloudDataPanelUIView child, RectTransform rectTransform)
        {
            foreach (var c in GetComponentsInChildren<PointCloudDataPanelUIView>())
            {
                if (c != child)
                    c.ClearSelected();
            }
            if(scrollRectController!=null) {
                scrollRectController.Select(rectTransform);
            }
        }

        IPointCloudDataUIView IPointCloudDataListUIView.Add(string key, string text)
        {
            var view = Instantiate(depthMeshItemPanelPrefab).GetComponent<PointCloudDataPanelUIView>();
            view.Initialize(key, text);
            view.transform.SetParent(transform, false);
            return view;
        }
    }
}
