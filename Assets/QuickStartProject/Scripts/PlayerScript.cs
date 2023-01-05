using Mirror;
using UnityEngine;

namespace QuickStartProject.Scripts
{
    public class PlayerScript : NetworkBehaviour
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

        private int _selectedWeaponLocal = 0;
        public GameObject[] weaponArray;

        [SyncVar(hook = nameof(OnWeaponChanged))]
        public int activeWeaponSynced = 1;

        private void Awake()
        {
            //allows all players to run this
            _sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
            
            // disable all weapons
            foreach (var item in weaponArray)
                if (item != null)
                    item.SetActive(false); 
        }
        
        void OnWeaponChanged(int current, int next)
        {
            // disable current weapon
            // in range and not null
            if (0 < current && current < weaponArray.Length && weaponArray[current] != null)
                weaponArray[current].SetActive(false);
    
            // enable next weapon
            // in range and not null
            if (0 < next && next < weaponArray.Length && weaponArray[next] != null)
                weaponArray[next].SetActive(true);
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
                // make non-local players run this
                floatingInfo.transform.LookAt(_mainCamera);
                return;
            }

            var moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
            var moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

            transform.Rotate(0, moveX, 0);
            transform.Translate(0, 0, moveZ);
            
            if (Input.GetButtonDown("Fire2")) //Fire2 is mouse 2nd click and left alt
            {
                _selectedWeaponLocal += 1;

                if (_selectedWeaponLocal > weaponArray.Length) 
                    _selectedWeaponLocal = 1; 

                CmdChangeActiveWeapon(_selectedWeaponLocal);
            }
        }
    }
}