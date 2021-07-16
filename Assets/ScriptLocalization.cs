using UnityEngine;

namespace I2.Loc
{
	public static class ScriptLocalization
	{

		public static class Generic
		{
			public static string License 		{ get{ return LocalizationManager.GetTranslation ("Generic/License"); } }
			public static string No 		{ get{ return LocalizationManager.GetTranslation ("Generic/No"); } }
			public static string WarningExitGame 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningExitGame"); } }
			public static string WarningFont 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningFont"); } }
			public static string WarningRestart 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningRestart"); } }
			public static string WarningReturnToTitle 		{ get{ return LocalizationManager.GetTranslation ("Generic/WarningReturnToTitle"); } }
			public static string Yes 		{ get{ return LocalizationManager.GetTranslation ("Generic/Yes"); } }
		}

		public static class HUD
		{
			public static string Graze 		{ get{ return LocalizationManager.GetTranslation ("HUD/Graze"); } }
			public static string HUDFont 		{ get{ return LocalizationManager.GetTranslation ("HUD/HUDFont"); } }
			public static string Health 		{ get{ return LocalizationManager.GetTranslation ("HUD/Health"); } }
			public static string Mana 		{ get{ return LocalizationManager.GetTranslation ("HUD/Mana"); } }
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
			public static string Gamepad 		{ get{ return LocalizationManager.GetTranslation ("Settings/Gamepad"); } }
			public static string Graphics 		{ get{ return LocalizationManager.GetTranslation ("Settings/Graphics"); } }
			public static string Keyboard 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard"); } }
			public static string Language 		{ get{ return LocalizationManager.GetTranslation ("Settings/Language"); } }
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

		public static class Settings_Keyboard
		{
			public static string ReadingKey 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/ReadingKey"); } }
			public static string MoveDown 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/MoveDown"); } }
			public static string MoveLeft 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/MoveLeft"); } }
			public static string MoveRight 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/MoveRight"); } }
			public static string MoveUp 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/MoveUp"); } }
			public static string Shoot 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/Shoot"); } }
			public static string Skill1 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/Skill1"); } }
			public static string Skill2 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/Skill2"); } }
			public static string Skill3 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/Skill3"); } }
			public static string SlowMode 		{ get{ return LocalizationManager.GetTranslation ("Settings/Keyboard/SlowMode"); } }
		}

		public static class Settings_Language
		{
			public static string English 		{ get{ return LocalizationManager.GetTranslation ("Settings/Language/English"); } }
			public static string Korean 		{ get{ return LocalizationManager.GetTranslation ("Settings/Language/Korean"); } }
		}

		public static class Skill
		{
			public static string Shield 		{ get{ return LocalizationManager.GetTranslation ("Skill/Shield"); } }
			public static string ShieldDescription 		{ get{ return LocalizationManager.GetTranslation ("Skill/ShieldDescription"); } }
		}
	}

    public static class ScriptTerms
	{

		public static class Generic
		{
		    public const string License = "Generic/License";
		    public const string No = "Generic/No";
		    public const string WarningExitGame = "Generic/WarningExitGame";
		    public const string WarningFont = "Generic/WarningFont";
		    public const string WarningRestart = "Generic/WarningRestart";
		    public const string WarningReturnToTitle = "Generic/WarningReturnToTitle";
		    public const string Yes = "Generic/Yes";
		}

		public static class HUD
		{
		    public const string Graze = "HUD/Graze";
		    public const string HUDFont = "HUD/HUDFont";
		    public const string Health = "HUD/Health";
		    public const string Mana = "HUD/Mana";
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
		    public const string Gamepad = "Settings/Gamepad";
		    public const string Graphics = "Settings/Graphics";
		    public const string Keyboard = "Settings/Keyboard";
		    public const string Language = "Settings/Language";
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

		public static class Settings_Keyboard
		{
		    public const string ReadingKey = "Settings/Keyboard/ReadingKey";
		    public const string MoveDown = "Settings/Keyboard/MoveDown";
		    public const string MoveLeft = "Settings/Keyboard/MoveLeft";
		    public const string MoveRight = "Settings/Keyboard/MoveRight";
		    public const string MoveUp = "Settings/Keyboard/MoveUp";
		    public const string Shoot = "Settings/Keyboard/Shoot";
		    public const string Skill1 = "Settings/Keyboard/Skill1";
		    public const string Skill2 = "Settings/Keyboard/Skill2";
		    public const string Skill3 = "Settings/Keyboard/Skill3";
		    public const string SlowMode = "Settings/Keyboard/SlowMode";
		}

		public static class Settings_Language
		{
		    public const string English = "Settings/Language/English";
		    public const string Korean = "Settings/Language/Korean";
		}

		public static class Skill
		{
		    public const string Shield = "Skill/Shield";
		    public const string ShieldDescription = "Skill/ShieldDescription";
		}
	}
}