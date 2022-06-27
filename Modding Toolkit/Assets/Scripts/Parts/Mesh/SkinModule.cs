using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SFS.Variables;
using System.Linq;
using System;



namespace SFS.Parts.Modules
{
    [HideMonoScript]
    public class SkinModule : MonoBehaviour, I_InitializePartModule
    {
        // Variables
        [BoxGroup("Ref", false), Required] public string skinTag;
        //
        public PipeMesh[] meshModules;
        ColorTexture[] defaultColors;
        ShapeTexture[] defaultShapes;
        //
        [BoxGroup("Tex", false)] public String_Reference colorTextureName;
        [BoxGroup("Tex", false)] public String_Reference shapeTextureName;
        public bool disableColorSelect, disableShapeSelect;
        
        int I_InitializePartModule.Priority => -10;
        void I_InitializePartModule.Initialize()
        {
            // Default skin to return to
            defaultColors = meshModules.Select(meshModule => meshModule.textures.texture.colorTexture).ToArray();
            defaultShapes = meshModules.Select(meshModule => meshModule.textures.texture.shapeTexture).ToArray();

            // When color or shape texture changes, regenerates mesh
            colorTextureName.OnChange += OnColorTextureChange;
            shapeTextureName.OnChange += OnShapeTextureChange;


        }



        // On change
        void OnColorTextureChange()
        {
            
        }
        void OnShapeTextureChange()
        {
            
        }

        // Available Textures
        public List<PartTexture> GetTextureOptions(int channel)
        {
            List<PartTexture> output = null;

            if (channel == 0)
                output = GetColorTextures().ConvertAll(colorTex => colorTex.colorTex);

            if (channel == 1)
                output = GetShapeTextures().ConvertAll(shapeTex => shapeTex.shapeTex);

            return output;
        }


        // Current Texture
        public PartTexture GetTexture(int channel)
        {
            if (channel == 0)
                return meshModules[0].textures.texture.colorTexture.colorTex;
            if (channel == 1)
                return meshModules[0].textures.texture.shapeTexture.shapeTex;

            throw new Exception();
        }
        public void SetTexture(int channel, PartTexture texture)
        {
            int index = GetTextureOptions(channel).IndexOf(texture);

            if (index == -1)
                return;

            if (channel == 0)
                colorTextureName.Value = GetColorTextures()[index].name;

            if (channel == 1)
                shapeTextureName.Value = GetShapeTextures()[index].name;
        }


        // Get
        List<ColorTexture> GetColorTextures()
        {
            return null;
        }
        List<ShapeTexture> GetShapeTextures()
        {

            return null;
        }
    }
}