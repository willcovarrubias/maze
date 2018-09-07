using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public enum WhereAmI { notSet, village, player, temp, armory, gems }

    public enum VillageMenu { mainMenu, inventory, armor, weapons, pub, upgrade }

    public enum Area { village, maze}
}
