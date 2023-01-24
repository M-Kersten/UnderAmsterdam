using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BezierPoint
{
    public Vector3 pos;
    public Quaternion rot;
    
    public BezierPoint (Vector3 pos, Vector3 forward)
    {
        this.pos = pos;
        this.rot = Quaternion.LookRotation(forward);
    }

    public Vector3 LocalToWorld(Vector3 localSpacePos)
    {
        return pos + rot * localSpacePos;
    }
    
    public Vector3 LocalToWorldVec(Vector3 localSpacePos)
    {
        return rot * localSpacePos;
    }
}
