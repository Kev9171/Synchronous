using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCharSelectionManager : Singleton<MapCharSelectionManager>
{
    [SerializeField] GameObject MapPanel;
    [SerializeField] GameObject CharPanel;
    [SerializeField] GameObject map;
    public MapBtn ClickedMapBtn { get; private set; }
    public CharBtn ClickedCharBtn { get; private set; }

    public void AfterMapSelected()
    {
        MapPanel.SetActive(false);
        CharPanel.SetActive(true);
        map.SetActive(true);
    }

    public void AfterCharSelected()
    {
        CharPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        MapPanel.SetActive(true);
        CharPanel.SetActive(false);
        map.SetActive(false);
    }

    public void SelectMap(MapBtn mapBtn)
    {
        this.ClickedMapBtn = mapBtn;
    }

    public void SelectChar(CharBtn charBtn)
    {
        this.ClickedCharBtn = charBtn;
    }

    public void DeployLimit()
    {
        ClickedCharBtn = null;
    }
}
