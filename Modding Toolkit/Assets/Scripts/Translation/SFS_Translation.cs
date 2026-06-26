using System;
using SFS.Translations;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine.Scripting;
using F = SFS.Translations.Field;

// ReSharper disable InconsistentNaming
// /ReSharper disable UnusedMember.Global

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
        [LocSpace]
        public F On => A(nameof(On), "On");
        public F Off => A(nameof(Off), "Off");
        [LocSpace]
        public F Open_Settings_Button => A(nameof(Open_Settings_Button), "Settings");
        public F Open_Cheats_Button => A(nameof(Open_Cheats_Button), "Cheats");
        public F Help => A(nameof(Help), "Help");
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
        [LocSpace]
        public F Video_Tutorials_OpenButton => A(nameof(Video_Tutorials_OpenButton), "Video Tutorials");
        public F Video_Orbit => A(nameof(Video_Orbit), "Orbit Tutorial");
        public F Video_Moon => A(nameof(Video_Moon), "Moon Tutorial");
        public F Video_Dock => A(nameof(Video_Dock), "Docking Tutorial");
        //
        [LocSpace]
        public F Development_Preview => A(nameof(Development_Preview), "Development Preview");
        public F Development_Preview_Explanation => A(nameof(Development_Preview_Explanation), "The development preview showcases future updates we are working on");
        [LocSpace]
        public F Mod_Loader_OpenButton => A(nameof(Mod_Loader_OpenButton), "Mod Loader");
        public F Download_Mods => A(nameof(Download_Mods), "Download Mods");
        public F Open_Mods_Folder => A(nameof(Open_Mods_Folder), "Open Mods Folder");
        //
        [LocSpace]
        public F Community_OpenButton => A(nameof(Community_OpenButton), "Community");
        public F Community_Youtube => A(nameof(Community_Youtube), "Youtube");
        public F Community_Discord => A(nameof(Community_Discord), "Discord");
        public F Community_Reddit => A(nameof(Community_Reddit), "Reddit");
        public F Community_Forums => A(nameof(Community_Forums), "Forums");
        //
        [LocSpace]
        public F Credits_OpenButton => A(nameof(Credits_OpenButton), "Credits");
        public F Credits_Text => A(nameof(Credits_Text), F.MultilineText(
            "<Size=70> Štefo Mai Morojna </size>",
            "<Size=55> Designer - Programmer - Artist </size>",
            "",
            "<Size=70> Jordi van der Molen </size>",
            "<Size=55> Programmer </size>",
            "",
            "<Size=70> Chris Christo </size>",
            "<Size=55> Programmer </size>",
            "",
            "<Size=70> Josh </size>",
            "<Size=55> Programmer </size>",
            "",
            "<Size=70> Aidan Ginise </size>",
            "<Size=55> Programmer </size>",
            "",
            "<Size=70> Andrey Onischenko </size>",
            "<Size=55> Programmer </size>",
            "",
            "<Size=70> Aris Semertzidis </size>",
            "<Size=55> Programmer </size>",
            "",
            "<Size=70> Davi Vasc </size>",
            "<Size=55> Composer </size>",
            "",
            "<Size=70> Ashton Mills </size>",
            "<size=55> Composer </size>"));
        //
        [LocSpace]
        public F First_Time_Playing_Question => A(nameof(First_Time_Playing_Question), "First time playing\nSpaceflight Simulator?");
        public F First_Time_Playing_Yes => A(nameof(First_Time_Playing_Yes), "First Time Playing");
        public F First_Time_Playing_No => A(nameof(First_Time_Playing_No), "Played Before");
        //
        [LocSpace]
        public F Update_Available => A(nameof(Update_Available), "A new update is available!\n\nCurrent version: %old%\nLatest version: %new%");
        public F Update_Confirm => A(nameof(Update_Confirm), "Update");
        //
        [LocSpace]
        public F Rate_Title => A(nameof(Rate_Title), F.MultilineText(
            "Would you like to rate or review the game?",
            "",
            "We deeply care about the quality of our game, your feedback helps us improve it",
            "",
            "Even after thousands of reviews, we still read a large number of them!"
        ));
        public F Rate_Confirm => A(nameof(Rate_Confirm), "Rate");
        //
        [LocSpace]
        public F Exit_Button => A(nameof(Exit_Button), "Exit");
        public F Close_Game => A(nameof(Close_Game), "Close game?");
        public F Follow_Development => A(nameof(Follow_Development), "Follow development");
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
        public F World_Mode_Name => A(nameof(World_Mode_Name), "Mode: %value%");
        public F Mode_Sandbox => A(nameof(Mode_Sandbox), "Sandbox");
        public F Mode_Challenge => A(nameof(Mode_Challenge), "Challenge");
        public F Mode_Career => A(nameof(Mode_Career), "Career");
        [LocSpace]
        public F Allow_Cheats_Name => A(nameof(Allow_Cheats_Name), "Allow Cheats: %value%");
        public F Allow_Cheats_Label => A(nameof(Allow_Cheats_Label), "Allow Cheats");
        [LocSpace]
        public F Allow_Quicksaves_Name => A(nameof(Allow_Quicksaves_Name), "Allow Quicksaves: %value%");
        public F Allow_Quicksaves_Label => A(nameof(Allow_Quicksaves_Label), "Allow Quicksaves");
        [LocSpace]
        public F World_Difficulty_Name => A(nameof(World_Difficulty_Name), "Difficulty: %value%");
        public F Difficulty_Easy => A(nameof(Difficulty_Easy), "Easy");
        public F Difficulty_Normal => A(nameof(Difficulty_Normal), "Normal");
        public F Difficulty_Hard => A(nameof(Difficulty_Hard), "Hard");
        public F Difficulty_Medium => A(nameof(Difficulty_Medium), "Medium");
        public F Difficulty_Extreme => A(nameof(Difficulty_Extreme), "Extreme");
        public F Difficulty_Realistic => A(nameof(Difficulty_Realistic), "Realistic");
        [LocSpace]
        public F World_SolarSystem_Name => A(nameof(World_SolarSystem_Name), "Solar System: %value%");
        [LocSpace]
        public F Last_Played => A(nameof(Last_Played), "Last played: %value% ago");
        public F Just_Played => A(nameof(Just_Played), "Last played: A moment ago");
        public F Time_Played => A(nameof(Time_Played), "Playtime: %value%");
        #endregion

        #region World Create Menu
        [Group("World Create Menu")]
        public F World_Create_Title => A(nameof(World_Create_Title), "Create World");
        public F World_Name_Label => A(nameof(World_Name_Label), "World Name:");
        public F Solar_System_Label => A(nameof(Solar_System_Label), "Solar System:");
        public F Mode_Label => A(nameof(Mode_Label), "Mode:");
        public F Difficulty_Label => A(nameof(Difficulty_Label), "Difficulty:");
        [LocSpace]
        [Unexported] public F Difficulty_Scale_Stat => A(nameof(Difficulty_Scale_Stat), "Scale: 1:%scale%");
        [Unexported] public F Difficulty_Isp_Stat => A(nameof(Difficulty_Isp_Stat), "Specific Impulse: %value%x");
        [Unexported] public F Difficulty_Dry_Mass_Stat => A(nameof(Difficulty_Dry_Mass_Stat), "Tank Dry Mass: %value%x");
        [Unexported] public F Difficulty_Engine_Mass_Stat => A(nameof(Difficulty_Engine_Mass_Stat), "Engine Mass: %value%x");
        #endregion

        #region Teleport Menu

        [Group("Teleport Menu")] public F Teleport_Cheat => A(nameof(Teleport_Cheat), "Teleport");
        public F Teleport_Action => A(nameof(Teleport_Action), "Teleport");
        public F Planet_Select => A(nameof(Planet_Select), "Celestial Body:");
        public F Surface_Teleport_Type => A(nameof(Surface_Teleport_Type), "Surface");
        public F Orbit_Teleport_Type => A(nameof(Orbit_Teleport_Type), "Orbit");
        public F Teleport_Longitude => A(nameof(Teleport_Longitude), "Longitude:");
        public F Teleport_Height => A(nameof(Teleport_Height), "Height (%unit%):");
        public F Teleport_Orbit_Prograde => A(nameof(Teleport_Orbit_Prograde), "Prograde");
        public F Teleport_Orbit_Retrograde => A(nameof(Teleport_Orbit_Retrograde), "Retrograde");
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
        public F Create => A(nameof(Create), "Create");
        public F Delete_File_With_Type => A(nameof(Delete_File_With_Type), "Delete %filename% %filetype%");
        //
        [Documentation("In progress")]
        public F Saving_In_Progress => A(nameof(Saving_In_Progress), "Saving...");
        public F Loading_In_Progress => A(nameof(Loading_In_Progress), "Loading...");
        public F Importing_In_Progress => A(nameof(Importing_In_Progress), "Importing...");
        //
        [Documentation("filetype (injected)")]
        public F Blueprint => A(nameof(Blueprint), "Blueprint");
        public F Quicksave => A(nameof(Quicksave), "Quicksave");
        //
        [Documentation("Ask overwrite menu")]
        public F File_Already_Exists => A(nameof(File_Already_Exists), "A %filetype% with this name already exists");
        public F Overwrite_File => A(nameof(Overwrite_File), "Overwrite %filetype%");
        public F New_File => A(nameof(New_File), "New %filetype%");
        //
        [Documentation("Load failure")]
        public F Load_Failed => A(nameof(Load_Failed), "Could not load %filetype% from %filepath%");
        #endregion

        #region Purchasing
        [Group("Purchasing")]
        public F Open_Shop_Menu => A(nameof(Open_Shop_Menu), "Shop");
        public F Open_Shop_Menu_2 => A(nameof(Open_Shop_Menu_2), "Open shop");
        [LocSpace]
        public F Expansions_List_Title => A(nameof(Expansions_List_Title), "Expansions & Packs");
        public F Bundles_List_Title => A(nameof(Bundles_List_Title), "Bundles");
        [LocSpace]
        public F View_Details => A(nameof(View_Details), "View Details");
        
        [LocSpace]
        public F Parts_Expansion => A(nameof(Parts_Expansion), "Parts Expansion");
        public F Expand_View_Button => A(nameof(Expand_View_Button), "Expand View");
        public F Redstone_Atlas_Pack => A(nameof(Redstone_Atlas_Pack), "Redstone Atlas Pack");
        public F Saturn5_Pack => A(nameof(Saturn5_Pack), "Saturn V Pack");
        public F SLS_Pack => A(nameof(SLS_Pack), "SLS Pack");
        public F Starship_Pack => A(nameof(Starship_Pack), "Starship Pack");
        public F Rockets_Bundle => A(nameof(Rockets_Bundle), "Rockets Bundle");
        public F Skins_Expansion => A(nameof(Skins_Expansion), "Skins Expansion");
        public F Gas_Giants_Expansion => A(nameof(Gas_Giants_Expansion), "Gas Giants Expansion");
        public F Ice_Giants_Expansion => A(nameof(Ice_Giants_Expansion), "Ice Giants Expansion");
        public F Cheats_Expansion => A(nameof(Cheats_Expansion), "Cheats");
        public F Infinite_Area_Expansion => A(nameof(Infinite_Area_Expansion), "Infinite Build Area");
        public F Builder_Bundle => A(nameof(Builder_Bundle), "Builder Bundle");
        public F Sandbox_Bundle => A(nameof(Sandbox_Bundle), "Sandbox Bundle");
        public F Full_Bundle => A(nameof(Full_Bundle), "Full Bundle");
        public F Upgrade_To_Full_Bundle => A(nameof(Upgrade_To_Full_Bundle), "Upgrade To Full Bundle");
        
        public F Most_Popular_Purchase => A(nameof(Most_Popular_Purchase), "Most Popular!");
        public F No_Connection => A(nameof(No_Connection), "[No Connection]");
        
        // Links from game to shop
        [LocSpace]
        public F Mac_Full_Version => A(nameof(Mac_Full_Version), "Full Version");
        public F View_Part_Expansion => A(nameof(View_Part_Expansion), "View Expansion");
        public F Not_All_Parts_Are_Owned_Full_Version => A(nameof(Not_All_Parts_Are_Owned_Full_Version), "Not all parts are owned\nDisabled not owned parts\n\nView parts expansion?");
        public F More_Parts => A(nameof(More_Parts), "View Parts Expansion");
        public F More_Skins => A(nameof(More_Skins), "View Skins Expansion");
        public F Cannot_Use_Cheats_In_Career => A(nameof(Cannot_Use_Cheats_In_Career), "Cheats can only be used in a sandbox mode world");
        public F Get_Infinite_Build_Area_Button => A(nameof(Get_Infinite_Build_Area_Button), "Get Infinite Build Area");
        public F Get_Cheats_Expansion_Button => A(nameof(Get_Cheats_Expansion_Button), "Get Cheats Expansion");
        
        // Checkout
        [LocSpace]
        public F Buy_Product => A(nameof(Buy_Product), "Buy %product% %price%");
        
        public F Timed_Sale_Text => A(nameof(Timed_Sale_Text), "%product_name% -%sale_percent%" + "\n" + "%time_left%");
        public F Time_Upgrade_Text => A(nameof(Time_Upgrade_Text), "Upgrade to %product_name% -%sale_percent%" + "\n" + "%time_left%");

        public F Purchase_Thanks_Msg => A(nameof(Purchase_Thanks_Msg), F.MultilineText(
            "Purchased: %product%",
            "",
            "Thanks for your support!",
            "Now go and explore the stars!"
            ));
        
        public F Owned => A(nameof(Owned), "%product% (Owned)");
        
        public F Restore_Open => A(nameof(Restore_Open), "Restore Purchases");
        
        // Details pages
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
        public F PlanetsExpansion_Jupiter => A(nameof(PlanetsExpansion_Jupiter), "Explore Jupiter and its four moons!" +
                                                                                 "\n" + "From the heavily cratered surface of Callisto, to the vast ice flats of Europa!");
        public F PlanetsExpansion_Saturn => A(nameof(PlanetsExpansion_Saturn), "Visit Saturn, its spectacular rings, and its diverse moons!");
        public F PlanetsExpansion_SaturnMoons => A(nameof(PlanetsExpansion_SaturnMoons), "Splashdown in the methane oceans of Titan and witness the geysers of Enceladus!");
        
        public F PlanetsExpansion_Uranus => A(nameof(PlanetsExpansion_Uranus), "Discover the ice giant Uranus and its rugged moons." +
                                                                                "\n" + "Explore Miranda's towering cliffs and Ariel's icy plains!");

        public F PlanetsExpansion_Neptune => A(nameof(PlanetsExpansion_Neptune), "Venture to Neptune, the distant ice giant and visit the captured moon Triton!");
        
        public F PlanetsExpansion_Pluto => A(nameof(PlanetsExpansion_Pluto), "Visit Pluto and its binary moon, Charon." +
                                                                              "\n" + "Land on the icy landscapes and mountains of these distant cold worlds!");
        
        [Documentation("Full bundle")]
        public F FullBundle_Description => A(nameof(FullBundle_Description), "Get all expansions at a discounted price!");
        
        [LocSpace]
        public F GoogleSignIn => A(nameof(GoogleSignIn), "Sign-In");
        public F GoogleSignOut => A(nameof(GoogleSignOut), "Sign-Out");
        public F SignInForPurchaseRecovery => A(nameof(SignInForPurchaseRecovery), "Sign in to retrieve purchases");
        public F CurrentGoogleAcc => A(nameof(CurrentGoogleAcc), "Account: %email%");
        public F RecoveredPurchases => A(nameof(RecoveredPurchases), "Recovered purchases:");
        public F PurchaseAddId => A(nameof(PurchaseAddId), "Add ID");
        public F PurchaseAlreadyRegistered => A(nameof(PurchaseAlreadyRegistered), "This purchase ID has already been registered.");
        public F OrderNotFound => A(nameof(OrderNotFound), "The purchase was not found, make sure the ID is correct.");
        public F OrderNotProcessed => A(nameof(OrderNotProcessed), "This purchase has not been processed properly.\nIt could have been refunded.");
        public F PurchaseNotConsumed => A(nameof(PurchaseNotConsumed), "This purchase should be functioning normally and can't be claimed.\nTry using the regular restore option.");
        public F RestoredSuccessfully => A(nameof(PurchaseNotConsumed), "Restored purchases successfully:\n%products%");
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
        public F Must_Create_World_Download_BP => A(nameof(Must_Create_World_Download_BP), "You must create a world to download blueprints");
        public F Confirm_Download_Button => A(nameof(Confirm_Download_Button), "Download Blueprint");
        public F Downloading_Message => A(nameof(Downloading_Message), "Downloading...");
        public F Download_Fail => A(nameof(Download_Fail), "Failed to download blueprint");
        public F URL_Invalid => A(nameof(URL_Invalid), "Invalid Blueprint URL");

        // Other
        public F Sharing_Connect_Fail => A(nameof(Sharing_Connect_Fail), "Could not connect to sharing servers");
        #endregion
        
        #region Settings
        [Group("Setting Titles PC")]
        [Unexported] public F General_Title => A(nameof(General_Title), "General Settings");
        [Unexported] public F Video_Title => A(nameof(Video_Title), "Video Settings");
        [Unexported] public F Audio_Title => A(nameof(Audio_Title), "Audio Settings");
        [Unexported] public F Keybindings_Title => A(nameof(Keybindings_Title), "Keybindings");
        
        [Group("Settings PC")]
        [Unexported] public F Video_Resolution_Name => A(nameof(Video_Resolution_Name), "Resolution");
        [Unexported] public F Video_WindowMode_Name => A(nameof(Video_WindowMode_Name), "Window mode");
        [Unexported] public F Fullscreen_Exclusive => A(nameof(Fullscreen_Exclusive), "Fullscreen");
        [Unexported] public F Fullscreen_Borderless => A(nameof(Fullscreen_Borderless), "Borderless");
        [Unexported] public F Fullscreen_Windowed => A(nameof(Fullscreen_Windowed), "Windowed");
        [Unexported] public F Fps_Unlimited => A(nameof(Fps_Unlimited), "Unlimited");
        [Unexported] public F Video_VerticalSync_Name => A(nameof(Video_VerticalSync_Name), "Vertical Sync");
        
        
        [Group("Settings Mobile")] // Settings
        public F Music_Name => A(nameof(Music_Name), "Music");
        public F Sound_Name => A(nameof(Sound_Name), "Sound");
        public F Screen_Rotation_Name => A(nameof(Screen_Rotation_Name), "Screen Rotation");
        public F FPS_Name => A(nameof(FPS_Name), "Fps");
        public F Language_Name => A(nameof(Language_Name), "Language");
        public F Menu_Scale => A(nameof(Menu_Scale), "Interface Scale");
        public F Menu_Opacity => A(nameof(Menu_Opacity), "Interface Opacity");
        public F Shakes_Name => A(nameof(Shakes_Name), "Camera Shake");
        public F Orbit_Line_Count => A(nameof(Orbit_Line_Count), "Orbit Line Count");
        public F Anti_Aliasing => A(nameof(Anti_Aliasing), "Anti-Aliasing");
        public F Set_Save_Location => A(nameof(Set_Save_Location), "Set save location");
        public F Change_Save_Location => A(nameof(Change_Save_Location), "Change save location");
        public F Open_Save_Location => A(nameof(Open_Save_Location), "Open save location");
        public F Current_Save_Location => A(nameof(Current_Save_Location), "Current save location:\n%path%");

        [Group("Cheats")] // Cheats
        public F Infinite_Build_Area_Name => A(nameof(Infinite_Build_Area_Name), "Infinite Build Area");
        public F Part_Clipping_Name => A(nameof(Part_Clipping_Name), "Part Clipping");
        public F Infinite_Fuel_Name => A(nameof(Infinite_Fuel_Name), "Infinite Fuel");
        //public F Infinite_Oxygen_Name => A(nameof(Infinite_Oxygen_Name), "Infinite Oxygen");
        public F No_Atmospheric_Drag_Name => A(nameof(No_Atmospheric_Drag_Name), "No Atmospheric Drag");
        public F No_Collision_Damage_Name => A(nameof(No_Collision_Damage_Name), "No Collision Damage");
        public F No_Gravity_Name => A(nameof(No_Gravity_Name), "No Gravity");
        public F No_Heat_Damage_Name => A(nameof(No_Heat_Damage_Name), "No Heat Damage");
        public F No_Burn_Marks_Name => A(nameof(No_Burn_Marks_Name), "No Burn Marks");
        public F Refill_Fuel_Tanks => A(nameof(Refill_Fuel_Tanks), "Refill Fuel Tanks");
        public F Refilled_Tanks => A(nameof(Refilled_Tanks), "Refilled all fuel tanks");
        
        public F Unlock_Cheats_Button => A(nameof(Unlock_Cheats_Button), "Unlock Cheats");

        public F Unlock_Cheats_Warning => A(nameof(Unlock_Cheats_Warning), "Enabling cheats will convert your Challenge world to a Sandbox world.\n\nThis change is irreversible.\n\nAre you sure you want to proceed?");
        #endregion

        #region Tutorials
        [Group("Tutorials")]
        public F Tut_Drag_And_Drop => A(nameof(Tut_Drag_And_Drop), "Drag and drop parts" + "\n" + "to build your rocket");
        public F Tut_Part_Info => A(nameof(Tut_Part_Info), "Click to view" + "\n" + "part information");
        [LocSpace]
        public F Tut_Use_Part => A(nameof(Tut_Use_Part), "Click on the engine to turn it on!\n(Click on parts to use them)");
        public F Tut_Retry => A(nameof(Tut_Retry), "Modify rocket\nand retry launch?");
        public F Tut_Ignition => A(nameof(Tut_Ignition), "Ignition!");
        public F Tut_Throttle => A(nameof(Tut_Throttle), "Adjust throttle\nto 100%");
        public F Tut_Double_Click => A(nameof(Tut_Double_Click), "Double click to select\nall connected parts");
        public F Tut_Symmetry_Mode => A(nameof(Tut_Symmetry_Mode), "Symmetry mode");
        public F Tut_Area_Select => A(nameof(Tut_Area_Select), "Hold, then drag for area select");
        public F Tut_Stages => A(nameof(Tut_Stages), "Stages allow you to activate\nmultiple parts at once");
        public F Tut_Parachute => A(nameof(Tut_Parachute), "Use parachutes to land\nyour crew safely at\nthe end of your flight");
        public F Tut_Capsule => A(nameof(Tut_Capsule), "Small capsule with\none astronaut inside it");
        public F Tut_Separator => A(nameof(Tut_Separator), "Use separators to\ndetach stages when\nthey run out of fuel");
        public F Tut_Fuel_Tanks => A(nameof(Tut_Fuel_Tanks), "Fuel tanks");
        public F Tut_Rocket_Engines => A(nameof(Tut_Rocket_Engines), "Rocket engines");
        public F Tut_Example_Rockets => A(nameof(Tut_Example_Rockets), "Example rockets");
        public F Tut_Infinite_Area => A(nameof(Tut_Infinite_Area), "Infinite area can be enabled in cheats");
        public F Tut_No_Fuel_Source => A(nameof(Tut_No_Fuel_Source), "Engine has no\nfuel source");
        #endregion

        #region Hub
        [Group("Hub")]
        public F Funds_Text => A(nameof(Funds_Text), "Funds: %funds%");
        public F Go_To_Space_Center => A(nameof(Go_To_Space_Center), "Space Center");
        public F Exit_To_Space_Center => A(nameof(Exit_To_Space_Center), "Exit To Space Center");
        public F Research_And_Development => A(nameof(Research_And_Development), "Research & Development %complete%/%total%");
        public F Challenges_Info => A(nameof(Challenges_Info), "Challenges: %complete%/%total%");
        public F Challenges_Button => A(nameof(Challenges_Button), "Challenges %complete%/%total%");
        public F Challenges_Title => A(nameof(Challenges_Title), "Challenges:");
        public F Challenges_Completed => A(nameof(Challenges_Completed), "<size=55>Challenge completed:</size>" + "\n" + "%challenge%");
        public F This_Is_Challenging_Game => A(nameof(This_Is_Challenging_Game), "This is a challenging game that\nrequires patience and perseverance");
        public F Rocket_Science_Is_Hard => A(nameof(Rocket_Science_Is_Hard), "Rocket science is hard");
        public F Dont_Give_Up => A(nameof(Dont_Give_Up), "Don't give up, keep trying and you will succeed!");
        public F New_Part_Unlock_Available => A(nameof(New_Part_Unlock_Available), "New part unlock is available");
        public F Insufficient_Funds => A(nameof(Insufficient_Funds), "Insufficient funds");
        #endregion
        
        #region Build
        [Group("Build")]
        public F Build_New_Rocket => A(nameof(Build_New_Rocket), "Build New Rocket");
        public F New => A(nameof(New), "New");
        public F Expand_Last_Rocket => A(nameof(Expand_Last_Rocket), "Continue Build");
        public F Not_All_Parts_Owned => A(nameof(Not_All_Parts_Owned), "Not all parts are owned\nDisabled not owned parts\n\nView full version?");
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
        public F Auto_Detach_Capsule => A(nameof(Auto_Detach_Capsule), "Auto Detach Capsule");
        //
        [Documentation("Launch warnings")]
        public F Warnings_Title => A(nameof(Warnings_Title), "WARNING:");
        public F Missing_Capsule => A(nameof(Missing_Capsule), "Your rocket has no capsule or probe, making it uncontrollable");
        [Unexported] public F Mission_Crew => A(nameof(Mission_Crew), "Your rocket has no crew onboard, making it uncontrollable");
        public F Missing_Parachute => A(nameof(Missing_Parachute), "Your rocket has no parachute");
        public F Missing_Heat_Shield => A(nameof(Missing_Heat_Shield), "Your rocket has no heat shield");
        public F Missing_Fuel_Popup => A(nameof(Missing_Fuel_Popup), "No fuel source");
        public F Too_Heavy => A(nameof(Too_Heavy), Field.MultilineText("Your rocket is too heavy to launch", "%mass%", "%thrust%"));
        [LocSpace]
        public F Launch_Anyway_Button => A(nameof(Launch_Anyway_Button), "Launch Anyway");
        [LocSpace]
        public F Launch_Horizontally_Ask => A(nameof(Launch_Horizontally_Ask), "Launch horizontally?");
        public F Launch_Horizontally_Confirm => A(nameof(Launch_Horizontally_Confirm), "Launch Horizontally");
        public F Launch_Vertically_Confirm => A(nameof(Launch_Vertically_Confirm), "Launch Vertically");
        //
        [Documentation("Example rockets")]
        public F Example_Rockets_OpenMenu => A(nameof(Example_Rockets_OpenMenu), "Example Rockets");
        public F Basic_Rocket => A(nameof(Basic_Rocket), "Basic Rocket");
        public F Stages => A(nameof(Stages), "Two Stage Rocket");
        public F Ideal_Stages => A(nameof(Ideal_Stages), "Three Stage Rocket");
        public F Lander => A(nameof(Lander), "Moon Lander");
        #endregion

        #region Map
        [Group("Map")]
        public F Toggle_Map_Button => A(nameof(Toggle_Map_Button), "Map");
        public F Escape => A(nameof(Escape), "Escape");
        public F Encounter => A(nameof(Encounter), "Encounter");
        public F Rendezvous => A(nameof(Rendezvous), "Rendezvous");
        public F Transfer => A(nameof(Transfer), "Transfer Window");
        #endregion

        #region Game
        [Group("Game")]
        public F Throttle_On => A(nameof(Throttle_On), "On");
        public F Throttle_Off => A(nameof(Throttle_Off), "Off");
        public F Ignition => A(nameof(Ignition), "IGNITION");
        public F RCS => A(nameof(RCS), "RCS");
        public F Rocket_Has_No_RCS => A(nameof(Rocket_Has_No_RCS), "Your rocket has no RCS thrusters");

        // Height/Velocity/Angle
        [Documentation("Game supports screen rotation, we split into 2 lines in vertical mode")]
        public F Height_Terrain_Vertical => A(nameof(Height_Terrain_Vertical), "Height (Terrain):\n\n%height%");
        public F Height_Vertical => A(nameof(Height_Vertical), "Height:\n\n%height%");
        public F Velocity_Vertical => A(nameof(Velocity_Vertical), "Velocity:\n\n%speed%");
        public F Velocity_Relative_Vertical => A(nameof(Velocity_Relative_Vertical), "Velocity (Relative):\n\n%speed%");
        public F Distance_Relative_Vertical => A(nameof(Distance_Relative_Vertical), "Distance (Relative):\n\n%distance%");
        public F Angle_Vertical => A(nameof(Angle_Vertical), "Angle:\n\n%angle% / %targetAngle%");
        //
        [LocSpace]
        public F Height_Terrain_Horizontal => A(nameof(Height_Terrain_Horizontal), "Height (Terrain): %height%");
        public F Height_Horizontal => A(nameof(Height_Horizontal), "Height: %height%");
        public F Velocity_Horizontal => A(nameof(Velocity_Horizontal), "Velocity: %speed%");
        public F Velocity_Relative_Horizontal => A(nameof(Velocity_Relative_Horizontal), "Velocity (Relative): %speed%");
        public F Distance_Relative_Horizontal => A(nameof(Distance_Relative_Horizontal), "Distance (Relative): %distance%");
        public F Angle_Horizontal => A(nameof(Angle_Horizontal), "Angle: %angle% / %targetAngle%");
        //
        [LocSpace]
        public F Relative_Velocity_Arrow => A(nameof(Relative_Velocity_Arrow), "Relative Velocity\n%velocity%");
        public F Side_Velocity_Arrow => A(nameof(Side_Velocity_Arrow), "Side Velocity\n%velocity%");
        public F Forward_Velocity_Arrow => A(nameof(Forward_Velocity_Arrow), "Distance\n%distance%\n\nVelocity\n%velocity%");
        [LocSpace]
        public F Cannot_Ignite_Vacuum_Engines_Below => A(nameof(Cannot_Ignite_Vacuum_Engines_Below), "Cannot ignite vacuum engines below %height%");
        public F Cannot_Use_Vacuum_Engines_In_Atmosphere => A(nameof(Cannot_Use_Vacuum_Engines_In_Atmosphere), "Cannot use vacuum engines in the lower atmosphere");
        
        [Group("Failure menu")]
        public F Failure_Cause => A(nameof(Failure_Cause), "FAILURE CAUSE:");
        public F Failure_Crash_Into_Rocket => A(nameof(Failure_Crash_Into_Rocket), "Crashed into another rocket");
        public F Failure_Crash_Into_The_Ocean => A(nameof(Failure_Crash_Into_The_Ocean), "Crashed into the ocean");
        public F Failure_Burn_Up => A(nameof(Failure_Burn_Up), "Burned up on reentry");
        [Group(GROUP_NAME_DYNAMIC_PLANET_SPECIFIC)]
        public F Failure_Crash_Into_Terrain => A(nameof(Failure_Crash_Into_Terrain), "Crashed into the surface of %planet{1}%");
        #endregion
        
        #region Game menus
        [Group("End mission menu")]
        // Rocket/generic
        public F Recover_Rocket => A(nameof(Recover_Rocket), "Recover");
        public F Destroy_Rocket => A(nameof(Destroy_Rocket), "Destroy");
        // Debris
        public F Debris_Recover => A(nameof(Debris_Recover), "Recover Debris");
        public F Debris_Destroy => A(nameof(Debris_Destroy), "Destroy Debris");
        public F Debris_Recover_Title => A(nameof(Debris_Recover_Title), "Recover debris?");
        public F Debris_Destroy_Title => A(nameof(Debris_Destroy_Title), "Destroy debris?");
        public F View_Mission_Log => A(nameof(View_Mission_Log), "View Flight Log");
        // Astronaut
        [Unexported] public F Crewed_Destroy_Warning => A(nameof(Crewed_Destroy_Warning), "Destroying this rocket will kill all crew on board");
        
        [Documentation("Restart menu")]
        public F Restart_Mission_To_Launch_Warning => A(nameof(Restart_Mission_To_Launch_Warning), "WARNING:\nThis will undo all progress since last launch");
        public F Restart_Mission_To_Build_Warning => A(nameof(Restart_Mission_To_Build_Warning), "WARNING:\nThis will undo all progress since last launch");
        public F Restart_Mission_To_Launch => A(nameof(Restart_Mission_To_Launch), "Revert To Launch");
        public F Restart_Mission_To_Build => A(nameof(Restart_Mission_To_Build), "Revert To Build");
        public F Revert_30_Secs => A(nameof(Revert_30_Secs), "Revert 30 Sec");
        public F Revert_3_Min => A(nameof(Revert_3_Min), "Revert 3 Min");
        //
        [Documentation("End mission menu")]
        public F End_Challenges_Title => A(nameof(End_Challenges_Title), "Completed Challenges:");
        public F End_Logs_Title => A(nameof(End_Logs_Title), "Mission Log:");
        public F Continue_To_Log => A(nameof(Continue_To_Log), "Continue");
        public F Back_To_Challenges => A(nameof(Back_To_Challenges), "Back");
        //
        [Documentation("Clear space junk/debris")]
        public F Clear_Debris_Warning => A(nameof(Clear_Debris_Warning), "Clear debris?" + "\n\n" + "This will remove all uncontrollable rockets");
        public F Clear_Debris_Confirm => A(nameof(Clear_Debris_Confirm), "Clear Debris");
        //
        [Documentation("Select menu")]
        [LocSpace]
        public F Navigate_To => A(nameof(Navigate_To), "Navigate To");
        public F End_Navigation => A(nameof(End_Navigation), "End Navigation");
        public F Focus => A(nameof(Focus), "Focus");
        public F Unfocus => A(nameof(Unfocus), "Unfocus");
        public F Track => A(nameof(Track), "Track");
        public F Stop_Tracking => A(nameof(Stop_Tracking), "Stop Tracking");
        public F Switch_To => A(nameof(Switch_To), "Switch To");
        [Unexported] public F Collect_Rock => A(nameof(Collect_Rock), "Collect");
        #endregion

        #region Rocket
        [Group("Rocket")]
        public F Default_Rocket_Name => A(nameof(Default_Rocket_Name), "Rocket");
        public F No_Control_Msg => A(nameof(No_Control_Msg), "No control");
        #endregion

        #region Timewarp
        [Group("Timewarp")]
        public F Msg_Timewarp_Speed => A(nameof(Msg_Timewarp_Speed), "Time acceleration %speed%x");
        [LocSpace]
        public F Cannot_Timewarp_Below_Basic => A(nameof(Cannot_Timewarp_Below_Basic), "Cannot timewarp below %height%");
        public F Cannot_Timewarp_Below => A(nameof(Cannot_Timewarp_Below), "Cannot timewarp faster than %speed%x while below %height%");
        public F Cannot_Timewarp_While_Moving_On_Surface => A(nameof(Cannot_Timewarp_While_Moving_On_Surface), "Cannot timewarp faster than %speed%x while moving on the surface");
        public F Cannot_Timewarp_While_Moving_Water => A(nameof(Cannot_Timewarp_While_Moving_Water), "Cannot timewarp faster than %speed%x while moving in water");
        public F Cannot_Timewarp_While_Accelerating => A(nameof(Cannot_Timewarp_While_Accelerating), "Cannot timewarp faster than %speed%x while under acceleration");
        public F Cannot_Use_Part_While_Timewarping => A(nameof(Cannot_Use_Part_While_Timewarping), "Cannot use %part% while timewarping");
        public F Cannot_Turn_While_Timewarping => A(nameof(Cannot_Turn_While_Timewarping), "Cannot turn while timewarping");
        [LocSpace]
        public F Timewarp_To_Button => A(nameof(Timewarp_To_Button), "Timewarp Here");
        #endregion

        #region Units
        [Group("Units")]
        public F Thrust_To_Weight_Ratio => A(nameof(Thrust_To_Weight_Ratio), "Thrust / Weight: %value%");
        public F Mass => A(nameof(Mass), "Mass: %value%t");
        public F Density => A(nameof(Density), "Density: %value%");
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
        [Group("Resource Types")]
        public F Solid_Fuel => A(nameof(Solid_Fuel), "Solid fuel");
        public F Liquid_Fuel => A(nameof(Liquid_Fuel), "Liquid fuel");
        [Unexported] public F Kerolox => A(nameof(Kerolox), "Kerolox");
        [Unexported] public F Hydrolox => A(nameof(Hydrolox), "Hydrolox");
        [Unexported] public F Methalox => A(nameof(Methalox), "Methalox");
        [Unexported] public F Hydrazine => A(nameof(Hydrazine), "Hydrazine");
        
        [Group("Resource Uses")]
        public F Resource_Bars_Title => A(nameof(Resource_Bars_Title), "%resource_name%:");
        public F Info_Resource_Amount => A(nameof(Info_Resource_Amount), "%resource%: %amount%");
        public F Msg_No_Resource_Source => A(nameof(Msg_No_Resource_Source), "No %resource% source");
        public F Msg_No_Resource_Left => A(nameof(Msg_No_Resource_Left), "Out of %resource%");
        #endregion
        
        #region Pick Categories
        [Group("Part Categories")]
        public F Basic_Parts => A(nameof(Basic_Parts), "Basics");
        public F Six_Wide_Parts => A(nameof(Six_Wide_Parts), "6 Wide");
        public F Eight_Wide_Parts => A(nameof(Eight_Wide_Parts), "8 Wide");
        public F Ten_Wide_Parts => A(nameof(Ten_Wide_Parts), "10 Wide");
        public F Twelve_Wide_Parts => A(nameof(Twelve_Wide_Parts), "12 Wide");
        public F Engine_Parts => A(nameof(Engine_Parts), "Engines");
        public F Boosters_Parts => A(nameof(Boosters_Parts), "Boosters");
        public F Aerodynamics_Parts => A(nameof(Aerodynamics_Parts), "Aerodynamics");
        public F Fairings_Parts => A(nameof(Fairings_Parts), "Fairings");
        public F Structural_Parts => A(nameof(Structural_Parts), "Structural");
        public F Other_Parts => A(nameof(Other_Parts), "Other");
        public F Redstone_Atlas => A(nameof(Redstone_Atlas), "Redstone Atlas");
        public F Boosters => A(nameof(Boosters), "Boosters");
        #endregion

        #region Part Names
        [Group("Part Names")]
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
        public F Separator_Name => A(nameof(Separator_Name), "Stage Separator");
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

        public F A_7_Engine_Name => A(nameof(A_7_Engine_Name), "A-7 Engine");
        public F Side_Cover_Name => A(nameof(Side_Cover_Name), "Side Cover");
        public F Cover_name => A(nameof(Cover_name), "Cover");
        public F Fuselage_Name => A(nameof(Fuselage_Name), "Fuselage");
        public F LR_89_5_Engine_Name => A(nameof(LR_89_5_Engine_Name), "LR-89-5 Engine");
        public F LR_105_5_Engine_Name => A(nameof(LR_105_5_Engine_Name), "LR-105-5 Engine");
        public F Adapter_Name => A(nameof(Adapter_Name), "Adapter");
        public F Engine_Base_Name => A(nameof(Engine_Base_Name), "Engine Base");
        public F Retro_Pack_Name => A(nameof(Retro_Pack_Name), "Retro Pack");
        public F LES_Name => A(nameof(LES_Name), "Launch Escape System");


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
        public F Booster_Description => A(nameof(Booster_Description), "Has high thrust, low efficiency" + "\n" + "Cannot be turned off or throttle once ignite");
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
        public F Magnet_Force => A(nameof(Magnet_Force), "Magnet force: %value%kN");
        [LocSpace]
        public F Max_Heat_Tolerance => A(nameof(Max_Heat_Tolerance), "Heat tolerance: %temperature%");
        [LocSpace]
        [MarkAsSub] public F State_On => A(nameof(State_On), "On");
        [MarkAsSub] public F State_Off => A(nameof(State_Off), "Off");
        [LocSpace]
        public F Engine_Module_State => A(nameof(Engine_Module_State), "Engine %state%");
        public F Engine_On_Label => A(nameof(Engine_On_Label), "Engine on");
        public F Gimbal_On_Label => A(nameof(Gimbal_On_Label), "Gimbal on");
        public F Engine_Cover_Label => A(nameof(Engine_Cover_Label), "Engine cover");
        [LocSpace]
        public F Booster_On => A(nameof(Booster_On), "Booster ignition: On");
        public F Booster_Off => A(nameof(Booster_Off), "Booster ignition: Off");
        public F Cannot_Ignite_Covered_Booster => A(nameof(Cannot_Ignite_Covered_Booster), "Cannot ignite a covered booster");
        public F Booster_Cannot_Be_Off => A(nameof(Booster_Cannot_Be_Off), "Solid fuel boosters cannot be turned off once ignited");
        [LocSpace]
        public F Msg_RCS_Module_State => A(nameof(Msg_RCS_Module_State), "RCS %state%");
        [LocSpace]
        public F Wheel_Module_State => A(nameof(Wheel_Module_State), "Rover wheel %state%");
        public F Wheel_On_Label => A(nameof(Wheel_On_Label), "Wheel on");
        [LocSpace]
        public F Panel_Expanded => A(nameof(Panel_Expanded), "Expanded");
        public F Landing_Leg_Expanded => A(nameof(Landing_Leg_Expanded), "Deployed");
        [LocSpace]
        public F Detach_Edges_Label => A(nameof(Detach_Edges_Label), "Detach edges");
        public F Adapt_To_Tanks_Label => A(nameof(Adapt_To_Tanks_Label), "Adapt to fuel tanks");
        [LocSpace]
        public F Info_Parachute_Max_Height => A(nameof(Info_Parachute_Max_Height), "Max deploy height: %height%");
        public F Msg_Cannot_Deploy_Parachute_In_Vacuum => A(nameof(Msg_Cannot_Deploy_Parachute_In_Vacuum), "Cannot deploy parachute in a vacuum");
        public F Msg_Cannot_Deploy_Parachute_Above => A(nameof(Msg_Cannot_Deploy_Parachute_Above), "Cannot deploy parachute above %height%");
        public F Msg_Cannot_Fully_Deploy_Above => A(nameof(Msg_Cannot_Fully_Deploy_Above), "Cannot fully deploy parachute above %height%");
        public F Msg_Cannot_Deploy_Parachute_While_Faster => A(nameof(Msg_Cannot_Deploy_Parachute_While_Faster), "Cannot deploy parachute when going faster than %velocity%");
        public F Msg_Cannot_Deploy_Parachute_While_Not_Moving => A(nameof(Msg_Cannot_Deploy_Parachute_While_Not_Moving), "Cannot deploy parachute while not moving");
        public F Msg_Cannot_Deploy_Parachute_Under_Water => A(nameof(Msg_Cannot_Deploy_Parachute_Under_Water), "Cannot deploy parachute underwater");
        public F Msg_Parachute_Half_Deployed => A(nameof(Msg_Parachute_Half_Deployed), "Parachute half deployed");
        public F Msg_Parachute_Fully_Deployed => A(nameof(Msg_Parachute_Fully_Deployed), "Parachute fully deployed");
        public F Msg_Parachute_Cut => A(nameof(Msg_Parachute_Cut), "Parachute cut");
        #endregion
        #region Part Pick Categories
        [Group("Part Pick Categories")]
        [Documentation("Part Categories in Pick Part Menu")]
        public F Part_Pick_Category_Aero => A(nameof(Part_Pick_Category_Aero), "Aero");
        public F Part_Pick_Category_Parachutes => A(nameof(Part_Pick_Category_Parachutes), "Parachutes");
        public F Part_Pick_Category_Engines => A(nameof(Part_Pick_Category_Engines), "Engines");
        public F Part_Pick_Category_Structural => A(nameof(Part_Pick_Category_Structural), "Structural");
        public F Part_Pick_Category_Utility => A(nameof(Part_Pick_Category_Utility), "Utility");
        public F Part_Pick_Category_Control => A(nameof(Part_Pick_Category_Control), "Control");
        public F Part_Pick_Category_Landing => A(nameof(Part_Pick_Category_Landing), "Landing");
        public F Part_Pick_Category_Electricity => A(nameof(Part_Pick_Category_Electricity), "Electric");
        public F Part_Pick_Category_Fuel => A(nameof(Part_Pick_Category_Fuel), "Fuel\nTanks");
        public F Part_Pick_Category_Fairings => A(nameof(Part_Pick_Category_Fairings), "Fairings");
        public F Part_Pick_Category_Wheels => A(nameof(Part_Pick_Category_Wheels), "Wheels");
        public F Part_Pick_Category_Docking => A(nameof(Part_Pick_Category_Docking), "Docking");
        public F Part_Pick_Category_Heat_Protection => A(nameof(Part_Pick_Category_Heat_Protection), "Heat\nShields");
        public F Part_Pick_Category_Staging => A(nameof(Part_Pick_Category_Staging), "Staging");
        public F Part_Pick_Category_Mounts => A(nameof(Part_Pick_Category_Mounts), "Mounts");
        #endregion

        #region Planets
        public const string GROUP_NAME_PLANETS = "Planets";
        [Group(GROUP_NAME_PLANETS, hasSubs = true)]
        public F Sun => A(nameof(Sun), F.Subs("Sun", "the Sun", "The Sun"));
        //
        public F Mercury => A(nameof(Mercury), "Mercury");
        public F Venus => A(nameof(Venus), "Venus");
        //
        public F Earth => A(nameof(Earth), F.Subs("Earth", "the Earth", "The Earth", "Earth's"));
        public F Moon => A(nameof(Moon), F.Subs("Moon", "the Moon", "The Moon"));
        public F Near_Earth_Asteroid => A(nameof(Near_Earth_Asteroid), F.Subs("Captured Asteroid", "the Captured Asteroid", "The Captured Asteroid"));
        //
        public F Mars => A(nameof(Mars), "Mars");
        public F Phobos => A(nameof(Phobos), "Phobos");
        public F Deimos => A(nameof(Deimos), "Deimos");
        //
        public F Ceres => A(nameof(Ceres), "Ceres");
        //
        public F Jupiter => A(nameof(Jupiter), F.Subs("Jupiter", "Jupiter", "Jupiter", "Jupiter's"));
        public F Europa => A(nameof(Europa), "Europa");
        public F Ganymede => A(nameof(Ganymede), "Ganymede");
        public F Io => A(nameof(Io), "Io");
        public F Callisto => A(nameof(Callisto), "Callisto");
        public F Thebe => A(nameof(Thebe), F.Subs("Thebe", "Thebe", "Thebe", "Thebe's"));
        //
        public F Saturn => A(nameof(Saturn), F.Subs("Saturn", "Saturn", "Saturn", "Saturn's"));
        public F Pan => A(nameof(Pan), F.Subs("Pan", "Pan", "Pan", "Pan's"));
        public F Enceladus => A(nameof(Enceladus), F.Subs("Enceladus", "Enceladus", "Enceladus", "Enceladus's"));
        public F Iapetus => A(nameof(Iapetus), F.Subs("Iapetus", "Iapetus", "Iapetus", "Iapetus's"));
        public F Titan => A(nameof(Titan), F.Subs("Titan", "Titan", "Titan", "Titan's"));
        public F Rhea => A(nameof(Rhea), F.Subs("Rhea", "Rhea", "Rhea", "Rhea's"));
        public F Tethys => A(nameof(Tethys), F.Subs("Tethys", "Tethys", "Tethys", "Tethys's"));
        public F Dione => A(nameof(Dione), F.Subs("Dione", "Dione", "Dione", "Dione's"));
        public F Mimas => A(nameof(Mimas), F.Subs("Mimas", "Mimas", "Mimas", "Mimas's"));
        //
        public F Uranus => A(nameof(Uranus), F.Subs("Uranus", "Uranus", "Uranus", "Uranus's"));
        public F Miranda => A(nameof(Miranda), F.Subs("Miranda", "Miranda", "Miranda", "Miranda's"));
        public F Ariel => A(nameof(Ariel), F.Subs("Ariel", "Ariel", "Ariel", "Ariel's"));
        public F Titania => A(nameof(Titania), F.Subs("Titania", "Titania", "Titania", "Titania's"));
        public F Umbriel => A(nameof(Umbriel), F.Subs("Umbriel", "Umbriel", "Umbriel", "Umbriel's"));
        public F Oberon => A(nameof(Oberon), F.Subs("Oberon", "Oberon", "Oberon", "Oberon's"));
        public F Puck => A(nameof(Puck), F.Subs("Puck", "Puck", "Puck", "Puck's"));
        //
        public F Neptune => A(nameof(Neptune), F.Subs("Neptune", "Neptune", "Neptune", "Neptune's"));
        public F Proteus => A(nameof(Proteus), F.Subs("Proteus", "Proteus", "Proteus", "Proteus's"));
        public F Triton => A(nameof(Triton), F.Subs("Triton", "Triton", "Triton", "Triton's"));
        public F Naiad => A(nameof(Naiad), F.Subs("Naiad", "Naiad", "Naiad", "Naiad's"));
        //
        public F Pluto => A(nameof(Pluto), F.Subs("Pluto", "Pluto", "Pluto", "Pluto's"));
        public F Charon => A(nameof(Charon), F.Subs("Charon", "Charon", "Charon", "Charon's"));
        public F Nix => A(nameof(Nix), F.Subs("Nix", "Nix", "Nix", "Nix's"));
        public F Hydra => A(nameof(Hydra), F.Subs("Hydra", "Hydra", "Hydra", "Hydra's"));
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
        public F Jezero_Crater => A(nameof(Jezero_Crater), "Jezero Crater");
        [LocSpace]
        public F Stickney_Crater => A(nameof(Stickney_Crater), "Stickney Crater");
        [LocSpace]
        public F Voltaire_Crater => A(nameof(Voltaire_Crater), "Voltaire Crater");
        public F Swift_Crater => A(nameof(Swift_Crater), "Swift Crater");
        [LocSpace]
        public F Atalanta_Planitia => A(nameof(Atalanta_Planitia), "Atalanta Planitia");
        public F Lavinia_Planitia => A(nameof(Lavinia_Planitia), "Lavinia Planitia");
        [LocSpace]
        public F Caloris_Planitia => A(nameof(Caloris_Planitia), "Caloris Planitia");
        public F Borealis_Planitia => A(nameof(Borealis_Planitia), "Borealis Planitia");
        public F Maxwell_Montes => A(nameof(Maxwell_Montes), "Maxwell Montes");
        [LocSpace]
        public F Laica_Crater => A(nameof(Laica_Crater), "Laica Crater");
        public F Kachina_Chasmata => A(nameof(Kachina_Chasmata), "Kachina Chasmata");
        [LocSpace]
        public F Urvara_Crater => A(nameof(Urvara_Crater), "Urvara Crater");
        public F Kerwan_Crater => A(nameof(Kerwan_Crater), "Kerwan Crater");
        [LocSpace]
        public F Bosphorus_Regio => A(nameof(Bosphorus_Regio), "Bosphorus Regio");
        public F Colchis_Regio => A(nameof(Colchis_Regio), "Colchis Regio");
        public F Chalybes_Regio => A(nameof(Chalybes_Regio), "Chalybes Regio");
        [LocSpace]
        public F Conamara_Chaos => A(nameof(Conamara_Chaos), "Conamara Chaos");
        [LocSpace]
        public F Galileo_Regio => A(nameof(Galileo_Regio), "Galileo Regio");
        [LocSpace]
        public F Equatorial_Ridge => A(nameof(Equatorial_Ridge), "Equatorial Ridge");
        [LocSpace]
        public F Verona_Rupes => A(nameof(Verona_Rupes), "Verona Rupes");
        public F Arden_Corona => A(nameof(Arden_Corona), "Arden Corona");
        public F Elsinore_Corona => A(nameof(Elsinore_Corona), "Elsinore Corona");
        [LocSpace]
        public F Mommur_Chasma => A(nameof(Mommur_Chasma), "Mommur Chasma");
        [LocSpace]
        public F Wunda_Crater => A(nameof(Wunda_Crater), "Wunda Crater");
        public F Vuver_Crater => A(nameof(Vuver_Crater), "Vuver Crater");
        public F Skynd_Crater => A(nameof(Skynd_Crater), "Skynd Crater");
        [LocSpace]
        public F Telemachus => A(nameof(Telemachus), "Telemachus");
        public F Ithaca_Chasma => A(nameof(Ithaca_Chasma), "Ithaca Chasma");
        [LocSpace]
        public F Bogle_Crater => A(nameof(Bogle_Crater), "Bogle Crater");
        public F Lob_Crater => A(nameof(Lob_Crater), "Lob Crater");
        public F Butz_Crater => A(nameof(Butz_Crater), "Butz Crater");
        [LocSpace]
        public F Palatine_Chasmata => A(nameof(Palatine_Chasmata), "Palatine Chasmata");
        public F Evander_Crater => A(nameof(Evander_Crater), "Evander Crater");
        [LocSpace]
        public F Tirawa_Crater => A(nameof(Tirawa_Crater), "Tirawa Crater");
        [LocSpace]
        public F Tiger_Stripes => A(nameof(Tiger_Stripes), "Tiger Stripes");
        [LocSpace]
        public F Senkyo_Ocean => A(nameof(Senkyo_Ocean), "Senkyo Ocean");
        public F Fensal_Ocean => A(nameof(Fensal_Ocean), "Fensal Ocean");
        public F Shangri_La_Ocean => A(nameof(Shangri_La_Ocean), "Shangri La Ocean");
        [LocSpace]
        public F Bona_Chasma => A(nameof(Bona_Chasma), "Bona Chasma");
        [LocSpace]
        public F Pharos_Crater => A(nameof(Pharos_Crater), "Pharos Crater");
        [LocSpace]
        public F Guttae => A(nameof(Guttae), "Guttae");
        [LocSpace]
        public F Herschel_Crater => A(nameof(Herschel_Crater), "Herschel Crater");
        [LocSpace]
        public F Sputnik_Planitia => A(nameof(Sputnik_Planitia), "Sputnik Planitia");
        [LocSpace]
        public F Serenity_Chasma => A(nameof(Serenity_Chasma), "Serenity Chasma");
        public F Mordor_Macula => A(nameof(Mordor_Macula), "Mordor Macula");
        [LocSpace]
        public F Metztli_Crater => A(nameof(Metztli_Crater), "Metztli Crater");
        [LocSpace]
        public F Zethus_Crater => A(nameof(Zethus_Crater), "Zethus Crater");
        #endregion

        #region Challenges
        [Group("Challenges")]
        
        // Specific
        public F Liftoff_Title => A(nameof(Liftoff_Title), "Liftoff");
        public F Liftoff => A(nameof(Liftoff), "Liftoff and land safely");
        [LocSpace]
        public F Reach10km_Title => A(nameof(Reach10km_Title), "Reach 10km");
        public F Reach10km => A(nameof(Reach10km), "Reach 10km and land safely");
        [LocSpace]
        public F ReachSpace_Title => A(nameof(ReachSpace_Title), "Reach Space");
        public F ReachSpace => A(nameof(ReachSpace), "Reach %height%, then survive reentry and land safely");
        [LocSpace]
        public F Land100kmDownrange_Title => A(nameof(Land100kmDownrange_Title), "Land 100km downrange");
        public F Land100kmDownrange => A(nameof(Land100kmDownrange), "Land at least 100km away from the launch pad");
        [LocSpace]
        public F ReachLowEarthOrbit_Title => A(nameof(ReachLowEarthOrbit_Title), "Reach Low Earth Orbit");
        public F ReachLowEarthOrbit => A(nameof(ReachLowEarthOrbit), "Reach low Earth orbit, then land safely");
        [LocSpace]
        public F ReachHighEarthOrbit_Title => A(nameof(ReachHighEarthOrbit_Title), "Reach High Earth Orbit");
        public F ReachHighEarthOrbit => A(nameof(ReachHighEarthOrbit), "Reach high Earth orbit, then land safely");
        [LocSpace]
        public F MoonOrbit_Title => A(nameof(MoonOrbit_Title), "Moon Orbit");
        public F MoonOrbit => A(nameof(MoonOrbit), "Capture into low Moon orbit, then return safely");
        [LocSpace]
        public F MoonTour_Title => A(nameof(MoonTour_Title), "Moon Tour");
        public F MoonTour => A(nameof(MoonTour), "Land on 3 separate landmarks, then return safely");
        [LocSpace]
        public F AsteroidImpact_Title => A(nameof(AsteroidImpact_Title), "Asteroid Impact");
        public F AsteroidImpact => A(nameof(AsteroidImpact), "Crash into the surface of the Captured Asteroid at 200+ m/s");
        [LocSpace]
        public F MarsGrandTour_Title => A(nameof(MarsGrandTour_Title), "Mars Grand Tour");
        public F MarsGrandTour => A(nameof(MarsGrandTour), "Land on Mars, Phobos and Deimos in one flight, then return safely");
        [LocSpace]
        public F VenusLanding_Title => A(nameof(VenusLanding_Title), "Venus Landing");
        public F VenusLanding => A(nameof(VenusLanding), "Descend through the thick atmosphere and land on the surface of Venus");
        [LocSpace]
        public F VenusReturn_Title => A(nameof(VenusReturn_Title), "Venus Return");
        public F VenusReturn => A(nameof(VenusReturn), "Land on the surface of Venus, then ascend trough the thick atmosphere and return safely");
        [LocSpace]
        public F MercuryLanding_Title => A(nameof(MercuryLanding_Title), "Mercury Landing");
        public F MercuryLanding => A(nameof(MercuryLanding), "Land on the surface of Mercury");
        [LocSpace]
        public F MercuryReturn_Title => A(nameof(MercuryReturn_Title), "Mercury Return");
        public F MercuryReturn => A(nameof(MercuryReturn), "Land on the surface of Mercury, then return safely");
        
        // Generic
        [LocSpace(2)]
        [Unexported] public F Planets_Rock => A(nameof(Planets_Rock), "%planet{0}% Rock");

        public const string GROUP_NAME_DYNAMIC_PLANET_SPECIFIC = "Dynamic_Planet_Specific";
        [Group(GROUP_NAME_DYNAMIC_PLANET_SPECIFIC)]
        public F LandAndReturn_Title => A(nameof(LandAndReturn_Title), "%planet{0}% Landing");
        public F LandAndReturn => A(nameof(LandAndReturn), "Land on the surface of %planet{0}%, then return safely");
        #endregion
        
        #region Achievements
        [Group("Achievements")]
        public F Reached_Karman_Line => A(nameof(Reached_Karman_Line), "Passed the Karman line, leaving the atmosphere and reaching space");
        public F Reached_Height => A(nameof(Reached_Height), "Reached %height% altitude");

        [Group(GROUP_NAME_DYNAMIC_PLANET_SPECIFIC)]
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
        public F Landed_At_Landmark => A(nameof(Landed_At_Landmark),  F.MultilineText("Landed on the surface of %planet{1}%", "<size=55>Location: %landmark%</size>"));
        public F Landed_At_Landmark__Short => A(nameof(Landed_At_Landmark__Short),  F.MultilineText("Landed on the surface of %planet{1}%", "- %landmark% -"));
        [LocSpace]
        public F Crashed_Into_Terrain => A(nameof(Crashed_Into_Terrain), "Crashed into the surface of %planet{1}%");
        [LocSpace]
        public F Entered_SOI => A(nameof(Entered_SOI), "Entered the sphere of influence of %planet{1}%");
        public F Escaped_SOI => A(nameof(Escaped_SOI), "Escaped the sphere of influence of %planet{1}%");
        [LocSpace]
        public F Docked_Suborbital => A(nameof(Docked_Suborbital), "Docked in a suborbital trajectory of %planet{1}%");
        public F Docked_Orbit_Low => A(nameof(Docked_Orbit_Low), "Docked in low %planet{0}% orbit");
        public F Docked_Orbit_Transfer => A(nameof(Docked_Orbit_Transfer), "Docked in a transfer orbit of %planet{1}%");
        public F Docked_Orbit_High => A(nameof(Docked_Orbit_High), "Docked in high %planet{0}% orbit");
        public F Docked_Escape => A(nameof(Docked_Escape), "Docked on an escape trajectory of %planet{1}%");
        public F Docked_Surface => A(nameof(Docked_Surface), "Docked on the surface of %planet{1}%");
        [LocSpace]
        [Unexported] public F EVA_Suborbital => A(nameof(EVA_Suborbital), "Performed a space walk in a suborbital trajectory of %planet{1}%");
        [Unexported] public F EVA_Orbit_Low => A(nameof(EVA_Orbit_Low), "Performed a space walk in low %planet{0}% orbit");
        [Unexported] public F EVA_Orbit_Transfer => A(nameof(EVA_Orbit_Transfer), "Performed a space walk in a transfer orbit of %planet{1}%");
        [Unexported] public F EVA_Orbit_High => A(nameof(EVA_Orbit_High), "Performed a space walk in high %planet{0}% orbit");
        [Unexported] public F EVA_Escape => A(nameof(EVA_Escape), "Performed a space walk on an escape trajectory of %planet{1}%");
        [Unexported] public F EVA_Surface => A(nameof(EVA_Surface), "Performed a space walk on the surface of %planet{1}%");
        [Unexported] public F Planted_Flag => A(nameof(Planted_Flag), "Planted a flag on the surface of %planet{1}%");
        [Unexported] public F Collected_Rock => A(nameof(Collected_Rock), "Collected a rock from the surface of %planet{1}%");
        [LocSpace]
        public F Recover_Home => A(nameof(Recover_Home), "Safely returned to %planet{1}%");
        #endregion

        #region Modloader
        [Group("Mod Loader")]
        public F Mods_Button => A(nameof(Mods_Button), "Mods");
        public F Mods_Still_Loading => A(nameof(Mods_Still_Loading), "Mods are still loading...");
        public F ModType_Label => A(nameof(ModType_Label), "Type: %type%");
        public F Version_Label => A(nameof(Version_Label), "Version: %version%");
        public F CodeMod_Name => A(nameof(CodeMod_Name), "Code Mod");
        public F PartAssetPack_Name => A(nameof(PartAssetPack_Name), "Part Assets Pack");
        public F TexturePack_Name => A(nameof(TexturePack_Name), "Texture Assets Pack");
        public F SolarSystemPack_Name => A(nameof(SolarSystemPack_Name), "Solar System Pack");
        public F CodeModNotAllowedOnMobile => A(nameof(CodeModNotAllowedOnMobile), "NOT SUPPORTED: Because of custom code execution restrictions on mobile, mods with custom code are not supported");
        public F Author_Label => A(nameof(Author_Label), "Author: %name%");
        public F Changes_Warning => A(nameof(Changes_Warning), "Would you like to relaunch the game now, so that mod changes take effect?");
        public F Relaunch => A(nameof(Relaunch), "Relaunch");
        public F Set_ModsFolder_Question => A(nameof(Set_ModsFolder_Question), "Selecting a mods folder will also move the game save data to that folder.\n" +
                                                                               "Your data will be copied over to the folder automatically.");
        public F Lost_Access_Mods => A(nameof(Lost_Access_Mods), "Access to the storage folder has been lost.\n" +
                                                                               "Reuse the same folder for saves to be accessible.\n" +
                                                                               "The old folder was at: %path%");
        public F Set_ModsFolder_Continue => A(nameof(Set_ModsFolder_Continue), "Continue");

        public F Set_ModsFolder_Confirm_Copy => A(nameof(Set_ModsFolder_Confirm_Copy), "Do you want to copy all your saves from:\n" +
                                                                                       "%oldPath%\n" +
                                                                                       "To:\n" +
                                                                                       "%newPath%\n" +
                                                                                       "This will overwrite save data on the new location and can be destructive!");
        #endregion

        // Future
        #region Astronaut
        [Group("Astronaut")]
        [LLMComment("Dismiss astronaut from service")]
        [Unexported] public F Discharge_Astronaut => A(nameof(Discharge_Astronaut), "Discharge {astronaut} ?");
        [LLMComment("Dismiss astronaut from service")]
        [Unexported] public F Discharge => A(nameof(Discharge), "Discharge");
        [Unexported] public F Invalid_Astronaut_Name => A(nameof(Invalid_Astronaut_Name), "Invalid astronaut name");
        [Unexported] public F Astronaut_Already_Exists => A(nameof(Astronaut_Already_Exists), "Astronaut already exists");
        [Unexported] public F Crew_Count => A(nameof(Crew_Count), "Crew: %count%");
        [Unexported] public F Crew_Assign => A(nameof(Crew_Assign), "Assign");
        [Unexported] public F Crew_Remove => A(nameof(Crew_Remove), "Remove");
        [Unexported] public F EVA_Exit => A(nameof(EVA_Exit), "Exit");
        [Unexported] public F EVA_Board => A(nameof(EVA_Board), "Board");
        [Unexported] public F Cannot_Board_This_Far => A(nameof(Cannot_Board_This_Far), "Cannot board from this far away");
        [LocSpace]
        [Unexported] public F Crew_AwaitingMission => A(nameof(Crew_AwaitingMission), "Awaiting mission");
        [Unexported] public F Crew_Available => A(nameof(Crew_Available), "Available");
        [Unexported] public F Crew_Assigned => A(nameof(Crew_Assigned), "Assigned");
        [Unexported] public F Crew_In_Flight => A(nameof(Crew_In_Flight), "In flight");
        [Unexported] public F Crew_In_EVA => A(nameof(Crew_In_EVA), "Performing a space walk");
        [Unexported] public F Crew_Deceased => A(nameof(Crew_Deceased), "Deceased");
        [LocSpace]
        [Unexported] public F Flag => A(nameof(Flag), "Flag");
        [Unexported] public F Confirm_Remove_Flag => A(nameof(Confirm_Remove_Flag), "Remove flag?");
        [Unexported] public F Remove_Flag => A(nameof(Remove_Flag), "Remove");
        [Unexported] public F Cannot_Plant_Flag_Here => A(nameof(Cannot_Plant_Flag_Here), "Cannot plant flag here");
        [Unexported] public F Cannot_Plant_Flag_Near_Another_Flag => A(nameof(Cannot_Plant_Flag_Near_Another_Flag), "Cannot plant flag near another one");
        [LocSpace]
        [Unexported] public F Astronaut_Fuel => A(nameof(Astronaut_Fuel), "Fuel");
        [Unexported] public F Fuel_Running_Out => A(nameof(Fuel_Running_Out), "%percent% fuel remaining");
        [Unexported] public F Out_Of_Fuel => A(nameof(Out_Of_Fuel), "Out of fuel");
        #endregion

        // Future
        #region Notifications
        [Group("Notifications")]
        public F Notify_For_New_Releases => A(nameof(Notify_For_New_Releases), "Want to be notified when a new update releases?");
        public F Notify_Me => A(nameof(Notify_Me), "Notify Me!");
        #endregion
    }

    // Utility
    public partial class SFS_Translation
    {
        public readonly Dictionary<string, F> fields = new();
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
    public static class TranslationUtility
    {
        public static F State_ToOnOff(this bool a) => a ? Loc.main.State_On : Loc.main.State_Off;
    }
}