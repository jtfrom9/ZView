using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using InputObservable;

namespace ZView
{
    public class CameraController : MonoBehaviour
    {
        readonly Vector3 origin = Vector3.zero;

        void Update()
        {
            Debug.DrawLine(transform.position, origin, Color.red);
            Debug.DrawLine(transform.position, transform.position + transform.forward * 10.0f, Color.blue);
        }

        void Start()
        {
            var orig = (transform.position, transform.rotation);
            var context = this.DefaultInputContext();


            (context as IMouseWheelObservable)?.Wheel.Subscribe(e =>
            {
                var cur_dist = (origin - transform.position).magnitude;
                var vec = (origin - transform.position).normalized;
                var new_pos = transform.position + vec * e.wheel;
                var new_dist = (origin - new_pos).magnitude;

                if(e.wheel > 0) {
                    Debug.Log("go near");
                    if ((vec * e.wheel).magnitude > cur_dist)
                        return;
                    if (cur_dist < 0.5f || new_dist < 0.5f) return;
                } else
                {
                    Debug.Log("go far");
                    if (cur_dist > 10.0f || new_dist > 10.0f) return;
                }
                // transform.position += vec * e.wheel;
                transform.position = new_pos;
                Debug.Log($"new pos = {new_pos}, dist = {new_dist}");
            }).AddTo(this);

            var hratio = -90.0f / Screen.width;
            var vratio = -90.0f / Screen.height;

            var io = context.GetObservable(0);
            io.Difference().Subscribe(diff =>
            {
                var rot = diff.ToEulerAngle(hratio, vratio);
                Debug.Log($"rotation = {rot}, diff = {diff}");
                // if (!Input.GetKey(KeyCode.LeftShift))
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                {
                    transform.RotateAround(origin, Vector3.up, rot.y);
                }
                else
                {
                    var (x, z) = (transform.position.x, transform.position.z);
                    transform.RotateAround(origin, new Vector3(-z, origin.y, x), rot.x);
                    var e = transform.rotation.eulerAngles;
                    transform.rotation = Quaternion.Euler(e.x, e.y, 0);
                }
            }).AddTo(this);

            context.GetObservable(1).Difference().Subscribe(diff =>
            {
                var rot = diff.ToEulerAngle(hratio, vratio);
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rot);
            }).AddTo(this);

            io.DoubleSequence(200).Subscribe(_ =>
            {
                Debug.Log("reset");
                transform.position = orig.position;
                transform.rotation = orig.rotation;
            }).AddTo(this);

            context.GetObservable(2).Difference().Select(diff => diff * 0.01f).Subscribe(diff => {
                transform.Translate(-diff.x, -diff.y, 0);
            }).AddTo(this);
        }
    }
}
