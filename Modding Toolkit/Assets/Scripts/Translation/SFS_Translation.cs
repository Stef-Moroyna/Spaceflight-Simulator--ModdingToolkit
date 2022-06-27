using Beebyte.Obfuscator;
using SFS.Translations;
using System.Collections.Generic;
using UnityEngine.Scripting;
using F = SFS.Translations.Field;

// ReSharper disable InconsistentNaming
// /ReSharper disable UnusedMember.Global

//public F A => A(nameof(A), "");

namespace SFS
{
    [Preserve, Skip]
    public partial class SFS_Translation
    {
        public F None => A(nameof(None), "None");
        public F Font => A(nameof(Font), "normal");
        
        #region General
        [Group("General")]
        public F Cancel => A(nameof(Cancel), "Cancel");
        public F Close => A(nameof(Close), "Close");
        //
        [LocSpace]
        public F Open_Settings_Button => A(nameof(Open_Settings_Button), "Settings");
        public F Open_Cheats_Button => A(nameof(Open_Cheats_Button), "Cheats");
        public F Help => A(nameof(Help), "Help");
        //
        [LocSpace]
        public F Build_Rocket => A(nameof(Build_Rocket), "Build Rocket");
        public F Resume_Game => A(nameof(Resume_Game), "Resume Game");
        public F Return_To_Main_Menu => A(nameof(Return_To_Main_Menu), "Main Menu");
        public F Exit_To_Main_Menu => A(nameof(Exit_To_Main_Menu), "Exit To Main Menu");
        #endregion

        #region Main Menu
        [Group("Main Menu")]
        public F Play => A(nameof(Play), "Play");
        //
        [Documentation("Tutorial")]
        public F Video_Tutorials_OpenButton => A(nameof(Video_Tutorials_OpenButton), "Video Tutorials");
        public F Video_Orbit => A(nameof(Video_Orbit), "Orbit Tutorial");
        public F Video_Moon => A(nameof(Video_Moon), "Moon Tutorial");
        public F Video_Dock => A(nameof(Video_Dock), "Docking Tutorial");
        //
        [Documentation("Community")]
        public F Community_OpenButton => A(nameof(Community_OpenButton), "Community");
        public F Community_Youtube => A(nameof(Community_Youtube), "Youtube");
        public F Community_Discord => A(nameof(Community_Discord), "Discord");
        public F Community_Reddit => A(nameof(Community_Reddit), "Reddit");
        public F Community_Forums => A(nameof(Community_Forums), "Forums");
        //
        [Documentation("Credits")]
        public F Credits_OpenButton => A(nameof(Credits_OpenButton), "Credits");
        public F Credits_Text => A(nameof(Credits_Text), F.MultilineText(
            "Štefo Mai Morojna",
            "<Size=55> Designer - Programmer - Artist </size>",
            "",
            "Jordi van der Molen",
            "<Size=55> Programmer </size>",
            "",
            "Chris Christo",
            "<Size=55> Programmer </size>",
            "",
            "Davi Vasc",
            "<Size=55> Composer </size>",
            "",
            "Ashton Mills",
            "<size=55> Composer </size>"));
        //
        [Documentation("Update menu")]
        public F Update_Available => A(nameof(Update_Available), "A new update is available!\n\nCurrent version: %old%\nLatest version: %new%");
        public F Update_Confirm => A(nameof(Update_Confirm), "Update");
        //
        public F Update_Overview_Video_Available => A(nameof(Update_Overview_Video_Available), "Check out what's new this update?");
        public F Update_Overview_Video_Confirm => A(nameof(Update_Overview_Video_Confirm), "Open");
        //
        [Documentation("Rate menu")]
        public F Rate_Title => A(nameof(Rate_Title), F.MultilineText(
            "Would you like to rate or review the game?",
            "",
            "We deeply care about the quality of our game, your feedback helps us improve it",
            "",
            "Even after thousands of reviews, we still read a large number of them!"
        ));
        //
        public F Rate_Confirm => A(nameof(Rate_Confirm), "Rate");
        //
        [Documentation("Close menu")]
        public F Exit_Button => A(nameof(Exit_Button), "Exit");
        public F Close_Game => A(nameof(Close_Game), "Close game?");
        #endregion
        
        #region World Menu
        [Group("Worlds Menu")]
        public F Create_New_World_Button => A(nameof(Create_New_World_Button), "Create New World");
        public F World_Delete => A(nameof(World_Delete), "Delete world?");
        //
        [Documentation("Create menu")]
        public F Create_World_Title => A(nameof(Create_World_Title), "World Name");
        public F Default_World_Name => A(nameof(Default_World_Name), "My World");
        public F Select_Solar_System => A(nameof(Select_Solar_System), "Select world's solar system");
        public F Select_Solar_System__NotFound => A(nameof(Select_Solar_System__NotFound), F.MultilineText("Solar system could not be found:", "%system%", "", "Select a new solar system"));
        public F Default_Solar_System => A(nameof(Default_Solar_System), "Solar System (Default)");
        public F Custom_Solar_System => A(nameof(Custom_Solar_System), "%name% (Custom)");
        //
        [Documentation("World info")]
        public F Last_Played => A(nameof(Last_Played), "Last played: %value% ago");
        public F Just_Played => A(nameof(Just_Played), "Last played: A moment ago");
        public F Time_Played => A(nameof(Time_Played), "Playtime: %value%");
        [LocSpace]
        public F World_Difficulty_Name => A(nameof(World_Difficulty_Name), "Difficulty: %value%");
        public F Difficulty_Normal => A(nameof(Difficulty_Normal), "Normal");
        public F Difficulty_Hard => A(nameof(Difficulty_Hard), "Hard");
        public F Difficulty_Realistic => A(nameof(Difficulty_Realistic), "Realistic");
        [LocSpace]
        public F World_Mode_Name => A(nameof(World_Mode_Name), "Mode: %value%");
        public F Mode_Classic => A(nameof(Mode_Classic), "Classic");
        public F Mode_Career => A(nameof(Mode_Career), "Career");
        public F Mode_Sandbox => A(nameof(Mode_Sandbox), "Sandbox");
        #endregion

        #region World Create Menu
        [Group("World Create Menu")]
        public F World_Create_Title => A(nameof(World_Create_Title), "Create World");
        public F World_Name_Label => A(nameof(World_Name_Label), "World Name:");
        public F Solar_System_Label => A(nameof(Solar_System_Label), "Solar System:");
        public F Mode_Label => A(nameof(Mode_Label), "Mode:");
        public F Difficulty_Label => A(nameof(Difficulty_Label), "Difficulty:");
        [LocSpace]
        public F Difficulty_Scale_Stat => A(nameof(Difficulty_Scale_Stat), "Scale: 1:%scale%");
        public F Difficulty_Isp_Stat => A(nameof(Difficulty_Isp_Stat), "Specific Impulse: %value%x");
        public F Difficulty_Dry_Mass_Stat => A(nameof(Difficulty_Dry_Mass_Stat), "Tank Dry Mass: %value%x");
        public F Difficulty_Engine_Mass_Stat => A(nameof(Difficulty_Engine_Mass_Stat), "Engine Mass: %value%x");
        #endregion

