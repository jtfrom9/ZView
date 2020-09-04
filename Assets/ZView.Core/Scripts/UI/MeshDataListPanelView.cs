using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using Zenject;

namespace ZView
{
    public class MeshDataListPanelView : MonoBehaviour, IMeshDataListUIView
    {
        [SerializeField] ScrollRectController scrollRectController;

        [SerializeField] GameObject depthMeshItemPanelPrefab;

        public void OnSelected(MeshDataPanelView child, RectTransform rectTransform)
        {
            foreach (var c in GetComponentsInChildren<MeshDataPanelView>())
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
            var view = Instantiate(depthMeshItemPanelPrefab).GetComponent<MeshDataPanelView>();
            view.Initialize(key, text);
            view.transform.SetParent(transform, false);
            return view;
        }
    }
}
