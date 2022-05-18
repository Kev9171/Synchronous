using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMovement : MonoBehaviour
{

    public Tilemap map;

    [SerializeField] private float movementSpeed;

    MouseInput mouseInput;

    private Vector3 destination;
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
        //transform.position = map.CellToWorld(map.WorldToCell(transform.position));
        destination = map.WorldToCell(transform.position);
        mouseInput.Mouse.MouseClick.performed += _ => MouseClick();
        
    }

    private void MouseClick()
    {
        Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPosition = map.WorldToCell(mousePosition);
        Vector3 gridCenter = map.CellToWorld(gridPosition);
        gridCenter.y += 0.3f;
        if (map.HasTile(gridPosition))
        {
            Debug.Log("grid pos =" + gridPosition);
            Debug.Log("world pos = " + gridCenter);
            destination = gridCenter;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, destination) > 0)
            transform.position = Vector3.MoveTowards(transform.position, destination, movementSpeed * Time.deltaTime);
    }
}