        #region Saving
        [Group("Saving")]
        //
        [Documentation("Blueprint stuff")]
        public F Blueprints_Menu_Title => A(nameof(Blueprints_Menu_Title), "Blueprints");
        public F Unnamed_Blueprint => A(nameof(Unnamed_Blueprint), "Unnamed Blueprint");
        public F Save_Blueprint => A(nameof(Save_Blueprint), "Save Blueprint");
        public F Load_Blueprint => A(nameof(Load_Blueprint), "Load Blueprint");
        public F Cannot_Save_Empty_Build => A(nameof(Cannot_Save_Empty_Build), "Cannot save an empty blueprint");
        //
        [Documentation("Quicksave stuff")]
        public F Quicksaves_Menu_Title => A(nameof(Quicksaves_Menu_Title), "Quicksaves");
        public F Unnamed_Quicksave => A(nameof(Unnamed_Quicksave), "Unnamed Quicksave");
        public F Create_Quicksave => A(nameof(Create_Quicksave), "Create Quicksave");
        public F Load_Quicksave => A(nameof(Load_Quicksave), "Load Quicksave");
        //
        [Documentation("Save and load menus")]
        public F Save => A(nameof(Save), "Save");
        public F Load => A(nameof(Load), "Load");
        public F Import => A(nameof(Import), "Import");
        public F Delete => A(nameof(Delete), "Delete");
        public F Rename => A(nameof(Rename), "Rename");
        //
        [Documentation("In progress")]
        public F Saving_In_Progress => A(nameof(Saving_In_Progress), "Saving...");
        public F Loading_In_Progress => A(nameof(Loading_In_Progress), "Loading...");
        public F Importing_In_Progress => A(nameof(Importing_In_Progress), "Importing...");
        //
        [Documentation("filetype (injected)")]
        public F Blueprint => A(nameof(Blueprint), F.Subs("Blueprint", "blueprint", "blueprints"));
        public F Quicksave => A(nameof(Quicksave), F.Subs("Quicksave", "quicksave", "quicksaves"));
        //
        [Documentation("Ask overwrite menu")]
        public F File_Already_Exists => A(nameof(File_Already_Exists), "A %filetype{1}% with this name already exists");
        public F Overwrite_File => A(nameof(Overwrite_File), "Overwrite %filetype{1}%");
        public F New_File => A(nameof(New_File), "New %filetype{1}%");
        //
        [Documentation("Load failure")]
        public F Load_Failed => A(nameof(Load_Failed), "Could not load %filetype{1}% from %filepath%");
        //
        [LocSpace]
        public F Conversion_Success => A(nameof(Conversion_Success), 
            "Detected a 1.4 %filetype{1}% and converted it to %version% format");
        [LocSpace]
        public F Conversion_Failed => A(nameof(Conversion_Failed), F.MultilineText(
            "Detected a 1.4 %filetype{1}% and converted it to %version% format",
            "",
            "Conversion failed:",
            "%reason%"));
        #endregion

        #region Purchasing
        [Group("Purchasing")]
        public F Open_Shop_Menu => A(nameof(Open_Shop_Menu), "Expansions");
        public F Parts_Expansion => A(nameof(Parts_Expansion), "Parts Expansion");
        public F Skins_Expansion => A(nameof(Skins_Expansion), "Skins Expansion");
        public F Planets_Expansion => A(nameof(Planets_Expansion), "Planets Expansion");
        public F Cheats_Expansion => A(nameof(Cheats_Expansion), "Cheats");
        public F Infinite_Area_Expansion => A(nameof(Infinite_Area_Expansion), "Infinite Build Area");
        public F Builder_Bundle => A(nameof(Builder_Bundle), "Builder Bundle");
        public F Upgrade_To_Builder_Bundle => A(nameof(Upgrade_To_Builder_Bundle), "Upgrade To Builder Bundle");
        public F Sandbox_Bundle => A(nameof(Sandbox_Bundle), "Sandbox Bundle");
        public F Upgrade_To_Sandbox_Bundle => A(nameof(Upgrade_To_Sandbox_Bundle), "Upgrade To Sandbox Bundle");
        public F Full_Bundle => A(nameof(Full_Bundle), "Full Bundle");
        public F Upgrade_To_Full_Bundle => A(nameof(Upgrade_To_Full_Bundle), "Upgrade To Full Bundle");
        //
        [LocSpace]
        public F More_Parts => A(nameof(More_Parts), "More Parts...");
        public F More_Skins => A(nameof(More_Skins), "More Skins...");
        public F Cannot_Use_Cheats_In_Career => A(nameof(Cannot_Use_Cheats_In_Career), "Cheats can only be used in a sandbox mode world");
        public F Get_Infinite_Build_Area_Button => A(nameof(Get_Infinite_Build_Area_Button), "Get Infinite Build Area");
        public F Get_Cheats_Expansion_Button => A(nameof(Get_Cheats_Expansion_Button), "Get Cheats Expansion");
        //
        [LocSpace]
        public F Buy_Product => A(nameof(Buy_Product), "Buy %product% %price%");
        public F Discount_Reason_Upgrade => A(nameof(Discount_Reason_Upgrade), "Discounted because you already own other expansions");
        public F Purchase_Thanks_Msg => A(nameof(Purchase_Thanks_Msg), F.MultilineText(
            "Purchased: %product%",
            "",
            "Thanks for your purchase",
            "Now go and explore the stars!"
            ));
        public F Owned => A(nameof(Owned), "%product% (Owned)");
        //
        [LocSpace]
        public F Restore_Open => A(nameof(Restore_Open), "Restore");
        public F Restore_Text => A(nameof(Restore_Text), F.MultilineText(
            "Purchases on android are restored automatically on startup",
            "",
            "- Check that you are logged into the google play account you bought it with (Check that purchase appears in google play purchase history)",
            "",
            "- Check the purchase button, if price is not shown, it means the game failed to connect to the store. Check your internet connection and restart the game",
            "",
            "If none of the above work, contact me at: games.morojna@gmail.com",
            "",
            "You can also try waiting, the purchase sometimes restores itself after a few hours"));

