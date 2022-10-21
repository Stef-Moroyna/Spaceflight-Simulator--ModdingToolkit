using SFS.Variables;
using SFS.World;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class LES_Module : MonoBehaviour
    {
        public Bool_Reference autoDetach;
        
        public void Activate(UsePartData usePartData)
        {
        }

        bool Detach(UsePartData usePartData)
        {
            return true;
        }
    }   
}