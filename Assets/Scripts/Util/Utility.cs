using UnityEngine;
using System.Collections;

public static class Utility
{
   public static double[] Vector3toDoubleArray(Vector3 value)
   {
        return new double[3] { (double)value.x, (double)value.y, (double)value.z };
   }

    public static Vector3 DoubleArrayListToVector3(ArrayList value)
    {
        if (value.Count != 3)
        {
            Debug.LogError("Invalid Data");
            return Vector3.zero;
        }
        else
        {
            //(float)(double)value[1]とやると値が0の場合死ぬ//

            float vf0 = (float)System.Convert.ToDouble(value[0]);
            float vf1 = (float)System.Convert.ToDouble(value[1]);
            float vf2 = (float)System.Convert.ToDouble(value[2]);

            return new Vector3(vf0, vf1, vf2);
        }
    }
}