        [Documentation("Parts Expansion")]
        public F PartsExpansion_Tanks => A(nameof(PartsExpansion_Tanks), "Large variety of fuel tanks!");
        public F PartsExpansion_Engines => A(nameof(PartsExpansion_Engines), "Heavy lift engines!");
        public F PartsExpansion_Parts => A(nameof(PartsExpansion_Parts), "Parts of all shapes and sizes!");
        public F PartsExpansion_Build => A(nameof(PartsExpansion_Build), "Large build space to bring" + "\n" + "your creations to life!");
        [Documentation("Skins Expansion")]
        public F SkinsExpansion_Tanks => A(nameof(SkinsExpansion_Tanks), "Paint your parts in a diverse variety of skins!");
        public F SkinsExpansion_Interstages => A(nameof(SkinsExpansion_Interstages), "Color everything from interstages");
        public F SkinsExpansion_Nosecones => A(nameof(SkinsExpansion_Nosecones), "To nosecones");
        public F SkinsExpansion_Fairings => A(nameof(SkinsExpansion_Fairings), "And even fairings");
        [Documentation("Planets Expansion")]
        public F PlanetsExpansion_Jupiter => A(nameof(PlanetsExpansion_Jupiter), "Explore Jupiter and its four moons!");
        public F PlanetsExpansion_Callisto => A(nameof(PlanetsExpansion_Callisto), "From the heavily cratered surface of Callisto!");
        public F PlanetsExpansion_Europa => A(nameof(PlanetsExpansion_Europa), "To the vast ice flats of Europa!");
        public F PlanetsExpansion_Conclusion => A(nameof(PlanetsExpansion_Conclusion), "Distant worlds are waiting for you" + "\n" + "to explore them!");
        #endregion

        #region Sharing
        [Group("Sharing")]
        // Upload
        public F Share_Button => A(nameof(Share_Button), "Share Blueprint");
        public F Upload_Blueprint_PC => A(nameof(Upload_Blueprint_PC), "Upload Blueprint");
        public F Download_Blueprint_PC => A(nameof(Download_Blueprint_PC), "Download Blueprint");
        public F Share_Button_PC => A(nameof(Share_Button_PC), "Share");
        public F Download_Confirm => A(nameof(Download_Confirm), "Download");
        public F URL_Field_TextBox => A(nameof(URL_Field_TextBox), "Blueprint URL");
        public F Empty_Upload => A(nameof(Empty_Upload), "Cannot upload empty blueprint");
        public F Uploading_Message => A(nameof(Uploading_Message), "Uploading...");
        public F Upload_Fail => A(nameof(Upload_Fail), "Failed to upload blueprint");
        public F Copied_URL_To_Clipboard => A(nameof(Copied_URL_To_Clipboard), "Copied blueprint URL to clipboard");
        
        // Download
        public F Sharing_Enter_Prompt => A(nameof(Sharing_Enter_Prompt), "Select which world you want blueprint to be loaded into");
        public F Confirm_Download_Button => A(nameof(Confirm_Download_Button), "Download Blueprint");
        public F Downloading_Message => A(nameof(Downloading_Message), "Downloading...");
        public F Download_Fail => A(nameof(Download_Fail), "Failed to download blueprint");
        public F URL_Invalid => A(nameof(URL_Invalid), "Invalid Blueprint URL");

        // Other
        public F Sharing_Connect_Fail => A(nameof(Sharing_Connect_Fail), "Could not connect to sharing servers");
        #endregion
        
        #region Settings
        [Group("Setting Titles")]
        [Unexported] public F General_Title => A(nameof(General_Title), "General Settings");
        [Unexported] public F Video_Title => A(nameof(Video_Title), "Video Settings");
        [Unexported] public F Audio_Title => A(nameof(Audio_Title), "Audio Settings");
        [Unexported] public F Keybindings_Title => A(nameof(Keybindings_Title), "Keybindings");
        
        [Group("Settings Mobile")]
        public F Music_Name => A(nameof(Music_Name), "Music");
        public F Sound_Name => A(nameof(Sound_Name), "Sound");
        public F Screen_Rotation_Name => A(nameof(Screen_Rotation_Name), "Screen Rotation");
        public F FPS_Name => A(nameof(FPS_Name), "Fps");
        public F Language_Name => A(nameof(Language_Name), "Language");
        public F Menu_Scale => A(nameof(Menu_Scale), "Interface Scale");
        public F Menu_Opacity => A(nameof(Menu_Opacity), "Interface Opacity");

        [Group("Settings PC")]
        [Unexported] public F Video_Resolution_Name => A(nameof(Video_Resolution_Name), "Resolution");
        [Unexported] public F Video_WindowMode_Name => A(nameof(Video_WindowMode_Name), "Window mode");
        [Unexported] public F Fullscreen_Exclusive => A(nameof(Fullscreen_Exclusive), "Fullscreen");
        [Unexported] public F Fullscreen_Borderless => A(nameof(Fullscreen_Borderless), "Borderless");
        [Unexported] public F Fullscreen_Windowed => A(nameof(Fullscreen_Windowed), "Windowed");
        [Unexported] public F Fps_Unlimited => A(nameof(Fps_Unlimited), "Unlimited");
        [Unexported] public F Video_VerticalSync_Name => A(nameof(Video_VerticalSync_Name), "Vertical Sync");
        
        
        [Group("Cheats")] // Cheats
        public F Infinite_Build_Area_Name => A(nameof(Infinite_Build_Area_Name), "Infinite Build Area");
        public F Part_Clipping_Name => A(nameof(Part_Clipping_Name), "Part Clipping");
        public F Infinite_Fuel_Name => A(nameof(Infinite_Fuel_Name), "Infinite Fuel");
        public F No_Atmospheric_Drag_Name => A(nameof(No_Atmospheric_Drag_Name), "No Atmospheric Drag");
        public F No_Collision_Damage_Name => A(nameof(No_Collision_Damage_Name), "No Collision Damage");
        public F No_Gravity_Name => A(nameof(No_Gravity_Name), "No Gravity");
        public F No_Heat_Damage_Name => A(nameof(No_Heat_Damage_Name), "No Heat Damage");
        public F No_Burn_Marks_Name => A(nameof(No_Burn_Marks_Name), "No Burn Marks");

        [Group("Active Cheats")] // Cheats list
        public F Using_Infinite_Build_Area => A(nameof(Using_Infinite_Build_Area), "Infinite Build Area");
        public F Using_Part_Clipping => A(nameof(Using_Part_Clipping), "Part Clipping");
        public F Using_Infinite_Fuel => A(nameof(Using_Infinite_Fuel), "Infinite Fuel");
        public F Using_No_Atmospheric_Drag => A(nameof(Using_No_Atmospheric_Drag), "No Atmospheric Drag");
        public F Using_No_Collision_Damage => A(nameof(Using_No_Collision_Damage), "No Collision Damage");
        public F Using_No_Gravity => A(nameof(Using_No_Gravity), "No Gravity");
        public F Using_No_Heat_Damage => A(nameof(Using_No_Heat_Damage), "No Heat Damage");
        public F Using_No_Burn_Marks => A(nameof(Using_No_Burn_Marks), "No Burn Marks");
        #endregion

        #region Tutorials
        [Group("Tutorials")]
        public F Tut_Drag_And_Drop => A(nameof(Tut_Drag_And_Drop), "Drag and drop parts" + "\n" + "to build your rocket");
        public F Tut_Part_Info => A(nameof(Tut_Part_Info), "Click to view" + "\n" + "part information");
        [LocSpace]
        public F Tut_Use_Part => A(nameof(Tut_Use_Part), "Click on parts to use them");
        public F Tut_Ignition => A(nameof(Tut_Ignition), "Ignition!");
        public F Tut_Throttle => A(nameof(Tut_Throttle), "Adjust throttle");
        #endregion

