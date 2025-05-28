using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Statics.Spikes
{
    public class Spikes : MonoBehaviour
    {
        public GameObject[] spikePrefabs;
        public int width = 10;
        public int seed;
        public float spacing = 1f;

        [ContextMenu("Generate Spikes")]
        public void GenerateSpikes()
        {
            var children = new List<GameObject>();
            foreach (Transform child in transform)
                children.Add(child.gameObject);

            foreach (var child in children)
                DestroyImmediate(child);

            var rand = new Random(seed);
            var halfWidth = width / 2;

            for (var i = 0; i < width; i++)
            {
                var prefabIndex = rand.Next(spikePrefabs.Length);
                var prefab = spikePrefabs[prefabIndex];
                var spike = Instantiate(prefab, transform);
                spike.transform.localPosition =
                    new Vector3(i * spacing - halfWidth, 0, 0) + prefab.transform.localPosition;
            }

            var boxCollider = GetComponent<BoxCollider2D>();

            if (boxCollider) boxCollider.size = new Vector2(width * spacing, boxCollider.size.y);
        }
    }
}