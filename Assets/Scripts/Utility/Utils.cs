using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 RandomVector3(Vector3 _limits, bool _allowNegative)
    {
        return new Vector3(Random.Range(_allowNegative ? -_limits.x : 0f, _limits.x),
            Random.Range(_allowNegative ? -_limits.y : 0f, _limits.y), Random.Range(_allowNegative ? -_limits.z : 0f, _limits.z));
    }

    public static Vector3 RandomVector3(float _limit, bool _allowNegative)
    {
        return new Vector3(Random.Range(_allowNegative ? -_limit : 0f, _limit), Random.Range(_allowNegative ? -_limit : 0f, _limit),
            Random.Range(_allowNegative ? -_limit : 0f, _limit));
    }
    public static Vector3 RandomVector3(float x, float y, float z, bool _allowNegative)
    {
        return new Vector3(Random.Range(_allowNegative ? -x : 0f, x), Random.Range(_allowNegative ? -y : 0f, y), Random.Range(_allowNegative ? -z : 0f, z));
    }
    public static Vector3 RandomVector3(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
    {
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
    }
}
