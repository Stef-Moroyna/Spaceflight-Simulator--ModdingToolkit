using UnityEngine;
using SFS.Variables;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class SimplePipe : PipeData
    {
        public Composed_Float width_a;
        public Composed_Float width_b;
        public Composed_Float height_a;
        public Composed_Float height_b;
        
        public override void Output()
        {
            Pipe pipe = new Pipe();

            pipe.AddPoint(new Vector2(0, height_a.Value), Vector2.right * width_a.Value);
            pipe.AddPoint(new Vector2(0, height_b.Value), Vector2.right * width_b.Value);

            SetData(pipe);
        }
    }
}