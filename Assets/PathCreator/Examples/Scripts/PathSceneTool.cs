using System;
using PathCreator.Core.Runtime.Objects;
using UnityEngine;

namespace PathCreator.Examples.Scripts
{
    [ExecuteInEditMode]
    public abstract class PathSceneTool : MonoBehaviour
    {
        public Core.Runtime.Objects.PathCreator pathCreator;
        public bool autoUpdate = true;

        protected VertexPath path => pathCreator.path;


        protected virtual void OnDestroy()
        {
            if (onDestroyed != null) onDestroyed();
        }

        public event Action onDestroyed;

        public void TriggerUpdate()
        {
            PathUpdated();
        }

        protected abstract void PathUpdated();
    }
}