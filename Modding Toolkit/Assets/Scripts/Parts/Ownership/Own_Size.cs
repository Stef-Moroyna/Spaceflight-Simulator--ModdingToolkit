using System;
using System.Linq;
using SFS.Variables;

namespace SFS.Parts.Modules
{
    public class Own_Size : OwnModule
    {
        public AllowedSize[] allowed;
        
        public override bool IsPartOwned => base.IsPartOwned || !IsPremium;
        bool IsPremium => !allowed.All(a => a.size.Value < a.maxAllowedSize + 0.01f);
        
        [Serializable]
        public class AllowedSize
        {
            public Float_Reference size;
            public float maxAllowedSize;
        }
    }   
}