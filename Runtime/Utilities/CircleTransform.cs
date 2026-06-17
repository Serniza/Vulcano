using UnityEngine;
using UnityEditor;

namespace SernizaGamesCore
{
	[DisallowMultipleComponent]

	[ExecuteInEditMode]
    public class CircleTransform : MonoBehaviour
    {
        #region Variables & Properties

        [Foldout("Variables & Properties")]
        [SerializeField] float radius;
        public float Radius
        {
            get => radius;
            set
            {
                radius = value;

                SetCoordinates((transform.parent == null) ? Vector2.zero : (Vector2)transform.parent.position);
            }
        }
        [Foldout("Variables & Properties")]
        [SerializeField][Range(0f, 360f, order = 2)] float angle;
        public float Angle
        {
            get => angle;
            set
            {
                angle = value;

                SetCoordinates((transform.parent == null) ? Vector2.zero : (Vector2)transform.parent.position);
            }
        }

        #endregion

        #if UNITY_EDITOR

        void OnValidate()
        {
            SetCoordinates((transform.parent == null) ? Vector2.zero : (Vector2)transform.parent.position);
        }

        #endif

        void Update()
        {
            if (transform.hasChanged)
            {
                float radius = GetRadius((transform.parent == null) ? Vector2.zero : (Vector2)transform.parent.position);

                if (radius != this.radius)
                    this.radius = radius;

                float angle = GetAngle((transform.parent == null) ? Vector2.zero : (Vector2)transform.parent.position);

                if (angle != this.angle)
                    this.angle = angle;
            }
        }

        public float GetRadius(Vector2 center)
        {
            float radius = Vector2.Distance(transform.position, center);

            return radius;
        }

        public float GetAngle(Vector2 center)
        {
            if (radius != 0f)
            {
                float radians = Mathf.Atan((transform.position.y - center.y) / (transform.position.x - center.x));

                float angle = 0f;

                if (transform.position.x >= center.x && transform.position.y >= center.y)
                    angle = (radians * 180f) / Mathf.PI;
                else if (transform.position.x < center.x)
                    angle = 180f + ((radians * 180f) / Mathf.PI);
                else
                    angle = 360f + ((radians * 180f) / Mathf.PI);

                return angle;
            }
            else
                return angle;
        }

        public static float GetAngle(Vector2 center, Vector2 point)
        {
            float radians = Mathf.Atan((point.y - center.y) / (point.x - center.x));

            float angle = 0f;

            if (point.x >= center.x && point.y >= center.y)
                angle = (radians * 180f) / Mathf.PI;
            else if (point.x < center.x)
                angle = 180f + ((radians * 180f) / Mathf.PI);
            else
                angle = 360f + ((radians * 180f) / Mathf.PI);

            return angle;
        }

        void SetCoordinates(Vector2 center)
        {
            float radians = (angle * Mathf.PI) / 180f;

            float x = radius * Mathf.Cos(radians) + center.x;
            float y = radius * Mathf.Sin(radians) + center.y;

            transform.position = new Vector3(x, y, transform.position.z);
        }

        #if UNITY_EDITOR

        void OnDrawGizmosSelected()
        {
            Handles.DrawWireDisc((transform.parent == null) ? Vector2.zero : (Vector2)transform.parent.position, Vector3.back, radius);

            Gizmos.DrawLine((transform.parent == null) ? Vector2.zero : (Vector2)transform.parent.position, transform.position);
        }

        #endif
    }
}