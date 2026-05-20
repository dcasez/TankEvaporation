using HarmonyLib;
using Timberborn.ModManagerScene;

namespace TankEvaporation;

public class ModStarter : IModStarter
{
    void IModStarter.StartMod(IModEnvironment modEnvironment)
    {
        new Harmony("TankEvaporation").PatchAll();
    }
}