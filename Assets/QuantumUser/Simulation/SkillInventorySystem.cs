namespace Quantum
{
  using Photon.Deterministic;

  /// <summary>
  ///   Handles input logic and creation of entities for the Skills
  /// </summary>
  public unsafe class SkillInventorySystem : SystemMainThreadFilter<SkillInventorySystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public PlayerLink* PlayerLink;
      public Transform2D* Transform;
      public SkillInventory* SkillInventory;
      public Status* Status;
    }

    public override void Update(Frame frame, ref Filter filter)
    {
      var robot = filter.Entity;
      var robotTransform = filter.Transform;
      var playerLink = filter.PlayerLink;
      var status = filter.Status;
      var robotSkillInventory = filter.SkillInventory;

      if (status->IsDead)
      {
        return;
      }

      Input* input = frame.GetPlayerInput(playerLink->PlayerRef);

      if (robotSkillInventory->CastRateTimer <= FP._0)
      {
        if (input->CastSkill.WasPressed)
        {
          CastSkill(frame, robot, robotTransform, robotSkillInventory, input->AimDirection);
        }
      }
      else
      {
        robotSkillInventory->CastRateTimer -= frame.DeltaTime;
      }
    }

    /// <summary>
    /// Creates a new Skill Entity in the world and setup it's values
    /// </summary>
    /// <param name="frame"></param>
    /// <param name="robot"></param>
    /// <param name="angle"></param>
    private void CastSkill(Frame frame, EntityRef robot, Transform2D* robotTransform, SkillInventory* skillInventory,  FPVector2 direction)
    {
      SkillInventoryData skillInventoryData =
        frame.FindAsset<SkillInventoryData>(skillInventory->SkillInventoryData.Id);
      SkillData skillData = frame.FindAsset<SkillData>(skillInventoryData.SkillData.Id);

      EntityPrototype skillPrototype = frame.FindAsset<EntityPrototype>(skillData.SkillPrototype);
      EntityRef skill = frame.Create(skillPrototype);

      SkillFields* skillFields = frame.Unsafe.GetPointer<SkillFields>(skill);

      skillFields->SkillData = skillData;
      skillFields->Source = robot;
      skillFields->TimeToActivate = skillData.ActivationDelay;

      Transform2D* skillTransform = frame.Unsafe.GetPointer<Transform2D>(skill);
      skillTransform->Position = robotTransform->Position;

      PhysicsBody2D* skillPhysics = frame.Unsafe.GetPointer<PhysicsBody2D>(skill);

      skillPhysics->Velocity = direction * skillInventoryData.CastForce;
      skillInventory->CastRateTimer = skillInventoryData.CastRate;

      frame.Events.OnSkillCasted(skill);
    }
  }
}