using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace ZView
{
    public class ControlPanelView : MonoBehaviour
    {
        [SerializeField] Button axisButton;
        [SerializeField] Button rulerButton;

        ReactiveProperty<bool> axisVisibility = new ReactiveProperty<bool>(true);
        ReactiveProperty<bool> rulerVisibility = new ReactiveProperty<bool>(true);

        public IReadOnlyReactiveProperty<bool> Axis { get => axisVisibility; }
        public IReadOnlyReactiveProperty<bool> Ruler { get => rulerVisibility; }

        void Start()
        {
            axisButton.OnClickAsObservable().Subscribe(_ => {
                axisVisibility.Value = !axisVisibility.Value;
            }).AddTo(this);
            rulerButton.OnClickAsObservable().Subscribe(_ =>
            {
                rulerVisibility.Value = !rulerVisibility.Value;
            }).AddTo(this);
        }
    }
}
