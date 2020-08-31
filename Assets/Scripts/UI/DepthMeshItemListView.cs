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
        public void OnSelected(DepthMeshItemPanelView child)
        {
            foreach (var c in GetComponentsInChildren<DepthMeshItemPanelView>())
            {
                if (c != child)
                    c.ClearSelected();
            }
        }
    }
}
