using PathCreator.Core.Runtime.Objects;
using UnityEngine;

namespace PathCreator.Examples.Scripts
{
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(Core.Runtime.Objects.PathCreator))]
    public class GeneratePathExample : MonoBehaviour
    {
        public bool closedLoop = true;
        public Transform[] waypoints;

        private void Start()
        {
            if (waypoints.Length > 0)
            {
                // Create a new bezier path from the waypoints.
                var bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz);
                GetComponent<Core.Runtime.Objects.PathCreator>().bezierPath = bezierPath;
            }
        }
    }
}