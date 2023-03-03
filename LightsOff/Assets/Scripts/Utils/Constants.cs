using UnityEngine;

public static class Constants
{
    public const int StartingLevelNumber = 1;

    // Game object/Scene names and prefixes
    public const string NameSceneBoot = "Boot";
    public const string NamePrefixSceneLevel = "Level_";
    public const string NamePrefixSceneTest = "Test_";
    public const string NameSceneStartMenu = "StartMenu";
    public const string NameGameObjectLevelLoader = "LevelLoader";
    public const string NameGameObjectLevelBoundaries = "LevelBoundaries";

    // Tags
    public const string TagPlayer = "Player";
    public const string TagFunctionalLevel = "FunctionalLevel";
    public const string TagLevelBoundaries = "LevelBoundaries";
    public const string TagSpawnPoints = "SpawnPoints";
    public const string TagSpawnPointEnemyDrunk = "SpawnPointEnemyDrunk";
    public const string TagSpawnPointEnemyPassedOut = "SpawnPointEnemyPassedOut";
    public const string TagLight = "Light";
    public const string TagStartDoor = "StartDoor";
    public const string TagDoor = "Door";
    public const string TagKey = "Key";
    public const string TagKeygate = "Keygate";
    public const string TagLadder = "Ladder";
    public const string TagLadderTop = "LadderTop";
    public const string TagLadderBottom = "LadderBottom";
    public const string TagFunctionalFan = "FunctionalFan";
    public const string TagEnemy = "Enemy";
    public const string TagPivotingGate = "PivotingGate";
    public const string TagConveyor = "Conveyor";
    public const string TagOutline = "Outline";
    public const string TagHaloLight = "HaloLight";
    public const string TagDirectionalLight = "DirectionalLight";
    public const string TagWindow = "Window";
    public const string TagOneWayPlatformTypeB = "OWPB";
    public const string TagGate = "Gate";

    // Layers
    public const string LayerPlayer = "Player";
    public const string LayerInteractibleObject = "InteractibleObject";
    public const string LayerGround = "Ground";
    public const string LayerGroundForPlayer = "GroundForPlayer";
    public const string LayerColliderForEnemies = "ColliderForEnemies";

    // Animator parameters/Animations
    public const string AnimatorCharacterIsJumping = "IsJumping";
    public const string AnimatorCharacterIsFalling = "IsFalling";
    public const string AnimatorCharacterIsClimbing = "IsClimbing";
    public const string AnimatorCharacterIsDying = "IsDying";
    public const string AnimatorCharacterIsExitingLevel = "IsExitingLevel";
    public const string AnimatorCharacterIsEnteringLevel = "IsEnteringLevel";
    public const string AnimatorCharacterSpeed = "Speed";
    public const string AnimatorEnemyIsChasing = "IsChasing";
    public const string AnimatorEnemyBouncedOn = "BouncedOn";
    public const string AnimatorEnemyCurrentStateTimer = "CurrentStateTimer";
    public const string AnimatorDoorIsOpen = "IsOpen";
    public const string AnimatorGateIsOpen = "IsOpen";
    public const string AnimatorBedWakingUp = "WakingUp";
    public const string AnimatorCrossfadeStart = "CrossfadeStart";
    public const string AnimatorCrossfadeEnd = "CrossfadeEnd";
    public const string AnimationCharacterClimb = "CharacterClimb";
    public const string AnimationCharacterDeath = "CharacterDeath";
    public const string AnimationKeygateOpen = "KeygateOpen";
    public const string AnimationWindowSunrise = "WindowSunrise";

    // Input
    public const string InputActionMapGameplay = "Gameplay";
    public const string InputActionMapUI = "UI";
    public const string InputActionMapCreditsEnd = "CreditsEnd";
    public const string InputControlSchemeKeyboardMouse = "Keyboard/Mouse";
    public const string InputControlSchemeGamepad = "Gamepad";

    // UI
    public const string UILevelNumberText = "Level ";
    public const string UILightKeyNumberText = "x";
    public const string UIInteractMessagePress = "Press";
    public const string UIInteractMessageEnter = "Enter";
    public const string UIInteractMessageSleep = "Sleep";
    public const string UISaveFileDeletedMessage = "Save file deleted.";
    public const string UINoSaveFileFoundMessage = "No save file found.";


    // ***** WWISE *****

    // Music
    public const string WwiseEventMusicStart = "Music_Start";
    public const string WwiseEventMusicPlayGame = "Music_PlayGame";
    public const string WwiseEventMusicBackToMenu = "Music_BackToMenu";

    // Player
    public const string WwiseEventPlayPlayerBounce = "Play_PlayerBounce";
    public const string WwiseEventPlayPlayerBunk = "Play_PlayerBunk";
    public const string WwiseEventPlayPlayerClimb = "Play_PlayerClimb";
    public const string WwiseEventPlayPlayerDeath = "Play_PlayerDeath";
    public const string WwiseEventPlayPlayerEnterLevel = "Play_PlayerEnterLevel";
    public const string WwiseEventPlayPlayerExitLevel = "Play_PlayerExitLevel";
    public const string WwiseEventPlayPlayerJump = "Play_PlayerJump";
    public const string WwiseEventPlayPlayerLand = "Play_PlayerLand";
    public const string WwiseEventPlayPlayerWalk = "Play_PlayerWalk";
    public const string WwiseEventStopPlayerClimb = "Stop_PlayerClimb";
    public const string WwiseEventStopPlayerWalk = "Stop_PlayerWalk";