        #region Hub
        [Group("Hub")]
        public F Funds_Text => A(nameof(Funds_Text), "Funds: %funds%");
        public F Go_To_Space_Center => A(nameof(Go_To_Space_Center), "Space Center");
        public F Exit_To_Space_Center => A(nameof(Exit_To_Space_Center), "Exit To Space Center");
        public F Research_And_Development => A(nameof(Research_And_Development), "Research & Development %complete%/%total%");
        public F Achievements_Title => A(nameof(Achievements_Title), "Achievements:");
        public F Achievements_Button => A(nameof(Achievements_Button), "Achievements %complete%/%total%");
        #endregion
        
        #region Build
        [Group("Build")]
        public F Build_New_Rocket => A(nameof(Build_New_Rocket), "Build New Rocket");
        public F New => A(nameof(New), "New");
        public F Expand_Last_Rocket => A(nameof(Expand_Last_Rocket), "Continue Build");
        //
        [LocSpace]
        public F Symmetry_On => A(nameof(Symmetry_On), "Symmetry: On");
        public F Symmetry_Off => A(nameof(Symmetry_Off), "Symmetry: Off");
        [LocSpace]
        public F Interior_View_On => A(nameof(Interior_View_On), "Interior View: On");
        public F Interior_View_Off => A(nameof(Interior_View_Off), "Interior View: Off");
        //
        [LocSpace]
        public F Launch_Button => A(nameof(Launch_Button), "Launch");
        public F Move_Rocket_Button => A(nameof(Move_Rocket_Button), "Move Rocket");
        //
        [Documentation("Clear build area")]
        public F Clear_Warning => A(nameof(Clear_Warning), "Clear build area?");
        public F Clear_Confirm => A(nameof(Clear_Confirm), "Clear");
        //
        [Documentation("Launch warnings")]
        public F Warnings_Title => A(nameof(Warnings_Title), "WARNING:");
        public F Missing_Capsule => A(nameof(Missing_Capsule), "Your rocket has no capsule, making it uncontrollable");
        public F Missing_Parachute => A(nameof(Missing_Parachute), "Your rocket has no parachute");
        public F Missing_Heat_Shield => A(nameof(Missing_Heat_Shield), "Your rocket has no heat shield");
        
        //public Field Missing_Fuel => GetField(nameof(Missing_Fuel), "One or more of your rocket engines are not connected to a fuel source");
        //public Field Missing_Fuel_Popup => GetField(nameof(Missing_Fuel_Popup), "No fuel source");
        
        public F Too_Heavy => A(nameof(Too_Heavy), F.MultilineText("Your rocket is too heavy to launch", "%mass%", "%thrust%"));
        public F Launch_Anyway_Button => A(nameof(Launch_Anyway_Button), "Launch Anyway");
        //
        [Documentation("Example rockets")]
        public F Example_Rockets_OpenMenu => A(nameof(Example_Rockets_OpenMenu), "Example Rockets");
        public F Basic_Rocket => A(nameof(Basic_Rocket), "Basic Rocket");
        public F Stages => A(nameof(Stages), "Stages");
        public F Ideal_Stages => A(nameof(Ideal_Stages), "Ideal Stages");
        public F Lander => A(nameof(Lander), "Lander");
        #endregion

        #region Map
        [Group("Map")]
        public F Toggle_Map_Button => A(nameof(Toggle_Map_Button), "Map");
        //
        [Documentation("Shown in map when you escape a gravity field of a planet")]
        public F Escape => A(nameof(Escape), "Escape");
        //
        [Documentation("Shown in map when you encounter/enter a gravity field of a planet")]
        public F Encounter => A(nameof(Encounter), "Encounter");
        //
        [Documentation("Shown in map when you approach/encounter another rocket")]
        public F Rendezvous => A(nameof(Rendezvous), "Rendezvous");
        //
        [Documentation("Shows the transfer window to another planet")]
        public F Transfer => A(nameof(Transfer), "Transfer Window");
        #endregion

        #region Game
        [Group("Game")]
        public F Throttle_On => A(nameof(Throttle_On), "On");
        public F Throttle_Off => A(nameof(Throttle_Off), "Off");
        public F Ignition => A(nameof(Ignition), "IGNITION");
        public F RCS => A(nameof(RCS), "RCS");
        
        // Height/Velocity/Angle
        [Documentation("Game supports screen rotation, we split into 2 lines in vertical mode")]
        public F Height_Terrain_Vertical => A(nameof(Height_Terrain_Vertical), "Height (Terrain):\n\n%height%");
        public F Height_Vertical => A(nameof(Height_Vertical), "Height:\n\n%height%");
        public F Velocity_Vertical => A(nameof(Velocity_Vertical), "Velocity:\n\n%speed%");
        [Documentation("Best ascend flight angle")]
        public F Angle_Vertical => A(nameof(Angle_Vertical), "Angle:\n\n%angle% / %targetAngle%");
        //
        [LocSpace]
        public F Height_Terrain_Horizontal => A(nameof(Height_Terrain_Horizontal), "Height (Terrain): %height%");
        public F Height_Horizontal => A(nameof(Height_Horizontal), "Height: %height%");
        public F Velocity_Horizontal => A(nameof(Velocity_Horizontal), "Velocity: %speed%");
        [Documentation("Angle indicates the best angle/direction the player can rotate their rocket towards")]
        public F Angle_Horizontal => A(nameof(Angle_Horizontal), "Angle: %angle% / %targetAngle%");
        #endregion
        
        #region Game menus
        [Group("Game menus")]
        public F Restart_Mission => A(nameof(Restart_Mission), "Restart Mission");
        public F Recover_Rocket => A(nameof(Recover_Rocket), "Recover");
        public F Destroy_Rocket => A(nameof(Destroy_Rocket), "Destroy");
        //
        [Documentation("Restart menu")]
        public F Restart_Mission_To_Launch => A(nameof(Restart_Mission_To_Launch), "Revert To Launch");
        public F Restart_Mission_To_Launch_Warning => A(nameof(Restart_Mission_To_Launch_Warning), "Revert to launch?" + "\n\n" + "WARNING:\nThis will revert all progress since last launch");
        public F Restart_Mission_To_Build => A(nameof(Restart_Mission_To_Build), "Revert To Build");
        public F Restart_Mission_To_Build_Warning => A(nameof(Restart_Mission_To_Build_Warning), "Revert to build?" + "\n\n" + "WARNING:\nThis will revert all progress since last launch");
        //
        [Documentation("End mission menu")]
        public F End_Mission_Menu_Title => A(nameof(End_Mission_Menu_Title), "Mission Achievements:");
        public F End_Mission => A(nameof(End_Mission), "End Mission");
        //
        [Documentation("Clear space junk/debris")]
        public F Clear_Debris_Warning => A(nameof(Clear_Debris_Warning), "Clear debris?" + "\n\n" + "This will remove all uncontrollable rockets");
        public F Clear_Debris_Confirm => A(nameof(Clear_Debris_Confirm), "Clear Debris");
        //
        [Documentation("Select menu")]
        public F Navigate_To => A(nameof(Navigate_To), "Navigate To");
        public F End_Navigation => A(nameof(End_Navigation), "End Navigation");
        public F Focus => A(nameof(Focus), "Focus");
        public F Unfocus => A(nameof(Unfocus), "Unfocus");
        public F Switch_To => A(nameof(Switch_To), "Switch To");
        #endregion

