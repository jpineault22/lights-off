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

    // Layers
    public const string LayerPlayer = "Player";
    public const string LayerInteractibleObject = "InteractibleObject";
    public const string LayerGround = "Ground";
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
    public const string UIKeyNumberText = "x";
    public const string UIInteractMessagePress = "Press";
    public const string UIInteractMessageEnter = "Enter";
    public const string UIInteractMessageSleep = "Sleep";

    // Audio mixer parameters
    public const string AudioMasterVolume = "MasterVolume";

    // Wwise
    public const string WwiseEventPlayPlayerWalk = "Play_PlayerWalk";
    public const string WwiseEventStopPlayerWalk = "Stop_PlayerWalk";
    public const string WwiseEventPlayPlayerJump = "Play_PlayerJump";
}
