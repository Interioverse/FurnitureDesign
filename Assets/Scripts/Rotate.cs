using UnityEngine;

namespace Assets
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public class Rotate : MonoBehaviour
    {
        public float rotationSpeed = 30.0f;
        public RotationAxis rotationAxis = RotationAxis.X;

        void Update()
        {
            // Determine which axis to rotate on based on the enum value.
            Vector3 axisVector = Vector3.zero;

            switch (rotationAxis)
            {
                case RotationAxis.X:
                    axisVector = Vector3.right;
                    break;
                case RotationAxis.Y:
                    axisVector = Vector3.up;
                    break;
                case RotationAxis.Z:
                    axisVector = Vector3.forward;
                    break;
            }

            // Rotate the object based on the selected axis.
            transform.Rotate(axisVector * rotationSpeed * Time.deltaTime);
        }
    }
}