        #region Rocket
        [Group("Rocket")]
        public F Default_Rocket_Name => A(nameof(Default_Rocket_Name), "Rocket");
        
        [Documentation("Informs the player that he cannot perform an action due to rocket having no control")]
        public F No_Control_Msg => A(nameof(No_Control_Msg), "No control");
        
        [Documentation("Failure menu")]
        public F Failure_Cause => A(nameof(Failure_Cause), "FAILURE CAUSE:");
        public F Failure_Crash_Into_Rocket => A(nameof(Failure_Crash_Into_Rocket), "Crashed into another rocket");
        public F Failure_Crash_Into_Terrain => A(nameof(Failure_Crash_Into_Terrain), "Crashed into the surface of %planet{1}%");
        public F Failure_Burn_Up => A(nameof(Failure_Burn_Up), "Burned up on reentry");
        #endregion

        #region Timewarp
        [Group("Timewarp")]
        public F Msg_Timewarp_Speed => A(nameof(Msg_Timewarp_Speed), "Time acceleration %speed%x");
        [LocSpace]
        public F Cannot_Timewarp_Below_Basic => A(nameof(Cannot_Timewarp_Below_Basic), "Cannot timewarp below %height%");
        public F Cannot_Timewarp_Below => A(nameof(Cannot_Timewarp_Below), "Cannot timewarp faster than %speed%x while below %height%");
        public F Cannot_Timewarp_While_Moving_On_Surface => A(nameof(Cannot_Timewarp_While_Moving_On_Surface), "Cannot timewarp faster than %speed%x while moving on the surface");
        public F Cannot_Timewarp_While_Accelerating => A(nameof(Cannot_Timewarp_While_Accelerating), "Cannot timewarp faster than %speed%x while under acceleration");
        public F Cannot_Use_Part_While_Timewarping => A(nameof(Cannot_Use_Part_While_Timewarping), "Cannot use %part{1}% while timewarping");
        public F Cannot_Turn_While_Timewarping => A(nameof(Cannot_Turn_While_Timewarping), "Cannot turn while timewarping");
        #endregion

        #region Units
        [Group("Units")]
        public F Thrust_To_Weight_Ratio => A(nameof(Thrust_To_Weight_Ratio), "Thrust / Weight: %value%");
        public F Mass => A(nameof(Mass), "Mass: %value%t");
        public F Thrust => A(nameof(Thrust), "Thrust: %value%t");
        public F Burn_Time => A(nameof(Burn_Time), "Burn Time: %value%s");
        public F Efficiency => A(nameof(Efficiency), "Efficiency: %value% Isp");
        //
        public F Mass_Unit => A(nameof(Mass_Unit), "t");
        public F Meter_Unit => A(nameof(Meter_Unit), "m");
        public F Km_Unit => A(nameof(Km_Unit), "km");
        public F Meter_Per_Second_Unit => A(nameof(Meter_Per_Second_Unit), "m/s");

        // PC unit titles
        [Unexported] public F Mass_Title => A(nameof(Mass_Title), "Mass");
        [Unexported] public F Height_Title => A(nameof(Height_Title), "Height");
        [Unexported] public F Thrust_Title => A(nameof(Thrust_Title), "Thrust");
        [Unexported] public F Thrust_To_Weight_Ratio_Title => A(nameof(Thrust_To_Weight_Ratio_Title), "Thrust / Weight");
        [Unexported] public F Part_Count_Title => A(nameof(Part_Count_Title), "Parts");
        #endregion

        #region Timestamp
        [Group("Timestamps")]
        public F Second_Short => A(nameof(Second_Short), "%value%s");
        public F Minute_Short => A(nameof(Minute_Short), "%value%m");
        public F Hour_Short => A(nameof(Hour_Short), "%value%h");
        public F Day_Short => A(nameof(Day_Short), "%value%d");
        #endregion

        #region Resource Types
        [Group("Resource Types", SubExport.Lowercase)]
        public F Solid_Fuel => A(nameof(Solid_Fuel), "Solid fuel");
        public F Liquid_Fuel => A(nameof(Liquid_Fuel), "Liquid fuel");
        [Unexported] public F Kerolox => A(nameof(Kerolox), "Kerolox");
        [Unexported] public F Hydrolox => A(nameof(Hydrolox), "Hydrolox");
        [Unexported] public F Methalox => A(nameof(Methalox), "Methalox");
        [Unexported] public F Hydrazine => A(nameof(Hydrazine), "Hydrazine");
        
        [Group("Resource Uses")]
        public F Resource_Bars_Title => A(nameof(Resource_Bars_Title), "%resource_name{0}%:");
        public F Info_Resource_Amount => A(nameof(Info_Resource_Amount), "%resource{0}%: %amount%");
        public F Msg_No_Resource_Source => A(nameof(Msg_No_Resource_Source), "No %resource{1}% source");
        public F Msg_No_Resource_Left => A(nameof(Msg_No_Resource_Left), "Out of %resource{1}%");
        #endregion
        
        #region Pick Categories
        [Group("Part Categories")]
        public F Basic_Parts => A(nameof(Basic_Parts), "Basics");
        public F Six_Wide_Parts => A(nameof(Six_Wide_Parts), "6 Wide");
        public F Eight_Wide_Parts => A(nameof(Eight_Wide_Parts), "8 Wide");
        public F Ten_Wide_Parts => A(nameof(Ten_Wide_Parts), "10 Wide");
        public F Twelve_Wide_Parts => A(nameof(Twelve_Wide_Parts), "12 Wide");
        public F Engine_Parts => A(nameof(Engine_Parts), "Engines");
        public F Aerodynamics_Parts => A(nameof(Aerodynamics_Parts), "Aerodynamics");
        public F Fairings_Parts => A(nameof(Fairings_Parts), "Fairings");
        public F Structural_Parts => A(nameof(Structural_Parts), "Structural");
        public F Other_Parts => A(nameof(Other_Parts), "Other");
        #endregion

