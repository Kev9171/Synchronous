using KWY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class CharSelection : MonoBehaviour
{
    [SerializeField] private MapCharSelectionManager mapSelectionScene;
    [SerializeField] private Tilemap map;
    
    private MouseInput mouseInput;
    private int deployedCharCounter = 0;

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
        DeployCharacter();
    }

    private void DeployCharacter()
    {
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int clickV = map.WorldToCell(mousePosition);
        //Vector3Int clickV = map.GetCellCenterWorld()
        if (map.HasTile(clickV) && Input.GetKeyDown(KeyCode.Mouse0) && map.GetSprite(clickV).name == "tileWater_full")
        {
            if (deployedCharCounter < 3) // ��ġ ���� ĳ���� ��
            {
                if (!EventSystem.current.IsPointerOverGameObject() && MapCharSelectionManager.Instance.ClickedCharBtn != null) // ���� �ƴ� �������� Ŭ���ϱ� ����
                {
                    mousePosition = map.GetCellCenterWorld(clickV);
                    GameObject deployedChar = (GameObject)Instantiate(MapCharSelectionManager.Instance.ClickedCharBtn.CharPrefab, mousePosition, Quaternion.identity);
                    deployedChar.GetComponent<SpriteRenderer>().sortingOrder = (int)mousePosition.x;
                    Debug.Log(mousePosition);
                    MapCharSelectionManager.Instance.DeployLimit(); // ��ư Ŭ�� ��Ȱ��ȭ
                    deployedCharCounter++;
                }
            }
            else
            {
                Debug.Log("Already deployed three characters.");
            }
        }
    }
}
