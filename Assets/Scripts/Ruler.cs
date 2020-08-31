using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

namespace ZView
{
    public class Ruler : MonoBehaviour
    {
        List<CircleInfo> circles = new List<CircleInfo>();
        List<LineInfo> lines = new List<LineInfo>();

        void Start()
        {
            var origin = Vector3.zero;
            float width = 0.02f;

            for (int i = 1; i <= 3; i++) {
                circles.Add(new CircleInfo
                {
                    // center = Vector3.up * 0.01f,
                    center = origin,
                    radius = 1.0f * i,
                    forward = Vector3.up,
                    // fillColor = Color.white
                    bordered = true,
                    borderColor = Color.white,
                    borderWidth = width,
                });
            }
            for (int i = 1; i <= 3; i++)
            {
                circles.Add(new CircleInfo
                {
                    center = origin,
                    radius = 1.0f * i,
                    forward = Vector3.forward,
                    // fillColor = Color.white
                    bordered = true,
                    borderColor = Color.white,
                    borderWidth = width,
                });
            }
            for (int i = 1; i <= 3; i++)
            {
                circles.Add(new CircleInfo
                {
                    center = origin,
                    radius = 1.0f * i,
                    forward = Vector3.right,
                    // fillColor = Color.white
                    bordered = true,
                    borderColor = Color.white,
                    borderWidth = width,
                });
            }
            lines.Add(new LineInfo { 
                startPos = origin - Vector3.right * 10.0f,
                endPos = origin + Vector3.right * 10.0f,
                forward = Vector3.forward,
                width = width,
                endArrow = true,
                fillColor = Color.red
            });
            lines.Add(new LineInfo
            {
                startPos = origin - Vector3.up * 10.0f,
                endPos = origin + Vector3.up * 10.0f,
                forward = Vector3.forward,
                width = width,
                endArrow = true,
                fillColor = Color.green
            });
            lines.Add(new LineInfo
            {
                startPos = origin - Vector3.forward * 10.0f,
                endPos = origin + Vector3.forward * 10.0f,
                forward = Vector3.up,
                width = width,
                endArrow = true,
                fillColor = Color.blue
            });
        }

        void Update()
        {
            foreach (var c in circles)
            {
                Circle.Draw(c);
            }
            foreach (var l in lines)
            {
                LineSegment.Draw(l);
            }
        }
    }
}