        #region Part Names
        [Group("Part Names", SubExport.Lowercase)]
        public F Capsule_Name => A(nameof(Capsule_Name), "Capsule");
        public F Probe_Name => A(nameof(Probe_Name), "Probe");
        public F Parachute_Name => A(nameof(Parachute_Name), "Parachute");
        [LocSpace]
        public F Kolibri_Engine_Name => A(nameof(Kolibri_Engine_Name), "Kolibri Engine");
        public F Hawk_Engine_Name => A(nameof(Hawk_Engine_Name), "Hawk Engine");
        public F Valiant_Engine_Name => A(nameof(Valiant_Engine_Name), "Valiant Engine");
        public F Titan_Engine_Name => A(nameof(Titan_Engine_Name), "Titan Engine");
        public F Frontier_Engine_Name => A(nameof(Frontier_Engine_Name), "Frontier Engine");
        public F Peregrine_Engine_Name => A(nameof(Peregrine_Engine_Name), F.Text("Peregrine Engine"));
        public F Ion_Engine_Name => A(nameof(Ion_Engine_Name), "Ion Engine");
        public F RCS_Thruster_Name => A(nameof(RCS_Thruster_Name), "RCS Thruster");
        [LocSpace]
        public F Solid_Rocket_Booster => A(nameof(Solid_Rocket_Booster), "Solid Rocket Booster");
        [LocSpace]
        public F Fuel_Tank_Name => A(nameof(Fuel_Tank_Name), "Fuel Tank");
        public F Separator_Name => A(nameof(Separator_Name), "Separator");
        public F Side_Separator_Name => A(nameof(Side_Separator_Name), "Side Separator");
        public F Structural_Part_Name => A(nameof(Structural_Part_Name), "Structural Part");
        public F Landing_Leg_Name => A(nameof(Landing_Leg_Name), "Landing Leg");

        public F Aerodynamic_Nose_Cone_Name => A(nameof(Aerodynamic_Nose_Cone_Name), "Aerodynamic Nose Cone");
        public F Aerodynamic_Fuselage_Name => A(nameof(Aerodynamic_Fuselage_Name), "Aerodynamic Fuselage");
        public F Fairing_Name => A(nameof(Fairing_Name), "Fairing");

        public F Rover_Wheel_Name => A(nameof(Rover_Wheel_Name), "Rover Wheel");
        public F Docking_Port_Name => A(nameof(Docking_Port_Name), "Docking Port");

        public F Solar_Panel_Name => A(nameof(Solar_Panel_Name), "Solar Panel");
        public F Battery_Name => A(nameof(Battery_Name), "Battery");
        public F RTG_Name => A(nameof(RTG_Name), "RTG");

        public F Heat_Shield_Name => A(nameof(Heat_Shield_Name), "Heat Shield");
        public F Fuel_Pipe_Name => A(nameof(Fuel_Pipe_Name), "Fuel Pipe");
        #endregion
        #region Part Descriptions
        [Group("Part Descriptions")]
        // Control
        public F Capsule_Description => A(nameof(Capsule_Description), "A small capsule, carrying one astronaut");
        public F Probe_Description => A(nameof(Probe_Description), "An unmanned probe, used for one way missions");
        // Basics
        public F Parachute_Description => A(nameof(Parachute_Description), "A parachute used for landing");
        public F Fuel_Tank_Description => A(nameof(Fuel_Tank_Description), "A fuel tank carrying liquid fuel and liquid oxygen");
        public F Separator_Description => A(nameof(Separator_Description), "Vertical separator, used to detach empty stages");
        public F Side_Separator_Description => A(nameof(Side_Separator_Description), "Horizontal separator, used for detaching side boosters");
        public F Landing_Leg_Description => A(nameof(Landing_Leg_Description), "An extendable and retractable leg used for landing on the surface of moons and planets");
        public F Structural_Part_Description => A(nameof(Structural_Part_Description), "A light and strong structural part");
        // Engines
        public F Hawk_Engine_Description => A(nameof(Hawk_Engine_Description), "A high thrust - lower efficiency engine, normally used in the first stage of a rocket");
        public F Titan_Engine_Description => A(nameof(Titan_Engine_Description), "A high thrust - lower efficiency engine, normally used in the first stage of a rocket");
        public F Valiant_Engine_Description => A(nameof(Valiant_Engine_Description), "High efficiency, low thrust. Used in space when high thrust isn't a priority");
        public F Frontier_Engine_Description => A(nameof(Frontier_Engine_Description), "High efficiency, low thrust. Used in space when high thrust isn't a priority");
        public F Kolibri_Engine_Description => A(nameof(Kolibri_Engine_Description), "A tiny engine used for landers");
        public F Ion_Engine_Description => A(nameof(Ion_Engine_Description), "A low thrust engine with an incredibly high efficiency");
        public F RCS_Thruster_Description => A(nameof(RCS_Thruster_Description), "A set of small directional thrusters, used for docking");
        public F Booster_Description => A(nameof(Booster_Description), "Can only be started once. Has high trust but low efficiency");
        // Aerodynamics
        public F Aerodynamic_Nose_Cone_Description => A(nameof(Aerodynamic_Nose_Cone_Description), "An aerodynamic nose cone, used to improve the aerodynamics of side boosters");
        public F Aerodynamic_Fuselage_Description => A(nameof(Aerodynamic_Fuselage_Description), "An aerodynamic fuselage, used to cover engines");
        public F Fairing_Description => A(nameof(Fairing_Description), "An aerodynamic fairing, used to encapsulate payloads");
        // Electricity
        public F Battery_Description => A(nameof(Battery_Description), "A battery used to store electric power");
        public F Solar_Panel_Description => A(nameof(Solar_Panel_Description), "A solar panel that generates power when extended");
        public F RTG_Description => A(nameof(RTG_Description), "Radioisotope thermoelectric generator or RTG");
        // Utility
        public F Rover_Wheel_Description => A(nameof(Rover_Wheel_Description), "Rover wheel used to drive on the surface of planets");
        public F Docking_Port_Description => A(nameof(Docking_Port_Description), "A docking port which can be used to connect two vehicles together");
        public F Heat_Shield_Description => A(nameof(Heat_Shield_Description), "A heat resistant shield used to survive atmospheric reentry");
        public F Fuel_Pipe_Description => A(nameof(Fuel_Pipe_Description), "A pipe used to transfer fuel");
        #endregion
        #region Part Modules
        [Group("Modules")]
        public F Torque_Module_Torque => A(nameof(Torque_Module_Torque), "Torque: %value%kN");
        public F Separation_Force => A(nameof(Separation_Force), "Separation force: %value%kN");
        [LocSpace]
        public F Max_Heat_Tolerance => A(nameof(Max_Heat_Tolerance), "Heat tolerance: %temperature%");
        [LocSpace]
        [MarkAsSub] public F State_On => A(nameof(State_On), "On");
        [MarkAsSub] public F State_Off => A(nameof(State_Off), "Off");
        [LocSpace]
        public F Engine_Module_State => A(nameof(Engine_Module_State), "Engine %state{0}%");
        public F Engine_On_Label => A(nameof(Engine_On_Label), "Engine on");
        [LocSpace]
        public F Msg_RCS_Module_State => A(nameof(Msg_RCS_Module_State), "RCS %state{0}%");
        [LocSpace]
        public F Wheel_Module_State => A(nameof(Wheel_Module_State), "Rover wheel %state{0}%");
        public F Wheel_On_Label => A(nameof(Wheel_On_Label), "Wheel on");
        [LocSpace]
        public F Panel_Expanded => A(nameof(Panel_Expanded), "Expanded");
        public F Landing_Leg_Expanded => A(nameof(Landing_Leg_Expanded), "Deployed");
        [LocSpace]
        public F Detach_Edges_Label => A(nameof(Detach_Edges_Label), "Detach edges");
        [LocSpace]
        public F Info_Parachute_Max_Height => A(nameof(Info_Parachute_Max_Height), "Max deploy height: %height%");
        public F Msg_Cannot_Deploy_Parachute_In_Vacuum => A(nameof(Msg_Cannot_Deploy_Parachute_In_Vacuum), "Cannot deploy parachute in a vacuum");
        public F Msg_Cannot_Deploy_Parachute_Above => A(nameof(Msg_Cannot_Deploy_Parachute_Above), "Cannot deploy parachute above %height%");
        public F Msg_Cannot_Fully_Deploy_Above => A(nameof(Msg_Cannot_Fully_Deploy_Above), "Cannot fully deploy parachute above %height%");
        public F Msg_Cannot_Deploy_Parachute_While_Faster => A(nameof(Msg_Cannot_Deploy_Parachute_While_Faster), "Cannot deploy parachute when going faster than %velocity%");
        public F Msg_Cannot_Deploy_Parachute_While_Not_Moving => A(nameof(Msg_Cannot_Deploy_Parachute_While_Not_Moving), "Cannot deploy parachute while not moving");
        public F Msg_Parachute_Half_Deployed => A(nameof(Msg_Parachute_Half_Deployed), "Parachute half deployed");
        public F Msg_Parachute_Fully_Deployed => A(nameof(Msg_Parachute_Fully_Deployed), "Parachute fully deployed");
        public F Msg_Parachute_Cut => A(nameof(Msg_Parachute_Cut), "Parachute cut");
        #endregion

