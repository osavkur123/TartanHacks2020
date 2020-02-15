using Sanicball.Gameplay;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    public abstract class BallSpawner : MonoBehaviour
    {
        [SerializeField]
        private Ball ballPrefab = null;

        protected Ball SpawnBall(Vector3 position, Quaternion rotation, BallType ballType, ControlType ctrlType, int character, string nickname)
        {
            var ball = (Ball)Instantiate(ballPrefab, position, rotation);
            ball.Init(ballType, ctrlType, character, nickname);

            var ball1 = (Ball)Instantiate(ballPrefab, position + 0.5f * Vector3.one, rotation);
            ball1.Init(BallType.Follower, ControlType.None, character, nickname);
            var ball2 = (Ball)Instantiate(ballPrefab, position + 1f * Vector3.one, rotation);
            ball2.Init(BallType.Follower, ControlType.None, character, nickname);
            var ball3 = (Ball)Instantiate(ballPrefab, position + 1.5f * Vector3.one, rotation);
            ball3.Init(BallType.Follower, ControlType.None, character, nickname);

            return ball;
        }
    }
}