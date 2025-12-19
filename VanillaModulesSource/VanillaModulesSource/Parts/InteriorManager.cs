using SFS.Builds;
using SFS.Platform;
using SFS.Translations;
using SFS.UI;
using SFS.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SFS
{
    public class InteriorManager : MonoBehaviour
    {
        public static InteriorManager main;
        void Awake() => main = this;


        public Bool_Local interiorView;

        [BoxGroup("PC")] public ButtonPC toggleButton;

        [BoxGroup("Mobile")] public RectTransform interiorViewDisabled;
        [BoxGroup("Mobile")] public RectTransform interiorViewEnabled;

        void Start()
        {
            interiorView.OnChange += UpdateUI;
        }
        public void ToggleInteriorView()
        {
            if (BuildManager.main != null)
                Undo.main.RecordOtherStep(interiorView.Value? Undo.OtherAction.Type.DisableInteriorView : Undo.OtherAction.Type.EnableInteriorView);
            
            interiorView.Value = !interiorView.Value;
            MsgDrawer.main.Log(interiorView.Value ? Loc.main.Interior_View_On : Loc.main.Interior_View_Off);
        }
        void UpdateUI()
        {
            if (BuildManager.main == null)
                return;

            if (PlatformManager.current == PlatformType.Mobile)
            {
                interiorViewDisabled.gameObject.SetActive(!interiorView.Value);
                interiorViewEnabled.gameObject.SetActive(interiorView.Value);   
            }
            else
                toggleButton.SetSelected(!interiorView.Value);
        }
    }
}