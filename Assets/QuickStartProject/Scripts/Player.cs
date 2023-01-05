using Mirror;
using UnityEngine;

namespace QuickStartProject.Scripts
{
    public class Player : NetworkBehaviour
    {
        private Transform _mainCamera;
        public TextMesh playerNameText;
        public GameObject floatingInfo;
        public Material playerMaterialClone;
        
        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;
        
        private SceneScript _sceneScript;

        private int selectedWeaponLocal = 1;
        public GameObject[] weaponArray;

        [SyncVar(hook = nameof(OnWeaponChanged))]
        public int activeWeaponSynced = 1;

        private void Awake()
        {
            //allow all players to run this
            _sceneScript = FindObjectOfType<SceneScript>();
            
            // disable all weapons
            foreach (var item in weaponArray)
                if (item != null)
                    item.SetActive(false); 
        }
        
        void OnWeaponChanged(int _Old, int _New)
        {
            // disable old weapon
            // in range and not null
            if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
                weaponArray[_Old].SetActive(false);
    
            // enable new weapon
            // in range and not null
            if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
                weaponArray[_New].SetActive(true);
        }

        [Command]
        public void CmdChangeActiveWeapon(int newIndex)
        {
            activeWeaponSynced = newIndex;
        }

        [Command]
        public void CmdSendPlayerMessage()
        {
            if (_sceneScript) 
                _sceneScript.statusText = $"{playerName} says hello {Random.Range(10, 99)}";
        }

        private void OnNameChanged(string original, string updated)
        {
            playerNameText.text = playerName;
        }

        private void OnColorChanged(Color original, Color updated)
        {
            playerNameText.color = updated;
            playerMaterialClone = new Material(playerMaterialClone)
            {
                color = updated
            };
        }
        public override void OnStartLocalPlayer()
        {
            _mainCamera = Camera.main!.transform;
            _mainCamera.SetParent(transform);
            _mainCamera.localPosition = new Vector3(0, 0, 0);
            
            floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
            floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string newPlayerName = "Player" + Random.Range(100, 999);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            CmdSetupPlayer(newPlayerName, color);
        }
        
        [Command]
        private void CmdSetupPlayer(string pName, Color col)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            this.playerName = pName;
            playerColor = col;
            _sceneScript.statusText = $"{this.playerName} joined.";
        }

        private void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            var moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
            var moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

            transform.Rotate(0, moveX, 0);
            transform.Translate(0, 0, moveZ);
        }
    }
}