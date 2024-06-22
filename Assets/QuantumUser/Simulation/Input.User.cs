namespace Quantum
{
  using System;
  using Photon.Deterministic;

  partial struct Input
  {
    public FPVector2 AimDirection
    {
      get { return DecodeDirection(EncodedAimDirection); }
      set { EncodedAimDirection = EncodeDirection(value); }
    }

    private FPVector2 DecodeDirection(byte encodedDirection)
    {
      if (encodedDirection == default) return default;
      Int32 angle = ((Int32)encodedDirection - 1) * 2;
      return FPVector2.Rotate(FPVector2.Up, angle * FP.Deg2Rad);
    }
    
    private byte EncodeDirection(FPVector2 value)
    {
      if (value == default)
      {
        return default;
      }
      var angle = FPVector2.RadiansSigned(FPVector2.Up, value) * FP.Rad2Deg;
      angle = (((angle + 360) % 360) / 2) + 1;
      return (Byte)(angle.AsInt);
    }
  }
}