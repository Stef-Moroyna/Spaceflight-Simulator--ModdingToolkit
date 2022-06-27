namespace SFS.Parts.Modules
{
    public interface I_PartMenu
    {
        
    }

    public class PartDrawSettings
    {
        public bool showTitle;
        public bool build;
        public bool game;
        public bool showDescription;
        public bool canBoardWorld;
        public bool showPartShowcaseButton;

        public static readonly PartDrawSettings PickGridSettings = new PartDrawSettings(true, false, false, true, false);
        public static readonly PartDrawSettings BuildSettings = new PartDrawSettings(true, true, false, false, true);
        public static readonly PartDrawSettings WorldSettings = new PartDrawSettings(true, false, true, false, false);

        public PartDrawSettings(bool showTitle, bool build, bool game, bool showDescription, bool showPartShowcaseButton)
        {
            this.showTitle = showTitle;
            this.build = build;
            this.game = game;
            this.showDescription = showDescription;
            this.showPartShowcaseButton = showPartShowcaseButton;
        }
    }
}