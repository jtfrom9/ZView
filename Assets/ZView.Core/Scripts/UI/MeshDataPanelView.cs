using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace ZView
{
    public class MeshDataPanelView : MonoBehaviour, IMeshDataUIView
    {
        [SerializeField] Toggle visibleToggle = default;
        [SerializeField] Text text = default;
        [SerializeField] Button modifyPoseButton = default;
        [SerializeField] Button jumpButton = default;

        RectTransform rectTransform;
        MeshDataListPanelView parent;
        bool selected = false;

        Subject<bool> enabledSubject = new Subject<bool>();
        Subject<bool> selectedSubject = new Subject<bool>();
        Subject<Unit> modifySubject = new Subject<Unit>();
        Subject<Unit> jumpSubject = new Subject<Unit>();

        public string Key { get; private set; }
        IObservable<bool> IMeshDataUIView.Enabled { get => enabledSubject; }
        IObservable<bool> IMeshDataUIView.Selected { get => selectedSubject; }
        IObservable<Unit> IMeshDataUIView.OnModifyPose { get => modifySubject; }
        IObservable<Unit> IMeshDataUIView.OnJump { get => jumpSubject; }

        void onVisibleChanged(bool v)
        {
            enabledSubject.OnNext(v);
            modifyPoseButton.interactable = v;
            jumpButton.interactable = v;
        }

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            parent = transform.parent.gameObject.GetComponent<MeshDataListPanelView>();
            // bg = GetComponent<Image>();
            visibleToggle.OnValueChangedAsObservable()
                .Subscribe(v => {
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

        public void Initialize(string key, string name)
        {
            this.Key = key;
            gameObject.name = name;
            text.text = name;
        }
    }
}