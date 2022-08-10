using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class MapSelection : MonoBehaviour
{
    public GameObject[] maps;
    public int selectedMap;

    public void SelectMap()
    {
        Vector2 mapPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        //maps = Instantiate(MapCharSelectionManager.Instance.ClickedMapBtn.MapPrefab, mapPosition, Quaternion.identity);
    }
}
