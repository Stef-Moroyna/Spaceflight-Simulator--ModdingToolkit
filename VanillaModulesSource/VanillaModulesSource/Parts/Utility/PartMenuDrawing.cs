namespace SFS.Parts.Modules
{
    public interface I_PartMenu
    {
        void Draw(StatsMenu drawer, PartDrawSettings settings);
    }

    public class PartDrawSettings
    {
        public bool showTitle;
        public bool build;
        public bool game;
        public bool showDescription;
        public bool canBoardWorld;

        public static readonly PartDrawSettings PickGridSettings = new PartDrawSettings(true, false, false, true);
        public static readonly PartDrawSettings BuildSettings = new PartDrawSettings(true, true, false, false);
        public static readonly PartDrawSettings WorldSettings = new PartDrawSettings(true, false, true, false);

        PartDrawSettings(bool showTitle, bool build, bool game, bool showDescription)
        {
            this.showTitle = showTitle;
            this.build = build;
            this.game = game;
            this.showDescription = showDescription;
        }
    }
}