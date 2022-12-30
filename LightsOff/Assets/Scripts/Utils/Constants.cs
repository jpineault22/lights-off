using UnityEngine;

public static class Constants
{
    public const int StartingLevelNumber = 1;

    // Game object/Scene names and prefixes
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
    public const string TagFirstPlayerSpawnPoint = "FirstPlayerSpawnPoint";
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
    public const string AnimatorCharacterIsLevelTransitioning = "IsLevelTransitioning";
    public const string AnimatorCharacterSpeed = "Speed";
    public const string AnimatorCrossfadeStart = "CrossfadeStart";
    public const string AnimatorCrossfadeEnd = "CrossfadeEnd";
    public const string AnimationCharacterClimb = "CharacterClimb";
    public const string AnimationCharacterDeath = "CharacterDeath";

    // Audio mixer parameters
    public const string AudioMasterVolume = "MasterVolume";

    // Input
    public const string InputActionMapGameplay = "Gameplay";
    public const string InputActionMapUI = "UI";
    public const string InputControlSchemeKeyboardMouse = "Keyboard/Mouse";
    public const string InputControlSchemeGamepad = "Gamepad";
    public const string InputInteractKeyboardMouse = "K";
    public const string InputInteractGamepad = "X";

    // UI
    public const string UILevelNumberText = "Level ";
    public const string UIInteractMessagePress = "Press";
    public const string UIInteractMessageEnter = "Enter";
}
