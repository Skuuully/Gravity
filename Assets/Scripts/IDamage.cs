using System.Collections.Generic;
using UnityEngine;

namespace Test {
    public interface IDamage {
        float GetDamage();

        List<Health> GetSafe();
    }
}