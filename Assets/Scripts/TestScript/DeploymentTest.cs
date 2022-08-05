using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DeploymentTest : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    //public GameObject flappy;
    private MouseInput mouseInput;
    private int deployCounter = 0;

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

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("마우스 클릭됨");
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit = new RaycastHit();
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.gameObject.name == "Tilemap")
        //        {
        //            Debug.Log("타일맵 클릭됨");
        //            Instantiate(flappy, hit.point, Quaternion.identity);
        //        }
        //        else
        //        {
        //            Debug.Log("타일맵 클릭 안 됨");
        //        }
        //    }
        //}
    }

    private void DeployCharacter()
    {
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int clickV = map.WorldToCell(mousePosition);
        //Vector3Int clickV = map.GetCellCenterWorld()
        DeploymentManager dm = GameObject.FindObjectOfType<DeploymentManager>();
        if (map.HasTile(clickV) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (deployCounter < 5) // 배치 가능 캐릭터 수
            {
                if (!EventSystem.current.IsPointerOverGameObject() && dm.ClickedBtn != null) // 맵이 아닌 아이콘을 클릭하기 위함
                {
                    mousePosition = map.GetCellCenterWorld(clickV);
                    GameObject deployedChar = (GameObject)Instantiate(dm.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                    deployedChar.GetComponent<SpriteRenderer>().sortingOrder = (int)mousePosition.x;
                    Debug.Log(mousePosition);
                    dm.DeployLimit(); // 버튼 클릭 비활성화
                    deployCounter++;
                }
            }
            else
            {
                Debug.Log("Already deployed three characters.");
            }
        }
    }

}
