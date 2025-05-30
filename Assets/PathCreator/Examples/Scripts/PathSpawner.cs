using UnityEngine;

namespace PathCreator.Examples.Scripts
{
    public class PathSpawner : MonoBehaviour
    {
        public Core.Runtime.Objects.PathCreator pathPrefab;
        public PathFollower followerPrefab;
        public Transform[] spawnPoints;

        private void Start()
        {
            foreach (var t in spawnPoints)
            {
                var path = Instantiate(pathPrefab, t.position, t.rotation);
                var follower = Instantiate(followerPrefab);
                follower.pathCreator = path;
                path.transform.parent = t;
            }
        }
    }
}