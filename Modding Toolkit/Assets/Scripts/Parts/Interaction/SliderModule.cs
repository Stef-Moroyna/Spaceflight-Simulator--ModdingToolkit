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
        
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }
    }
}
