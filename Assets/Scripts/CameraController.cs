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
            io.Difference().Subscribe(diff =>
            {
                var hratio = -90.0f / Screen.width;
                var vratio = -90.0f / Screen.height;
                var rot = diff.ToEulerAngle(hratio, vratio);
                Debug.Log($"rotation = {rot}");
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // var q = Quaternion.Euler(rot);
                    // float angle;
                    // Vector3 axis;
                    // q.ToAngleAxis(out angle, out axis);
                    // transform.RotateAround(origin, axis,angle);
                    // transform.RotateAround()
                    transform.RotateAround(origin, Vector3.right, rot.x);
                    transform.RotateAround(origin, Vector3.up, rot.y);
                }
                else
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

            // context.GetObservable(2).Difference().Subscribe(diff => {
            //     //ToEulerAngle(0.01f, 0.01f)
            //     Debug.Log($"diff={diff}");
            //     transform.Translate(-diff.y, diff.x, 0);
            // }).AddTo(this);
        }
    }
}
