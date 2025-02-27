// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial
// declarations in another file.
// </auto-generated>
#pragma warning disable 0109
#pragma warning disable 1591


namespace Quantum.Prototypes {
  using Photon.Deterministic;
  using Quantum;
  using Quantum.Core;
  using Quantum.Collections;
  using Quantum.Inspector;
  using Quantum.Physics2D;
  using Quantum.Physics3D;
  using Byte = System.Byte;
  using SByte = System.SByte;
  using Int16 = System.Int16;
  using UInt16 = System.UInt16;
  using Int32 = System.Int32;
  using UInt32 = System.UInt32;
  using Int64 = System.Int64;
  using UInt64 = System.UInt64;
  using Boolean = System.Boolean;
  using String = System.String;
  using Object = System.Object;
  using FlagsAttribute = System.FlagsAttribute;
  using SerializableAttribute = System.SerializableAttribute;
  using MethodImplAttribute = System.Runtime.CompilerServices.MethodImplAttribute;
  using MethodImplOptions = System.Runtime.CompilerServices.MethodImplOptions;
  using FieldOffsetAttribute = System.Runtime.InteropServices.FieldOffsetAttribute;
  using StructLayoutAttribute = System.Runtime.InteropServices.StructLayoutAttribute;
  using LayoutKind = System.Runtime.InteropServices.LayoutKind;
  #if QUANTUM_UNITY //;
  using TooltipAttribute = UnityEngine.TooltipAttribute;
  using HeaderAttribute = UnityEngine.HeaderAttribute;
  using SpaceAttribute = UnityEngine.SpaceAttribute;
  using RangeAttribute = UnityEngine.RangeAttribute;
  using HideInInspectorAttribute = UnityEngine.HideInInspector;
  using PreserveAttribute = UnityEngine.Scripting.PreserveAttribute;
  using FormerlySerializedAsAttribute = UnityEngine.Serialization.FormerlySerializedAsAttribute;
  using MovedFromAttribute = UnityEngine.Scripting.APIUpdating.MovedFromAttribute;
  using CreateAssetMenu = UnityEngine.CreateAssetMenuAttribute;
  using RuntimeInitializeOnLoadMethodAttribute = UnityEngine.RuntimeInitializeOnLoadMethodAttribute;
  #endif //;
  
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.BulletFields))]
  public unsafe class BulletFieldsPrototype : ComponentPrototype<Quantum.BulletFields> {
    public FP Time;
    public MapEntityId Source;
    public FPVector2 Direction;
    public AssetRef<BulletData> BulletData;
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.BulletFields component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.BulletFields result, in PrototypeMaterializationContext context = default) {
        result.Time = this.Time;
        PrototypeValidator.FindMapEntity(this.Source, in context, out result.Source);
        result.Direction = this.Direction;
        result.BulletData = this.BulletData;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.GameController))]
  public unsafe partial class GameControllerPrototype : StructPrototype {
    public Quantum.QEnum32<GameState> State;
    public FP GameTimer;
    partial void MaterializeUser(Frame frame, ref Quantum.GameController result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.GameController result, in PrototypeMaterializationContext context = default) {
        result.State = this.State;
        result.GameTimer = this.GameTimer;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Input))]
  public unsafe partial class InputPrototype : StructPrototype {
    public Button Fire;
    public Button Jump;
    public Button CastSkill;
    public Button ChangeWeapon;
    public SByte Movement;
    public Byte EncodedAimDirection;
    partial void MaterializeUser(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.Input result, in PrototypeMaterializationContext context = default) {
        result.Fire = this.Fire;
        result.Jump = this.Jump;
        result.CastSkill = this.CastSkill;
        result.ChangeWeapon = this.ChangeWeapon;
        result.Movement = this.Movement;
        result.EncodedAimDirection = this.EncodedAimDirection;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Movement))]
  public unsafe partial class MovementPrototype : ComponentPrototype<Quantum.Movement> {
    public FP GroundedFramesCount;
    public FP UngroundedFramesCount;
    public QBoolean VirtualGrounded;
    public QBoolean PrevGrounded;
    public QBoolean CanDoubleJump;
    public QBoolean IsFacingRight;
    public FP JumpDelayTimer;
    public AssetRef<MovementData> MovementData;
    partial void MaterializeUser(Frame frame, ref Quantum.Movement result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Movement component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Movement result, in PrototypeMaterializationContext context = default) {
        result.GroundedFramesCount = this.GroundedFramesCount;
        result.UngroundedFramesCount = this.UngroundedFramesCount;
        result.VirtualGrounded = this.VirtualGrounded;
        result.PrevGrounded = this.PrevGrounded;
        result.CanDoubleJump = this.CanDoubleJump;
        result.IsFacingRight = this.IsFacingRight;
        result.JumpDelayTimer = this.JumpDelayTimer;
        result.MovementData = this.MovementData;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.PlayerLink))]
  public unsafe partial class PlayerLinkPrototype : ComponentPrototype<Quantum.PlayerLink> {
    public PlayerRef PlayerRef;
    partial void MaterializeUser(Frame frame, ref Quantum.PlayerLink result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.PlayerLink component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.PlayerLink result, in PrototypeMaterializationContext context = default) {
        result.PlayerRef = this.PlayerRef;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Score))]
  public unsafe partial class ScorePrototype : ComponentPrototype<Quantum.Score> {
    public Int32 Kills;
    public Int32 Deaths;
    partial void MaterializeUser(Frame frame, ref Quantum.Score result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Score component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Score result, in PrototypeMaterializationContext context = default) {
        result.Kills = this.Kills;
        result.Deaths = this.Deaths;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.SkillFields))]
  public unsafe class SkillFieldsPrototype : ComponentPrototype<Quantum.SkillFields> {
    public FP TimeToActivate;
    public MapEntityId Source;
    public AssetRef<SkillData> SkillData;
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.SkillFields component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.SkillFields result, in PrototypeMaterializationContext context = default) {
        result.TimeToActivate = this.TimeToActivate;
        PrototypeValidator.FindMapEntity(this.Source, in context, out result.Source);
        result.SkillData = this.SkillData;
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.SkillInventory))]
  public unsafe partial class SkillInventoryPrototype : ComponentPrototype<Quantum.SkillInventory> {
    public FP CastRateTimer;
    public AssetRef<SkillInventoryData> SkillInventoryData;
    partial void MaterializeUser(Frame frame, ref Quantum.SkillInventory result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.SkillInventory component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.SkillInventory result, in PrototypeMaterializationContext context = default) {
        result.CastRateTimer = this.CastRateTimer;
        result.SkillInventoryData = this.SkillInventoryData;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.SpawnIdentifier))]
  public unsafe partial class SpawnIdentifierPrototype : ComponentPrototype<Quantum.SpawnIdentifier> {
    [HideInInspector()]
    public Int32 _empty_prototype_dummy_field_;
    partial void MaterializeUser(Frame frame, ref Quantum.SpawnIdentifier result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.SpawnIdentifier component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.SpawnIdentifier result, in PrototypeMaterializationContext context = default) {
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Status))]
  public unsafe partial class StatusPrototype : ComponentPrototype<Quantum.Status> {
    public FP CurrentHealth;
    public QBoolean IsDead;
    public FP RespawnTimer;
    public FP RegenTimer;
    public FP InvincibleTimer;
    public Int32 DisconnectedTicks;
    public AssetRef<StatusData> StatusData;
    partial void MaterializeUser(Frame frame, ref Quantum.Status result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.Status component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.Status result, in PrototypeMaterializationContext context = default) {
        result.CurrentHealth = this.CurrentHealth;
        result.IsDead = this.IsDead;
        result.RespawnTimer = this.RespawnTimer;
        result.RegenTimer = this.RegenTimer;
        result.InvincibleTimer = this.InvincibleTimer;
        result.DisconnectedTicks = this.DisconnectedTicks;
        result.StatusData = this.StatusData;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.Weapon))]
  public unsafe partial class WeaponPrototype : StructPrototype {
    public QBoolean IsRecharging;
    public Int32 CurrentAmmo;
    public FP FireRateTimer;
    public FP DelayToStartRechargeTimer;
    public FP RechargeRate;
    public FP ChargeTime;
    public AssetRef<WeaponData> WeaponData;
    partial void MaterializeUser(Frame frame, ref Quantum.Weapon result, in PrototypeMaterializationContext context);
    public void Materialize(Frame frame, ref Quantum.Weapon result, in PrototypeMaterializationContext context = default) {
        result.IsRecharging = this.IsRecharging;
        result.CurrentAmmo = this.CurrentAmmo;
        result.FireRateTimer = this.FireRateTimer;
        result.DelayToStartRechargeTimer = this.DelayToStartRechargeTimer;
        result.RechargeRate = this.RechargeRate;
        result.ChargeTime = this.ChargeTime;
        result.WeaponData = this.WeaponData;
        MaterializeUser(frame, ref result, in context);
    }
  }
  [System.SerializableAttribute()]
  [Quantum.Prototypes.Prototype(typeof(Quantum.WeaponInventory))]
  public unsafe partial class WeaponInventoryPrototype : ComponentPrototype<Quantum.WeaponInventory> {
    public Int32 CurrentWeaponIndex;
    [ArrayLengthAttribute(2)]
    public Quantum.Prototypes.WeaponPrototype[] Weapons = new Quantum.Prototypes.WeaponPrototype[2];
    partial void MaterializeUser(Frame frame, ref Quantum.WeaponInventory result, in PrototypeMaterializationContext context);
    public override Boolean AddToEntity(FrameBase f, EntityRef entity, in PrototypeMaterializationContext context) {
        Quantum.WeaponInventory component = default;
        Materialize((Frame)f, ref component, in context);
        return f.Set(entity, component) == SetResult.ComponentAdded;
    }
    public void Materialize(Frame frame, ref Quantum.WeaponInventory result, in PrototypeMaterializationContext context = default) {
        result.CurrentWeaponIndex = this.CurrentWeaponIndex;
        for (int i = 0, count = PrototypeValidator.CheckLength(Weapons, 2, in context); i < count; ++i) {
          this.Weapons[i].Materialize(frame, ref *result.Weapons.GetPointer(i), in context);
        }
        MaterializeUser(frame, ref result, in context);
    }
  }
}
#pragma warning restore 0109
#pragma warning restore 1591
