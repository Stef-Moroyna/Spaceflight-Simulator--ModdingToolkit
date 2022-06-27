using Sirenix.OdinInspector;
using UnityEngine;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class SliderModule : MonoBehaviour
    {
        [BoxGroup("Input", false)] public Vector2 start;
        [BoxGroup("Input", false)] public Direction direction;
        //
        [BoxGroup("State", false)] public MinMaxRange range;
        [BoxGroup("State", false)] public Float_Local sliderPosition;
        //
        [BoxGroup("Output", false)] public Float_Reference output;


        void Start()
        {
            sliderPosition.Filter = ValueFilter;

            range.min.OnChange += OnRangeChange;
            range.max.OnChange += OnRangeChange;

            sliderPosition.OnChange += UpdateOutput;
        }

        // Filter
        float ValueFilter(float oldValue, float newValue)
        {
            return Mathf.Clamp(newValue, range.min.Value, range.max.Value);
        }

        // On Change
        void OnRangeChange()
        {
            if (sliderPosition.Value < range.min.Value)
                sliderPosition.Value = range.min.Value;

            if (sliderPosition.Value > range.max.Value)
                sliderPosition.Value = range.max.Value;
        }
        void UpdateOutput()
        {
            output.Value = sliderPosition.Value;
        }


        void Update()
        {
            //Debug.DrawLine(transform.TransformPoint(start + DirectionToNormal(direction) * 0), transform.TransformPoint(start + DirectionToNormal(direction) * range.min.Value), Color.blue);

            Debug.DrawLine(transform.TransformPoint(start + DirectionToNormal(direction) * range.min.Value), transform.TransformPoint(start + DirectionToNormal(direction) * range.max.Value), Color.red);
            Debug.DrawRay(transform.TransformPoint(start + DirectionToNormal(direction) * range.min.Value) - Vector3.left * 0.5f, Vector3.left * 0.5f, Color.red);
            Debug.DrawRay(transform.TransformPoint(start + DirectionToNormal(direction) * range.max.Value) - Vector3.left * 0.5f, Vector3.left * 0.5f, Color.red);

            Debug.DrawRay(transform.TransformPoint(start + DirectionToNormal(direction) * sliderPosition.Value), new Vector2(0.3f, 0.2f), Color.green);
            Debug.DrawRay(transform.TransformPoint(start + DirectionToNormal(direction) * sliderPosition.Value), new Vector2(0.3f, -0.2f), Color.green);
        }
        
        Vector2 DirectionToNormal(Direction a)
        {
            if (a == Direction.Up)
                return Vector2.up;
            if (a == Direction.Down)
                return Vector2.down;
            if (a == Direction.Left)
                return Vector2.left;
            if (a == Direction.Right)
                return Vector2.right;

            throw new System.Exception();
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }
    }
}
