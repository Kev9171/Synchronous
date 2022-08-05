using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MapSelection : MonoBehaviour
{
    private MouseInput mouseInput;
    //[SerializeField] private Tilemap tilemap;
    [SerializeField] private MapCharSelectionManager mapSelectionScene;
    private int generatedMapCounter = 0;

    private void Awake()
    {
        mouseInput = new MouseInput();
    }
    private void OnEnable()
    {
        mouseInput.Enable();
    }
    private void OnDisable()
    {
        mouseInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        //if (!PhotonNetwork.IsMasterClient)
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        if (generatedMapCounter < 1)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && MapCharSelectionManager.Instance.ClickedMapBtn != null)
            {
                Debug.Log("Map selected.");
                mapSelectionScene.AfterMapSelected();
                //GameObject tilemap = (GameObject)Instantiate(MapCharSelectionManager.Instance.ClickedMapBtn.MapPrefab, new Vector3(0, 0), Quaternion.identity);
                generatedMapCounter++;
            }
        }

    }
}
