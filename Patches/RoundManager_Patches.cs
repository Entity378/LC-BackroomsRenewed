namespace VELDDev.BackroomsRenewed.Patches;

[HarmonyPatch(typeof(RoundManager))]
public static class RoundManager_Patches
{
    [HarmonyPostfix, HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    public static void DespawnBackroomsAtEndOfRound(RoundManager __instance)
    {
        if (!__instance.IsServer)
            return;

        if (!Backrooms.Instance)
            return;
        
        Backrooms.Instance.GetComponent<NetworkObject>().Despawn(true);
        GameObject.Destroy(Backrooms.Instance.gameObject);
    }

    [HarmonyPostfix, HarmonyPatch(nameof(RoundManager.SpawnSyncedProps))]
    public static void SpawnBackroomsAtStartOfRound(RoundManager __instance)
    {
        if (!__instance.IsServer)
            return;

        if (Backrooms.Instance)
        {
            Backrooms.Instance.GetComponent<NetworkObject>().Despawn(true);
            Backrooms.Instance = null;
        }

        if (__instance.dungeonGenerator == null || __instance.dungeonGenerator.Root == null)
            return;
        
        var dungenRoot = __instance.dungeonGenerator.Root.transform.position;
        var backroomsGo = GameObject.Instantiate(Plugin.Instance.BackroomsPrefab, new Vector3(5000, dungenRoot.y, 0), Quaternion.identity);
        backroomsGo.GetComponent<NetworkObject>().Spawn(true);
        
    }
}
