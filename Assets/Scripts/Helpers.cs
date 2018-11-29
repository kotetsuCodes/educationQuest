using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

    public class Helpers
    {
        public static void DebugValue(string label, object value)
        {
            Debug.Log($"{label}: {value}");
        }
    }
