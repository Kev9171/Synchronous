using KWY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] MainGameData data;
    [SerializeField] private GameObject[] maps; // ·£´ý ¸Ê
    public Character SelChara { get; private set; }
    MouseInput mouseInput;
    private GameObject generatedMap;

    // Start is called before the first frame update
    void Start()
    {
        //MapSelection();
        StartControl();
        
        // Ä³¸¯ÅÍ ÆÐ³Î º¸¿© ÁÖ±â
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartControl()
    {
        mouseInput.Mouse.MouseClick.performed += OnClick;
    }

    public void SetSelChara(CID cid)
    {
        SelChara = null;

        Character c = data.GetCharacter(cid);

        if (c == null)
        {
            Debug.LogErrorFormat("Can not select - cid: {0}", cid);
            return;
        }

        SelChara = c;

        mouseInput.Mouse.MouseClick.performed += OnClick;

        Debug.Log("Character selected: " + cid);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            CID cid = hit.collider.gameObject.GetComponent<Character>().Cb.cid;
            SetSelChara(cid);
        }
    }

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

    public void MapSelection()
    {
        Vector2 mapPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)); // Screen Áß¾Ó¿¡ ¸Ê »ý¼º
        generatedMap = Instantiate(maps[0], mapPosition, Quaternion.identity);
    }
}
