using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

namespace ZView
{
    public class Ruler : MonoBehaviour
    {
        List<CircleInfo> circles = new List<CircleInfo>();

        void Start()
        {
            circles.Add(new CircleInfo
            {
                center = Vector3.up * 0.01f,
                radius = 1.0f,
                forward = Vector3.up,
                // fillColor = Color.white
                bordered = true,
                borderColor = Color.white,
                borderWidth = 0.02f,
            });
            circles.Add(new CircleInfo
            {
                center = Vector3.up * 0.01f,
                radius = 2.0f,
                forward = Vector3.up,
                // fillColor = Color.white
                bordered = true,
                borderColor = Color.white,
                borderWidth = 0.02f
            });
            circles.Add(new CircleInfo
            {
                center = Vector3.up * 0.01f,
                radius = 3.0f,
                forward = Vector3.up,
                // fillColor = Color.white
                bordered = true,
                borderColor = Color.white,
                borderWidth = 0.02f
            });
        }

        void Update()
        {
            foreach (var c in circles)
            {
                Circle.Draw(c);
            }
        }
    }
}
