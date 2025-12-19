using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using SFS.Variables;
using System.Linq;
using System;

#if !UNITY_STANDALONE
using SFS.Core;
#endif

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

            // Purchasing
            #if !UNITY_STANDALONE
            Purchases.Main.HasSkins.OnChange += OnColorTextureChange;
            Purchases.Main.HasSkins.OnChange += OnShapeTextureChange;
            #endif
        }

        #if !UNITY_STANDALONE
        void OnDestroy()
        {
            if (Base.sceneLoader.isUnloading)
                return;
            
            Purchases.Main.HasSkins.OnChange -= OnColorTextureChange;
            Purchases.Main.HasSkins.OnChange -= OnShapeTextureChange;
        }
        #endif

        // Sets variable
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
        
        // On change (applies variable)
        void OnColorTextureChange()
        {
            for (int i = 0; i < meshModules.Length; i++)
            {
                #if UNITY_STANDALONE
                bool hasSkins = DevSettings.FullVersion;
                bool hasRedstone = DevSettings.FullVersion;
                #else
                bool hasSkins = Purchases.Main.HasSkins.Value;
                bool hasRedstone = Purchases.Main.HasRedstoneAtlas.Value;
                #endif

                string key = colorTextureName.Value;
                Dictionary<string, ColorTexture> tex = Base.partsLoader.colorTextures;
                meshModules[i].SetColorTexture(hasSkins && tex.TryGetValue(key, out ColorTexture a) && (!a.pack_Redstone_Atlas || hasRedstone)? a : defaultColors[i]);
            }
        }
        void OnShapeTextureChange()
        {
            for (int i = 0; i < meshModules.Length; i++)
            {
                #if UNITY_STANDALONE
                bool hasSkins = DevSettings.FullVersion;
                bool hasRedstone = DevSettings.FullVersion;
                #else
                bool hasSkins = Purchases.Main.HasSkins.Value;
                bool hasRedstone = Purchases.Main.HasRedstoneAtlas.Value;
                #endif

                string key = shapeTextureName.Value;
                Dictionary<string, ShapeTexture> tex = Base.partsLoader.shapeTextures;
                meshModules[i].SetShapeTexture(hasSkins && tex.TryGetValue(key, out ShapeTexture a) && (!a.pack_Redstone_Atlas || hasRedstone)? a : defaultShapes[i]);
            }
        }
        
        // Current Texture
        public PartTexture GetTexture(int channel)
        {
            Textures.TextureSelector tex = meshModules[0].textures.texture;
            
            if (channel == 0)
                return tex.colorTexture.GetTexID();
            if (channel == 1)
                return tex.shapeTexture.GetTexID();

            throw new Exception();
        }
        
        // Available Textures
        public List<PartTexture> GetTextureOptions(int channel)
        {
            List<PartTexture> output = null;

            if (channel == 0)
                output = GetColorTextures().ConvertAll(colorTex => colorTex.GetTexID());
            if (channel == 1)
                output = GetShapeTextures().ConvertAll(shapeTex => shapeTex.GetTexID());

            return output;
        }
        List<ColorTexture> GetColorTextures()
        {
            List<ColorTexture> output = new List<ColorTexture>();

            if (!disableColorSelect)
                foreach (ColorTexture colorTexture in Base.partsLoader.colorTextures.Values)
                    if (colorTexture.tags.Contains(skinTag))
                        output.Add(colorTexture);

            #if !UNITY_STANDALONE
            output = output.Where(a => !a.pack_Redstone_Atlas || Purchases.Main.HasRedstoneAtlas.Value).ToList();
            #endif
            
            return output;
        }
        List<ShapeTexture> GetShapeTextures()
        {
            List<ShapeTexture> output = new List<ShapeTexture>();

            if (!disableShapeSelect)
                foreach (ShapeTexture shapeTexture in Base.partsLoader.shapeTextures.Values)
                    if (shapeTexture.tags.Contains(skinTag))
                        output.Add(shapeTexture);

            #if !UNITY_STANDALONE
            output = output.Where(a => !a.pack_Redstone_Atlas || Purchases.Main.HasRedstoneAtlas.Value).ToList();
            #endif
            
            return output;
        }
    }
}