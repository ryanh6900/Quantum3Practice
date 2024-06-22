namespace Blueless
{
  using Photon.Deterministic;
  using Quantum;
  using UnityEngine;
  using Animator = UnityEngine.Animator;

  public sealed unsafe class WeaponController : QuantumEntityViewComponent<CustomViewContext>
  {
    [System.NonSerialized] public int Ammo;
    [System.NonSerialized] public float Angle;
    public WeaponAnimationRoot AnimationRoot;

    public Animator Animator;
    public Animator CharacterAnimator;
    public IkControl Ik;
    public float X = 0.1f;

    private WeaponView[] _weapons;

    public override void OnActivate(Frame frame)
    {
      //QuantumEvent.Subscribe<EventOnWeaponShoot>(this, ShootEffect);
      QuantumEvent.Subscribe<EventOnRobotChangeWeapon>(this, ChangeWeapon);

      var weaponInventory = VerifiedFrame.Get<WeaponInventory>(EntityRef);

      _weapons = new WeaponView[weaponInventory.Weapons.Length];
      for (int i = 0; i < weaponInventory.Weapons.Length; i++)
      {
        var weaponData = QuantumUnityDB.GetGlobalAsset<WeaponData>(weaponInventory.Weapons[i].WeaponData.Id);
        var temp = Instantiate(weaponData.Prefab, transform);
        _weapons[i] = temp.GetComponent<WeaponView>();
        _weapons[i].transform.localPosition = Vector3.zero;
        _weapons[i].transform.localRotation = Quaternion.identity;
      }

      UpdateWeapon(EntityRef);
    }

    public override void OnUpdateView()
    {
      var frame = Game.Frames.Predicted;
      if (frame.Global->GameController.State == GameState.Ended)
      {
        return;
      }

      var aimDirection = GetPlayerDirection(frame);
      var zAngle = DirectionToAngle(aimDirection);
      Angle = Mathf.Rad2Deg * zAngle;

      if (frame.TryGet<WeaponInventory>(EntityRef, out var weaponInventory))
      {
        var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
        var weaponData = frame.FindAsset<WeaponData>(weapon.WeaponData.Id);
        Ammo = weapon.CurrentAmmo;
        var positionOffset = weaponData.PositionOffset;
        var finalRotation = Quaternion.Euler(Mathf.Rad2Deg * zAngle * -1, 0, 0);
        UpdateWeapon(EntityRef);

        var robotMovement = frame.Get<Movement>(EntityRef);
        if (CharacterAnimator.GetBool("IsFacingRight") == false)
        {
          finalRotation = Quaternion.Euler(180 - Mathf.Rad2Deg * zAngle * -1, 0, 0);
        }

        transform.localPosition =
          new Vector3(X, positionOffset.Y.AsFloat, positionOffset.X.AsFloat) + AnimationRoot.WeaponOffset;
        transform.localRotation = finalRotation;
      }
    }

    private FPVector2 GetPlayerDirection(Frame frame)
    {
      if (ViewContext.LocalPlayerView != null && EntityRef == ViewContext.LocalPlayerView.EntityRef)
      {
        return ViewContext.LocalPlayerLastDirection;
      }
      else
      {
        if (frame.TryGet(EntityRef, out PlayerLink playerLink))
        {
          return frame.GetPlayerInput(playerLink.PlayerRef)->AimDirection;
        }else
        {
          return FPVector2.Zero;
        }
      }
    }

    private float DirectionToAngle(FPVector2 direction)
    {
      var angle = Mathf.Atan2(direction.Y.AsFloat, direction.X.AsFloat);
      angle = Mathf.Repeat((angle + 2 * Mathf.PI), 2 * Mathf.PI);
      return angle;
    }

    private void OnDrawGizmos()
    {
      if (Application.isPlaying == false || EntityRef == null || VerifiedFrame.Exists(EntityRef) == false)
      {
        return;
      }

      var playerLink = VerifiedFrame.Get<PlayerLink>(EntityRef);
      var weaponInventory = VerifiedFrame.Get<WeaponInventory>(EntityRef);

      var angle = VerifiedFrame.GetPlayerInput(playerLink.PlayerRef)->AimDirection;
      var weapon = weaponInventory.Weapons[weaponInventory.CurrentWeaponIndex];
      var weaponData = VerifiedFrame.FindAsset<WeaponData>(weapon.WeaponData.Id);

      var fireSpotOffset = WeaponHelper.GetFireSpotWorldOffset(
        weaponData,
        angle
      );

      var weaponFireSpotPosition = transform.position + fireSpotOffset.ToUnityVector3();
      var fireDirection = angle;

      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, 0.2f);
      Gizmos.DrawWireSphere(weaponFireSpotPosition, 0.2f);
      Gizmos.DrawLine(transform.position, weaponFireSpotPosition);

      Gizmos.color = Color.blue;
      Gizmos.DrawRay(weaponFireSpotPosition, fireDirection.ToUnityVector3());
    }

    private void ChangeWeapon(EventOnRobotChangeWeapon eventData)
    {
      if (eventData.Robot.Equals(EntityRef))
      {
        Animator.SetTrigger("ChangeWeapon");
        UpdateWeapon(eventData.Robot);
      }
    }

    private void UpdateWeapon(EntityRef robot)
    {
      var currentWeaponIndex = VerifiedFrame.Get<WeaponInventory>(robot).CurrentWeaponIndex;

      if (_weapons == null)
      {
        return;
      }

      for (var i = 0; i < _weapons.Length; i++)
      {
        if (i == currentWeaponIndex)
        {
          _weapons[i].gameObject.SetActive(true);
          Ik.RightHandObj = _weapons[i].RightHand;
          Ik.LeftHandObj = _weapons[i].LeftHand;
          Ik.LookObj = _weapons[i].LookDir;
        }
        else
        {
          _weapons[i].gameObject.SetActive(false);
        }
      }
    }

    private void OnDestroy()
    {
      QuantumEvent.UnsubscribeListener(this);
    }

    private void ShootEffect(EventOnWeaponShoot eventData)
    {
      if (eventData.Robot.Equals(EntityRef))
      {
        var robotInventoty = PredictedFrame.Get<WeaponInventory>(eventData.Robot);
        _weapons[robotInventoty.CurrentWeaponIndex].ShootFx();
      }
    }
  }
}