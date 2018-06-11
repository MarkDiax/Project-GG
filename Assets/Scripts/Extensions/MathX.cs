using UnityEngine;
using System.Collections;

public static class MathX
{
    public static class Float
    {
        public static bool NearlyEqual(float A, float B, float Epsilon) {
            float absA = Mathf.Abs(A);
            float absB = Mathf.Abs(B);
            float diff = Mathf.Abs(A - B);

            if (A == B) { // shortcut, handles infinities
                return true;
            }
            else if (A == 0 || B == 0 || diff < float.MinValue) {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (Epsilon * float.MinValue);
            }
            else { // use relative error
                return diff / (absA + absB) < Epsilon;
            }
        }

    }
}