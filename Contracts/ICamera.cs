/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     Contracts
 * FILE:        ICamera.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

namespace Contracts;

public interface ICamera
{
    int X { get; }
    int Y { get; }
    int Z { get; }
    int Angle { get; }
    int Pitch { get; }
    float ZFar { get; }
    string ToString();
}