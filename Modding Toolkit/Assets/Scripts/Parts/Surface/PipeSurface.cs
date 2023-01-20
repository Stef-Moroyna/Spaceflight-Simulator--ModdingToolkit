using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class PipeSurface : SurfaceData
    {
        // Variables
        [Space]
        [Required] public PipeData pipeData;
        public bool top = true, left = true, bottom = true, right = true;

        public override void Output()
        {
            if (!Application.isPlaying)
                pipeData.Output();
            if (pipeData.pipe == null)
                pipeData.Output();

            
            List<Vector2> vertices = pipeData.polygon.vertices.ToList();
            List<Vector2>[] sections =
            {
                left? vertices.GetRange(0, vertices.Count / 2) : null,
                top? vertices.GetRange(vertices.Count / 2 - 1, 2) : null,
                right? vertices.GetRange(vertices.Count / 2, vertices.Count / 2) : null,
                bottom? new List<Vector2> { vertices.Last(), vertices[0] } : null,
            };
        
            // Merges sections
            for (int i = 0; i < 4; i++)
            {
                List<Vector2> section = sections[i];
                List<Vector2> nextSection = sections[(i + 1) % 4];
                
                if (section != null && nextSection != null && section != nextSection)
                {
                    section.AddRange(nextSection.GetRange(1, nextSection.Count - 1));
                    sections[(i + 1) % 4] = section;
                }
            }

            // Adds sections
            List<Surfaces> output = new List<Surfaces>();
            foreach (List<Vector2> section in sections)
                if (section != null && section.Count > 0)
                {
                    output.Add(new Surfaces(section.ToArray(), false, pipeData.transform));
                    section.Clear();
                }

            SetData(output, output);
        }
    }
}