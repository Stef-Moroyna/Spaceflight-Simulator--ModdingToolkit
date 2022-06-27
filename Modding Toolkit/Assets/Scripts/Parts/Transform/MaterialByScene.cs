using SFS.World;
using SFS.Builds;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace SFS.Parts.Modules
{
    public class MaterialByScene : MonoBehaviour
    {
        public SceneType sceneType;
        [Required] public Material material;
        [Required] public Graphic graphic;
        
        void Start()
        {
            graphic.material = material;
        }
    }
    
    public enum SceneType
    {
        Build,
        World,
    }
}