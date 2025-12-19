using SFS.Variables;
using UnityEngine;

namespace InterplanetaryModule
{
    public class GravityRingModule : MonoBehaviour
    {
        public float targetAngularSpeed = 18f;
        public float accelerationTime = 2f;

        float currentAngularSpeed;
        float angularVelocity;

        bool isRotating;
        bool stopping;

        float rotationY;

        public Bool_Reference counterclockwise;

        public void Toggle()
        {
            if (!isRotating && !stopping)
            {
                isRotating = true;
                stopping = false;
            }
            else if (isRotating)
            {
                isRotating = false;
                stopping = true;
            }
        }

        void Update()
        {
            float targetSpeed = 0f;

            if (isRotating)
            {
                targetSpeed = targetAngularSpeed;
            }
            else if (stopping)
            {
                float remainder = Mathf.Repeat(rotationY, 360f);
                if (remainder < 1f || remainder > 359f)
                {
                    currentAngularSpeed = 0f;
                    stopping = false;
                    rotationY = 0f;
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    return;
                }

                targetSpeed = targetAngularSpeed * Mathf.Clamp01(remainder / 180f);
            }

            currentAngularSpeed = Mathf.SmoothDamp(currentAngularSpeed, targetSpeed, ref angularVelocity, accelerationTime);

            float deltaAngle = currentAngularSpeed * Time.deltaTime;
            rotationY += deltaAngle;
            transform.localRotation = Quaternion.Euler(0f, (counterclockwise.Value ? -1 : 1) * rotationY, 0f);
        }
    }
}