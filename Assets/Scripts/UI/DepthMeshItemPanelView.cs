using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace ZView
{
    public class DepthMeshItemPanelView : MonoBehaviour
    {
        [SerializeField] Toggle visibleToggle = default;
        [SerializeField] Text text = default;
        [SerializeField] Button modifyPoseButton = default;
        [SerializeField] Button jumpButton = default;

        RectTransform rectTransform;
        DepthMeshItemListView parent;
        bool selected = false;

        Subject<bool> enabledSubject = new Subject<bool>();
        Subject<bool> selectedSubject = new Subject<bool>();
        Subject<Unit> modifySubject = new Subject<Unit>();
        Subject<Unit> jumpSubject = new Subject<Unit>();

        public IObservable<bool> Enabled { get => enabledSubject; }
        public IObservable<bool> Selected { get => selectedSubject; }
        public IObservable<Unit> ModifyPose { get => modifySubject; }
        public IObservable<Unit> Jump { get => jumpSubject; }

        void onVisibleChanged(bool v)
        {
            enabledSubject.OnNext(v);
            modifyPoseButton.interactable = v;
            jumpButton.interactable = v;
        }

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            parent = transform.parent.gameObject.GetComponent<DepthMeshItemListView>();
            // bg = GetComponent<Image>();
            visibleToggle.OnValueChangedAsObservable()
                .Subscribe(v => {
                    Debug.Log("visible");
                    onVisibleChanged(v);
                    })
                .AddTo(this);
            modifyPoseButton.OnClickAsObservable()
                .Subscribe(_ => {
                    modifySubject.OnNext(Unit.Default);
                }).AddTo(this);
            jumpButton.OnClickAsObservable()
                .Subscribe(_ => {
                    jumpSubject.OnNext(Unit.Default);
                }).AddTo(this);
        }

        void OnDestroy()
        {
            selectedSubject.Dispose();
        }

        public void OnSelect()
        {
            Debug.Log($"OnSelect. {name}");
            if (!selected)
            {
                selected = true;
                Debug.Log($"Selected. {name}");
                parent.OnSelected(this, rectTransform);
                selectedSubject.OnNext(selected);
                // bg.color = Color.white;
            }
        }

        public void OnSubmit()
        {
            Debug.Log($"OnSubmit. {name}");
            visibleToggle.isOn = !visibleToggle.isOn;
            onVisibleChanged(visibleToggle.isOn);
        }

        public void ClearSelected()
        {
            if (selected)
            {
                Debug.Log($"Deselected. {name}");
                selected = false;
                selectedSubject.OnNext(selected);
                // bg.color = Color.gray;
            }
        }

        public void Initialize(string name)
        {
            gameObject.name = name;
            text.text = name;
        }
    }
}