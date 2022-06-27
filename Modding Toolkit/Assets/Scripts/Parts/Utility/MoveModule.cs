using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class MoveModule : MonoBehaviour, I_InitializePartModule
    {
        [InlineProperty] public Float_Reference time;
        [InlineProperty] public Float_Reference targetTime;

        public float animationTime; // Time for 1 unit of animation to pass
        public bool unscaledTime;
        
        [Space, InlineProperty]
        public MoveData[] animationElements = { };
        
        
        // Initialization
        int I_InitializePartModule.Priority => 0;
        void I_InitializePartModule.Initialize() => Start();
        bool initialized;
        void Start()
        {
            if (initialized)
                return;
            
            time.OnChange += ApplyAnimation;
            targetTime.OnChange += () => enabled = true;
            initialized = true;
        }

        
        void Update()
        {
            float deltaTime = (unscaledTime? Time.unscaledDeltaTime : Time.deltaTime);
            
            if (deltaTime == 0)
                return;

            float maxDelta = animationTime > 0? deltaTime / animationTime : 10000;
            time.Value = Mathf.MoveTowards(time.Value, targetTime.Value, maxDelta); // Moves towards target by delta

            if (time.Value == targetTime.Value)
                enabled = false; // Target reached, no need to update anymore
        }
        
        [Button] void ApplyAnimation()
        {
            foreach (MoveData a in animationElements)
                switch (a.type)
                {
                    case MoveData.Type.RotationZ:
                        a.transform.localEulerAngles = new Vector3(0, 0, a.X.Evaluate(time.Value - a.offset));
                        break;

                    case MoveData.Type.Position:
                        a.transform.localPosition = new Vector3(a.X.Evaluate(time.Value - a.offset), a.Y.Evaluate(time.Value - a.offset), 0);
                        break;

                    case MoveData.Type.Scale:
                        a.transform.localScale = new Vector3(a.X.Evaluate(time.Value - a.offset), a.Y.Evaluate(time.Value - a.offset), 0);
                        break;

                    case MoveData.Type.SpriteColor:
                        a.spriteRenderer.color = a.gradient.Evaluate(time.Value - a.offset);
                        break;

                    case MoveData.Type.ImageColor:
                        a.image.color = a.gradient.Evaluate(time.Value - a.offset);
                        break;

                    case MoveData.Type.Active:
                        float valueAtTime = a.X.Evaluate(time.Value - a.offset);
                        if (a.transform.gameObject.activeSelf != valueAtTime > 0)
                            a.transform.gameObject.SetActive(valueAtTime > 0);
                        AudioSource audioSource = a.transform.gameObject.GetComponent<AudioSource>();
                        if (audioSource && Application.isPlaying) ;
                            break;

                    case MoveData.Type.Inactive:
                        bool active = a.X.Evaluate(time.Value - a.offset) <= 0;
                        if (a.transform.gameObject.activeSelf != active)
                            a.transform.gameObject.SetActive(active);
                        break;

                    case MoveData.Type.SoundVolume:
                        if (Application.isPlaying) ;
                            break;

                    case MoveData.Type.AnchoredPosition:
                        (a.transform as RectTransform).anchoredPosition = new Vector3(a.X.Evaluate(time.Value - a.offset), a.Y.Evaluate(time.Value - a.offset), 0);
                        break;
                    
                    case MoveData.Type.RotationXY:
                        a.transform.localEulerAngles = new Vector3(a.X.Evaluate(time.Value - a.offset), a.Y.Evaluate(time.Value - a.offset), 0);
                        break;
                }
        }



        public void Toggle()
        {
            targetTime.Value = targetTime.Value != 0 ? 0 : 1;
        }
        public void Activate()
        {
            targetTime.Value = 1;
        }
    }

    [Serializable]
    public class MoveData
    {
        public Type type;
        public float offset;

        [ShowIf("ShowTransform")] public Transform transform;
        [ShowIf("ShowSpriteRenderer")] public SpriteRenderer spriteRenderer;
        [ShowIf("ShowImage")] public Image image;

        [ShowIf("ShowCurveX")] public AnimationCurve X = new AnimationCurve();
        [ShowIf("ShowCurveY")] public AnimationCurve Y = new AnimationCurve();

        [ShowIf("ShowGradient")] public Gradient gradient = new Gradient();
        [ShowIf("ShowAudio")] public AudioSource audioSource;


        bool ShowTransform() { return new [] { true, true, true, false, false, false, false, true, true, false, true, true }[(int)type]; }
        bool ShowSpriteRenderer() { return type == Type.SpriteColor; }
        bool ShowImage() { return type == Type.ImageColor; }

        bool ShowCurveX() { return new [] { true, true, true, true, true, false, false, true, true, true, true, true }[(int)type]; }
        bool ShowCurveY() { return new [] { false, true, true, true, true, false, false, false, false, false, true, true }[(int)type]; }

        bool ShowGradient() { return type == Type.SpriteColor || type == Type.ImageColor; }
        bool ShowAudio() { return type == Type.SoundVolume; }

        public enum Type
        {
            RotationZ,
            Scale,
            Position,
            CenterOfMass,
            CenterOfDrag,
            SpriteColor,
            ImageColor,
            Active,
            Inactive,
            SoundVolume,
            AnchoredPosition,
            RotationXY
        }
    }
}