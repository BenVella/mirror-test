using Mirror;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace QuickStartProject.Scripts
{
    public class SceneScript : NetworkBehaviour
    {
        public SceneReference sceneReference;
        
        public Text canvasStatusText;
        [FormerlySerializedAs("player")] public PlayerScript playerScript;

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        public string statusText;

        void OnStatusTextChanged(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage()
        {
            if (playerScript != null)  
                playerScript.CmdSendPlayerMessage();
        }
    }
}