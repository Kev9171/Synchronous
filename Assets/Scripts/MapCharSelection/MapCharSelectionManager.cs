using KWY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MapCharSelectionManager : Singleton<MapCharSelectionManager>
{
    [SerializeField] GameObject MapPanel;
    [SerializeField] GameObject CharPanel;
    [SerializeField] GameObject StartMainGameSceneBtn;
    public MapBtn ClickedMapBtn { get; private set; }
    public CharBtn ClickedCharBtn { get; private set; }
    
    public GameObject generatedMap;
    //public GameObject[] maps;
    //public int selectedMap = 0;
    //private string selectedMapDataName = "SelectedMap";
    bool isMapGenerated = false;
    public CustomTile CustomTile;

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
    private void Start()
    {
        MapPanel.SetActive(true);
        CharPanel.SetActive(false);
        StartMainGameSceneBtn.SetActive(false);
    }

    private void Update()
    {
        if (isMapGenerated == true)
        {
            if (deployedCharCounter < 3)
            {
                DeployChar();
                UndeployChar();
            }
            else
            {
                CharPanel.SetActive(false);
                StartMainGameSceneBtn.SetActive(true);
            }
        }
        
    }

    public void SelectMap(MapBtn mapBtn)
    {
        this.ClickedMapBtn = mapBtn;
        //selectedMap = PlayerPrefs.GetInt(selectedMapDataName, 0);
        //PlayerPrefs.SetInt(selectedMapDataName, selectedMap);
        Debug.Log(MapCharSelectionManager.Instance.ClickedMapBtn.MapPrefab.name + " selected.");
        MapPanel.SetActive(false);
        Vector2 mapPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        generatedMap = Instantiate(MapCharSelectionManager.Instance.ClickedMapBtn.MapPrefab, mapPosition, Quaternion.identity);
        isMapGenerated = true;
        CharPanel.SetActive(true);
    }

    public void SelectChar(CharBtn charBtn)
    {
        this.ClickedCharBtn = charBtn;
        Debug.Log(MapCharSelectionManager.Instance.ClickedCharBtn.CharPrefab.name + " selected.");
    }

    public void DeployChar()
    {
        Tilemap map = (Tilemap)generatedMap.GetComponentInChildren(typeof(Tilemap)) as Tilemap;
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int clickV = map.WorldToCell(mousePosition);
        
        if (map.HasTile(clickV) && Input.GetKeyDown(KeyCode.Mouse0)) //&& map.GetSprite(clickV).name == "tileWater_full"
        {
            if (!EventSystem.current.IsPointerOverGameObject() && MapCharSelectionManager.Instance.ClickedCharBtn != null) // ���� �ƴ� �������� Ŭ���ϱ� ����
            {
                mousePosition = map.GetCellCenterWorld(clickV);
                GameObject deployedChar = (GameObject)Instantiate(MapCharSelectionManager.Instance.ClickedCharBtn.CharPrefab, mousePosition, Quaternion.identity);
                Debug.Log(MapCharSelectionManager.Instance.ClickedCharBtn.CharPrefab.name + " deployed.");
                deployedChar.GetComponent<SpriteRenderer>().sortingOrder = (int)mousePosition.x; // �Ʒ��� ��ġ�� ĳ���Ͱ� ȭ�鿡 ������
                //Debug.Log(mousePosition);
                MapCharSelectionManager.Instance.DeployLimit(); // ��ư Ŭ�� ��Ȱ��ȭ
                deployedCharCounter++;
            }
        }
        
    }

    public void UndeployChar()
    {
        Tilemap map = (Tilemap)generatedMap.GetComponentInChildren(typeof(Tilemap)) as Tilemap;
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int clickV = map.WorldToCell(mousePosition);

        if (map.HasTile(clickV) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!EventSystem.current.IsPointerOverGameObject() && MapCharSelectionManager.Instance.ClickedCharBtn == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        Debug.Log(gameObject.name + " destroyed.");
                        Destroy(gameObject);
                    }
                }
            }

        }
    }

    public void DeployLimit()
    {
        ClickedCharBtn = null;
    }

}
