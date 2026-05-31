namespace VELDDev.BackroomsRenewed.Generation;

public class ExitBackroom : NetworkBehaviour
{
    public void TeleportOutsideTheBackrooms()
    {
        PlayerControllerB localplayer = GameNetworkManager.Instance.localPlayerController;
        Transform thisPlayer = GameNetworkManager.Instance.localPlayerController.thisPlayerBody;
        
        localplayer.TeleportPlayer(StartOfRound.Instance.playerSpawnPositions[localplayer.actualClientId].position, false, 0f, false, true);
        localplayer.isInsideFactory = false;
    }
}