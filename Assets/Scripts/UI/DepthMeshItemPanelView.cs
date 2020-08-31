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
        [SerializeField] Toggle visibleToggle;
        [SerializeField] Text text;
        [SerializeField] Button modifyPoseButton;
        [SerializeField] Button jumpButton;

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

        void Start()
        {
            parent = transform.parent.gameObject.GetComponent<DepthMeshItemListView>();
            visibleToggle.OnValueChangedAsObservable()
                .Subscribe(v => {
                    enabledSubject.OnNext(v);
                    modifyPoseButton.interactable = v;
                    jumpButton.interactable = v;
                }).AddTo(this);
            modifyPoseButton.OnClickAsObservable()
                .Subscribe(_ => {
                    modifySubject.OnNext(Unit.Default);
                }).AddTo(this);
            jumpButton.OnClickAsObservable()
                .Subscribe(_ => {
                    jumpSubject.OnNext(Unit.Default);
                }).AddTo(this);
        }

        public void OnSelected()
        {
            if (!selected)
            {
                selected = true;
                Debug.Log($"Selected. {name}");
                parent.OnSelected(this);
                selectedSubject.OnNext(selected);
            }
        }

        public void ClearSelected()
        {
            if (selected)
            {
                Debug.Log($"Deselected. {name}");
                selected = false;
                selectedSubject.OnNext(selected);
            }
        }

        void OnDestroy()
        {
            selectedSubject.Dispose();
        }

        public void Initialize(string name)
        {
            gameObject.name = name;
            text.text = name;
        }
    }
}