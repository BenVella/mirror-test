using Mirror;
using UnityEngine.UI;

namespace QuickStartProject.Scripts
{
    public class SceneScript : NetworkBehaviour
    {
        public Text canvasStatusText;
        public Player player;

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        public string statusText;

        void OnStatusTextChanged(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage()
        {
            if (player != null)  
                player.CmdSendPlayerMessage();
        }
    }
}