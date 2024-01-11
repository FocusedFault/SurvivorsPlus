using UnityEngine;

namespace SurvivorsPlus.Railgunner
{
    public class PolarReversal : MonoBehaviour
    {
        public Vector3 initialVelocity;
        private Rigidbody rigidbody;
        private float reversalDelay = 0.5f;
        private float stopwatch = 0f;

        private void Start()
        {
            this.rigidbody = this.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            this.stopwatch += Time.fixedDeltaTime;
            if (this.stopwatch < this.reversalDelay)
                return;
            this.stopwatch = 0f;
            this.rigidbody.velocity = -initialVelocity;
            GameObject.Destroy(this);
        }

    }
}