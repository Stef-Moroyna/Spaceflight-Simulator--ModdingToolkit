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
            bool active = (sceneType == SceneType.Build && BuildManager.main != null) || (sceneType == SceneType.World && GameManager.main != null);

            if (active)
                graphic.color = color;
        }
    }
}