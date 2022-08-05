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
        //    Debug.Log("���콺 Ŭ����");
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit = new RaycastHit();
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider.gameObject.name == "Tilemap")
        //        {
        //            Debug.Log("Ÿ�ϸ� Ŭ����");
        //            Instantiate(flappy, hit.point, Quaternion.identity);
        //        }
        //        else
        //        {
        //            Debug.Log("Ÿ�ϸ� Ŭ�� �� ��");
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
            if (deployCounter < 5) // ��ġ ���� ĳ���� ��
            {
                if (!EventSystem.current.IsPointerOverGameObject() && dm.ClickedBtn != null) // ���� �ƴ� �������� Ŭ���ϱ� ����
                {
                    mousePosition = map.GetCellCenterWorld(clickV);
                    GameObject deployedChar = (GameObject)Instantiate(dm.ClickedBtn.CharacterPrefab, mousePosition, Quaternion.identity);
                    deployedChar.GetComponent<SpriteRenderer>().sortingOrder = (int)mousePosition.x;
                    Debug.Log(mousePosition);
                    dm.DeployLimit(); // ��ư Ŭ�� ��Ȱ��ȭ
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
