using UnityEngine;

namespace SFS.Parts.Modules
{
    public class OwnModule : MonoBehaviour
    {
        public PartPack pack;
        
        
        // Owned
       
        public virtual bool IsPartOwned => true;



        public enum PartPack
        {
            BigParts,
            SaturnV,
            Shuttle,
        }
    }   
    
    public enum OwnershipState
    {
        NotOwned,
        NotUnlocked,
        OwnedAndUnlocked,
    }
}