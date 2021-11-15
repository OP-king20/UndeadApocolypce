using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public enum Hitboxes {Head, Body, Limbs};
    public Hitboxes hitbox;

    public static DamageZone instance;
}
