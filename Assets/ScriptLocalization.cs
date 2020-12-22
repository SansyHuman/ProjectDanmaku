using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static class Generic
		{
			public static string No 		{ get{ return LocalizationManager.GetTranslation ("Generic/No"); } }
			public static string WarningExitGame 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningExitGame"); } }
			public static string WarningFont 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningFont"); } }
			public static string WarningReturnToTitle 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningReturnToTitle"); } }
			public static string Yes 		{ get{ return LocalizationManager.GetTranslation ("Generic/Yes"); } }
			public static string WarningRestart 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningRestart"); } }
		}

		public static class HUD
		{
			public static string Graze 		{ get{ return LocalizationManager.GetTranslation ("HUD/Graze"); } }
			public static string HUDFont 		{ get{ return LocalizationManager.GetTranslation ("HUD/HUDFont"); } }
			public static string Health 		{ get{ return LocalizationManager.GetTranslation ("HUD/Health"); } }
			public static string Power 		{ get{ return LocalizationManager.GetTranslation ("HUD/Power"); } }
			public static string Score 		{ get{ return LocalizationManager.GetTranslation ("HUD/Score"); } }
		}

		public static class Pause
		{
			public static string ExitGame 		{ get{ return LocalizationManager.GetTranslation ("Pause/ExitGame"); } }
			public static string PauseEntryFont 		{ get{ return LocalizationManager.GetTranslation ("Pause/PauseEntryFont"); } }
			public static string PauseMenuFont 		{ get{ return LocalizationManager.GetTranslation ("Pause/PauseMenuFont"); } }
			public static string RestartStage 		{ get{ return LocalizationManager.GetTranslation ("Pause/RestartStage"); } }
			public static string Resume 		{ get{ return LocalizationManager.GetTranslation ("Pause/Resume"); } }
			public static string ReturnToTitle 		{ get{ return LocalizationManager.GetTranslation ("Pause/ReturnToTitle"); } }
			public static string Settings 		{ get{ return LocalizationManager.GetTranslation ("Pause/Settings"); } }
		}

		public static class Settings
		{
			public static string SettingsFont 		{ get{ return LocalizationManager.GetTranslation ("Settings/SettingsFont"); } }
		}

		public static class Settings_Graphics
		{
			public static string Resolution 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics/Resolution"); } }
			public static string ScreenMode 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics/ScreenMode"); } }
			public static string TripleBuffering 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics/TripleBuffering"); } }
			public static string VerticalSync 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics/VerticalSync"); } }
		}

		public static class Settings_Graphics_ScreenMode
		{
			public static string ExclusiveFullScreen 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics/ScreenMode/ExclusiveFullScreen"); } }
			public static string FullScreenWindow 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics/ScreenMode/FullScreenWindow"); } }
			public static string Windowed 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics/ScreenMode/Windowed"); } }
		}
	}

    public static class ScriptTerms
	{

		public static class Generic
		{
		    public const string No = "Generic/No";
		    public const string WarningExitGame = "Generic/WarningExitGame";
		    public const string WarningFont = "Generic/WarningFont";
		    public const string WarningReturnToTitle = "Generic/WarningReturnToTitle";
		    public const string Yes = "Generic/Yes";
		    public const string WarningRestart = "Generic/WarningRestart";
		}

		public static class HUD
		{
		    public const string Graze = "HUD/Graze";
		    public const string HUDFont = "HUD/HUDFont";
		    public const string Health = "HUD/Health";
		    public const string Power = "HUD/Power";
		    public const string Score = "HUD/Score";
		}

		public static class Pause
		{
		    public const string ExitGame = "Pause/ExitGame";
		    public const string PauseEntryFont = "Pause/PauseEntryFont";
		    public const string PauseMenuFont = "Pause/PauseMenuFont";
		    public const string RestartStage = "Pause/RestartStage";
		    public const string Resume = "Pause/Resume";
		    public const string ReturnToTitle = "Pause/ReturnToTitle";
		    public const string Settings = "Pause/Settings";
		}

		public static class Settings
		{
		    public const string SettingsFont = "Settings/SettingsFont";
		}

		public static class Settings_Graphics
		{
		    public const string Resolution = "Settings/Graphics/Resolution";
		    public const string ScreenMode = "Settings/Graphics/ScreenMode";
		    public const string TripleBuffering = "Settings/Graphics/TripleBuffering";
		    public const string VerticalSync = "Settings/Graphics/VerticalSync";
		}

		public static class Settings_Graphics_ScreenMode
		{
		    public const string ExclusiveFullScreen = "Settings/Graphics/ScreenMode/ExclusiveFullScreen";
		    public const string FullScreenWindow = "Settings/Graphics/ScreenMode/FullScreenWindow";
		    public const string Windowed = "Settings/Graphics/ScreenMode/Windowed";
		}
	}
}