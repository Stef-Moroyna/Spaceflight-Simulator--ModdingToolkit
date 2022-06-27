using SFS.World;
using SFS.Builds;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace SFS.Parts.Modules
{
    public class ColorByScene : MonoBehaviour
    {
        public SceneType sceneType;
        public Color color = Color.white;
        [Required] public Graphic graphic;
        
        void Start()
        {
            graphic.color = color;
        }
    }
}