    // Enemies
    public const string WwiseEventPlayEnemyLand = "Play_EnemyLand";
    public const string WwiseEventPlayEnemyPassOut = "Play_EnemyPassOut";
    public const string WwiseEventPlayEnemyStartChasing = "Play_EnemyStartChasing";
    public const string WwiseEventPlayEnemyStopChasing = "Play_EnemyStopChasing";
    public const string WwiseEventPlayEnemyStunned = "Play_EnemyStunned";
    public const string WwiseEventPlayEnemyWakeUp = "Play_EnemyWakeUp";
    public const string WwiseEventPlayEnemyWander = "Play_EnemyWander";
    public const string WwiseEventStopEnemyPassOut = "Stop_EnemyPassOut";
    public const string WwiseEventStopEnemyWander = "Stop_EnemyWander";
    public const string WwiseEventStopEnemyChasing = "Stop_EnemyChasing";

    // Devices
    public const string WwiseEventPlaySwitchOn = "Play_SwitchOn";
    public const string WwiseEventPlaySwitchOff = "Play_SwitchOff";
    public const string WwiseEventPlayTimerSwitch = "Play_TimerSwitch";
    public const string WwiseEventStopTimerSwitch = "Stop_TimerSwitch";
    public const string WwiseEventPlayTimerSwitchEnds = "Play_TimerSwitchEnds";
    public const string WwiseEventPlayCycleSwitch = "Play_CycleSwitch";
    public const string WwiseEventPlayBreakerOn = "Play_BreakerOn";
    public const string WwiseEventPlayBreakerOff = "Play_BreakerOff";
    public const string WwiseEventPlayDoorOpens = "Play_DoorOpens";
    public const string WwiseEventPlayDoorCloses = "Play_DoorCloses";
    public const string WwiseEventPlayLadderMoves = "Play_LadderMoves";
    public const string WwiseEventPlayGateMoves = "Play_GateMoves";
    public const string WwiseEventPlayKeygateOpens = "Play_KeygateOpens";
    public const string WwiseEventPlayKeyCollect = "Play_KeyCollect";
    public const string WwiseEventPlayKeyLands = "Play_KeyLands";
    public const string WwiseEventPlayConveyor = "Play_Conveyor";
    public const string WwiseEventStopConveyor = "Stop_Conveyor";
    public const string WwiseEventPlayFan = "Play_Fan";
    public const string WwiseEventStopFan = "Stop_Fan";
    public const string WwiseEventPlayMovingPlatformTypeA = "Play_MovingPlatformTypeA";
    public const string WwiseEventStopMovingPlatformTypeA = "Stop_MovingPlatformTypeA";
    public const string WwiseEventPlayMovingPlatformTypeB = "Play_MovingPlatformTypeB";
    public const string WwiseEventStopMovingPlatformTypeB = "Stop_MovingPlatformTypeB";
    public const string WwiseEventPlayOWBOn = "Play_OWBOn";
    public const string WwiseEventPlayOWBOff = "Play_OWBOff";
    public const string WwiseEventPlayPivotingGateRotates = "Play_PivotingGateRotates";
    public const string WwiseEventPlayPivotingGateBlocks = "Play_PivotingGateBlocks";

    // UI
    public const string WwiseEventPlayMenuPlay = "Play_MenuPlay";
    public const string WwiseEventPlayMenuQuit = "Play_MenuQuit";
    public const string WwiseEventPlayMenuButtonPress = "Play_MenuButtonPress";
    public const string WwiseEventPlayMenuButtonBackPress = "Play_MenuButtonBackPress";
    public const string WwiseEventPlayMenuDeleteSaveFileButtonPress = "Play_MenuDeleteSaveFileButtonPress";
    public const string WwiseEventPlayMenuSelectionMove = "Play_MenuSelectionMove";
    public const string WwiseEventPlayMenuChangeSetting = "Play_MenuChangeSetting";
    public const string WwiseEventPlayPauseMenuClose = "Play_PauseMenuClose";
    public const string WwiseEventPlayPauseMenuOpen = "Play_PauseMenuOpen";
    public const string WwiseEventPlayPauseMenuRetry = "Play_PauseMenuRetry";

    // Ambiances
    public const string WwiseEventPlayCreditsAmbiance = "Play_CreditsAmbiance";
    public const string WwiseEventStopCreditsAmbiance = "Stop_CreditsAmbiance";

    // Transitions
    public const string WwiseEventFadeOutForLevelTransition = "FadeOutForLevelTransition";
    public const string WwiseEventFadeInForLevelTransition = "FadeInForLevelTransition";
    public const string WwiseEventFadeOutForPauseMenu = "FadeOutForPauseMenu";
    public const string WwiseEventFadeInForPauseMenu = "FadeInForPauseMenu";
    public const string WwiseEventFadeOutForQuitTransition = "FadeOutForQuitTransition";
    public const string WwiseEventFadeInToMenu = "FadeInToMenu";
    public const string WwiseEventFadeOutForCredits = "FadeOutForCredits";
    public const string WwiseEventFadeInCreditsAmbiance = "FadeInCreditsAmbiance";
    public const string WwiseEventFadeOutCreditsAmbiance = "FadeOutCreditsAmbiance";
    public const string WwiseEventSetTransitionSFXVolumeToMax = "SetTransitionSFXVolumeToMax";

    // RTPC
    public const string WwiseRTPCMasterVolume = "MasterVolume";
    public const string WwiseRTPCMusicVolume = "MusicVolume";
    public const string WwiseRTPCSFXVolume = "SFXVolume";
    public const string WwiseRTPCUIVolume = "UIVolume";
}
