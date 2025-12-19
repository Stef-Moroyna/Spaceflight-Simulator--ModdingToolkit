using UnityEngine;

namespace SFS.Parts.Modules
{
    public class OwnModule : MonoBehaviour
    {
        public PartPack pack;
        
        // Owned
        #if UNITY_STANDALONE
        public bool IsOwned => pack != PartPack.Full_Version || DevSettings.FullVersion;
        #else
        public bool IsOwned
        {
            get
            {
                switch (pack)
                {
                    case PartPack.BigParts: return Core.Purchases.Main.HasParts.Value;
                    case PartPack.Redstone_Atlas: return Core.Purchases.Main.HasRedstoneAtlas.Value;
                    
                    // case PartPack.SaturnV: return Core.Purchases.Main.HasSaturnV.Value;
                    // case PartPack.Shuttle: return Core.Purchases.Main.HasShuttle.Value;

                    default: return true;
                }
            }
        }
        #endif

        // Premium
        public virtual bool IsPremium => true;
        
        public enum PartPack
        {
            BigParts = 0,
            Redstone_Atlas = 1,
            
            //SaturnV,
            //Shuttle,
            
            Full_Version = 3, // For mac
        }
    }
    
    public enum OwnershipState
    {
        NotOwned,
        NotUnlocked,
        OwnedAndUnlocked,
    }
}