using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using InputObservable;

namespace ZView
{
    public class CameraController : MonoBehaviour
    {
        void Start()
        {
            var orig = (transform.position, transform.rotation);
            var context = this.DefaultInputContext();

            var origin = Vector3.zero;

            (context as IMouseWheelObservable)?.Wheel.Subscribe(e => {
                // transform.position += Vector3.forward * e.wheel;
                transform.Translate(transform.forward.normalized * e.wheel);
            }).AddTo(this);

            var io = context.GetObservable(0);
            io.ToEulerAngle(new Vector2 { x = -90, y = -90 })
                .Subscribe(rot => {
                    Debug.Log($"rotation = {rot}");
                    if(Input.GetKey(KeyCode.LeftShift)) {
                        transform.RotateAround(origin, Vector3.up, rot.y);
                        transform.RotateAround(origin, Vector3.right, rot.x);
                    }else
                    {
                        transform.Rotate(rot);
                    }
                }).AddTo(this);

            io.DoubleSequence(200).Subscribe(_ =>
            {
                Debug.Log("reset");
                transform.position = orig.position;
                transform.rotation = orig.rotation;
            }).AddTo(this);
        }
    }
}