        #region Planets
        [Group("Planets", hasSubs = true)]
        public F Sun => A(nameof(Sun), F.Subs("Sun", "the Sun", "The Sun"));
        //
        public F Mercury => A(nameof(Mercury), "Mercury");
        public F Venus => A(nameof(Venus), "Venus");
        //
        public F Earth => A(nameof(Earth), F.Subs("Earth", "the Earth", "The Earth", "Earth's"));
        public F Moon => A(nameof(Moon), F.Subs("Moon", "the Moon", "The Moon"));
        //
        public F Mars => A(nameof(Mars), "Mars");
        public F Phobos => A(nameof(Phobos), "Phobos");
        public F Deimos => A(nameof(Deimos), "Deimos");
        //
        public F Jupiter => A(nameof(Jupiter), F.Subs("Jupiter", "Jupiter", "Jupiter", "Jupiter's"));
        public F Europa => A(nameof(Europa), "Europa");
        public F Ganymede => A(nameof(Ganymede), "Ganymede");
        public F Io => A(nameof(Io), "Io");
        public F Callisto => A(nameof(Callisto), "Callisto");
        #endregion

        #region Landmarks
        [Group("Landmarks")]
        public F Sea_of_Tranquility => A(nameof(Sea_of_Tranquility), "Sea of Tranquility");
        public F Sea_of_Serenity => A(nameof(Sea_of_Serenity), "Sea of Serenity");
        public F Ocean_of_Storms => A(nameof(Ocean_of_Storms), "Ocean of Storms");
        public F Copernicus_Crater => A(nameof(Copernicus_Crater), "Copernicus Crater");
        public F Tycho_Crater => A(nameof(Tycho_Crater), "Tycho Crater");
        [LocSpace]
        public F Olympus_Mons => A(nameof(Olympus_Mons), "Olympus Mons");
        public F Valles_Marineris => A(nameof(Valles_Marineris), "Valles Marineris");
        public F Gale_Crater => A(nameof(Gale_Crater), "Gale Crater");
        public F Hellas_Planitia => A(nameof(Hellas_Planitia), "Hellas Planitia");
        public F Arcadia_Planitia => A(nameof(Arcadia_Planitia), "Arcadia Planitia");
        public F Utopia_Planitia => A(nameof(Utopia_Planitia), "Utopia Planitia");
        [LocSpace]
        public F Atalanta_Planitia => A(nameof(Atalanta_Planitia), "Atalanta Planitia");
        public F Lavinia_Planitia => A(nameof(Lavinia_Planitia), "Lavinia Planitia");
        [LocSpace]
        public F Caloris_Planitia => A(nameof(Caloris_Planitia), "Caloris Planitia");
        public F Borealis_Planitia => A(nameof(Borealis_Planitia), "Borealis Planitia");
        public F Maxwell_Montes => A(nameof(Maxwell_Montes), "Maxwell Montes");
        #endregion

        #region Achievements
        [Group("Achievements")]
        public F Reached_Height => A(nameof(Reached_Height), "Reached %height% altitude");
        public F Reached_Karman_Line => A(nameof(Reached_Karman_Line), "Passed the Karman line, leaving the atmosphere and reaching space");
        public F Survived_Reentry => A(nameof(Survived_Reentry), "Reentered %planet{3}% atmosphere, max temperature %temperature%");
        [LocSpace]
        public F Reached_Low_Orbit => A(nameof(Reached_Low_Orbit), "Reached low %planet{0}% orbit");
        public F Reached_High_Orbit => A(nameof(Reached_High_Orbit), "Reached high %planet{0}% orbit");
        public F Descend_Low_Orbit => A(nameof(Descend_Low_Orbit), "Descended to low %planet{0}% orbit");
        public F Capture_Low_Orbit => A(nameof(Capture_Low_Orbit), "Captured into low %planet{0}% orbit");
        public F Capture_High_Orbit => A(nameof(Capture_High_Orbit), "Captured into high %planet{0}% orbit");
        [LocSpace]
        public F Entered_Lower_Atmosphere => A(nameof(Entered_Lower_Atmosphere), "Entered %planet{3}% lower atmosphere"); // High -> low
        public F Entered_Upper_Atmosphere => A(nameof(Entered_Upper_Atmosphere), "Entered %planet{3}% upper atmosphere"); // Space -> high
        public F Left_Lower_Atmosphere => A(nameof(Left_Lower_Atmosphere), "Reached %planet{3}% upper atmosphere"); // Ground -> upper
        public F Left_Upper_Atmosphere => A(nameof(Left_Upper_Atmosphere), "Escaped %planet{3}% atmosphere"); // Upper -> space
        [LocSpace]
        public F Landed => A(nameof(Landed), "Landed on the surface of %planet{1}%");
        public F Landed_At_Landmark => A(nameof(Landed_At_Landmark),  F.MultilineText("Landed on the surface of %planet{1}%", "", "Location: %landmark%"));
        public F Landed_At_Landmark__Short => A(nameof(Landed_At_Landmark__Short),  F.MultilineText("Landed on the surface of %planet{1}%", "Location: %landmark%"));
        [LocSpace]
        public F Crashed_Into_Terrain => A(nameof(Crashed_Into_Terrain), "Crashed into the surface of %planet{1}%");
        [LocSpace]
        public F Entered_SOI => A(nameof(Entered_SOI), "Entered the sphere of influence of %planet{1}%");
        public F Escaped_SOI => A(nameof(Escaped_SOI), "Escaped the sphere of influence of %planet{1}%");
        [LocSpace]
        public F Docked_Suborbital => A(nameof(Docked_Suborbital), "Docked in a suborbital trajectory of %planet{0}%");
        public F Docked_Orbit_Low => A(nameof(Docked_Orbit_Low), "Docked in low %planet{0}% orbit");
        public F Docked_Orbit_Transfer => A(nameof(Docked_Orbit_Transfer), "Docked in a transfer orbit of %planet{1}%");
        public F Docked_Orbit_High => A(nameof(Docked_Orbit_High), "Docked in high %planet{0}% orbit");
        public F Docked_Escape => A(nameof(Docked_Escape), "Docked on an escape trajectory of %planet{1}%");
        public F Docked_Surface => A(nameof(Docked_Surface), "Docked on the surface of %planet{1}%");
        [LocSpace]
        public F Recover_Home => A(nameof(Recover_Home), "Safely returned to %planet{1}%");
        #endregion
    }

    // Utility
    public partial class SFS_Translation
    {
        readonly Dictionary<string, F> fields = new Dictionary<string, F>();
        F A(string name, string _default)
        {
            if (fields.TryGetValue(name, out F output))
                return output;

            fields[name] = F.Text(_default);
            return fields[name];
        }
        F A(string name, F _default)
        {
            if (fields.TryGetValue(name, out F output))
                return output;

            fields[name] = _default;
            return fields[name];
        }
    }
}

//[Unexported] public Field Booster => GetField(nameof(Booster), "Solid Rocket Booster");
//[Unexported] public Field Engine_Vac => GetField(nameof(Engine_Vac), "%part_name% (Vac)");

// // Future
// #region Saturn V
// [Group("Saturn V")]
// [Unexported] public F Category_Apollo_Payload => A(nameof(Category_Apollo_Payload), "Apollo Payload");
// [Unexported] public F Category_Apollo_Booster => A(nameof(Category_Apollo_Booster), "Apollo Booster");
// [LocSpace]
// [Unexported] public F CSM_LES => A(nameof(CSM_LES), "CSM Launch Escape Tower");
// [Unexported] public F CSM_Docking_Post => A(nameof(CSM_Docking_Post), "CSM Docking Port");
// [Unexported] public F CSM_Parachute => A(nameof(CSM_Parachute), "CSM Parachute");
// [Unexported] public F CSM_Capsule => A(nameof(CSM_Capsule), "CSM Capsule");
// [Unexported] public F CSM_Heat_Shield => A(nameof(CSM_Heat_Shield), "CSM Heat Shield");
// [Unexported] public F CSM_Separator => A(nameof(CSM_Separator), "CSM Separator");
// [Unexported] public F CSM_RCS => A(nameof(CSM_RCS), "CSM RCS");
// [Unexported] public F CSM_Tank => A(nameof(CSM_Tank), "CSM Tank");
// [Unexported] public F CSM_AJ10 => A(nameof(CSM_AJ10), "Aerojet AJ10");
// // [LocSpace]
// // public Field SV_Payload_Fairing => GetField(nameof(SV_Payload_Fairing), "Payload Fairing");
// // public Field SV_Guidance => GetField(nameof(SV_Guidance), "Apollo Guidance Computer");
// // public Field SV_S3_Fuselage => GetField(nameof(SV_S3_Fuselage), "Stage III Fuselage");
// // public Field SV_S3_Tank => GetField(nameof(SV_S3_Tank), "Stage III Fuel Tank");
// // public Field SV_S2_Separator => GetField(nameof(SV_S2_Separator), "Stage II Separator");
// // public Field SV_S2_Fuselage => GetField(nameof(SV_S2_Fuselage), "Stage II Fuselage");
// // public Field SV_S2_Tank => GetField(nameof(SV_S2_Tank), "Stage II Fuel Tank");
// // public Field SV_J2_Cluster => GetField(nameof(SV_J2_Cluster), "Rocketdyne J2 5x Cluster");
// // public Field SV_J2 => GetField(nameof(SV_J2), "Rocketdyne J2");
// // public Field SV_S1_Separator => GetField(nameof(SV_S1_Separator), "Stage I Separator");
// // public Field SV_S1_Fuselage => GetField(nameof(SV_S1_Fuselage), "Stage I Fuselage");
// // public Field SV_S1_Tank => GetField(nameof(SV_S1_Tank), "Stage I Fuel Tank");
// // public Field SV_Base => GetField(nameof(SV_Base), "Thrust Structure");
// // public Field SV_F1_Cluster => GetField(nameof(SV_F1_Cluster), "Rocketdyne F1 5x Cluster");
// // public Field SV_F1 => GetField(nameof(SV_F1), "Rocketdyne F1");
// #endregion
// #region STS
// [Group("STS")]
// [Unexported] public F Category_STS => A(nameof(Category_STS), "Space Shuttle");
// [LocSpace]
// [Unexported] public F STS_Payload_Bay => A(nameof(STS_Payload_Bay), "Payload Bay");
// [Unexported] public F STS_SRB => A(nameof(STS_SRB), "Solid Rocket Booster");
// [Unexported] public F STS_Intertank => A(nameof(STS_Intertank), "Intertank");
// [Unexported] public F STS_Tank_LH2 => A(nameof(STS_Tank_LH2), "Liquid Hydrogen Tank");
// [Unexported] public F STS_Tank_LOX => A(nameof(STS_Tank_LOX), "Liquid Oxygen Tank");
// [Unexported] public F STS_Tank_Tip => A(nameof(STS_Tank_Tip), "External Tank Nosecone");
// [Unexported] public F STS_Back => A(nameof(STS_Back), "Aft Section");
// [Unexported] public F STS_Cockpit => A(nameof(STS_Cockpit), "Cockpit");
// [Unexported] public F STS_RS25 => A(nameof(STS_RS25), "Aerojet Rocketdyne RS-25");
// [Unexported] public F STS_RS25_Cluster => A(nameof(STS_RS25_Cluster), "Shuttle Engine Cluster");
// [Unexported] public F STS_Wing => A(nameof(STS_Wing), "Wing");
// [Unexported] public F STS_Tail => A(nameof(STS_Tail), "Horizontal Stabilizer");
// [Unexported] public F STS_Fuel_Pipe => A(nameof(STS_Fuel_Pipe), "Fuel Pipe");
// [Unexported] public F STS_Fuel_Pipe_Joint => A(nameof(STS_Fuel_Pipe_Joint), "Fuel Pipe Joint");
// [Unexported] public F STS_Fuel_Pipe_End => A(nameof(STS_Fuel_Pipe_End), "Fuel Pipe End");
// [Unexported] public F STS_Fuel_Pipe_LH2 => A(nameof(STS_Fuel_Pipe_LH2), "LH2 Tank Fuel Pipe");
// [LocSpace]
// [Unexported] public F STS_Gear_Deployed => A(nameof(STS_Gear_Deployed), "Gear Deployed");
// [Unexported] public F STS_Payload_Bay_Open => A(nameof(STS_Payload_Bay_Open), "Open");
// #endregion