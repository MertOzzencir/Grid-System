using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject _mouseCursor;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Grid _grid;
    [SerializeField] private ObjectDataBaseOS _dataBase;
    [SerializeField] private GameObject _gridVisualation;

    private int _selectedObjectIndex = -1;
    private GridData floorData, furnutireData;
    private List<GameObject> placedGameObject = new();

    [SerializeField]
    private PreviewSystem _previewSystem;

    private Vector3Int _lastDetectedPosition = Vector3Int.zero;
    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnutireData = new();
    }
    void Update()
    {
            
            if(_selectedObjectIndex <0)
                return;
            Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
            Vector3Int gridPosition = _grid.WorldToCell(mousePosition);

            if(_lastDetectedPosition != gridPosition)
            {
            bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
            _mouseCursor.transform.position = mousePosition;
            _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
            _lastDetectedPosition = gridPosition;
        }
           

       

    }
    public void StopPlacement()
    {
        _selectedObjectIndex = -1;
        _gridVisualation.SetActive(false);
        _previewSystem.StopShowingPreview();
        _mouseCursor.SetActive(false);
        _inputManager.OnClicked -= PlaceStructure;
        _inputManager.OnExit -= StopPlacement;
        _lastDetectedPosition = Vector3Int.zero;
    }
    public void StartPlacement(int ID)
    {
        StopPlacement();
        _selectedObjectIndex = _dataBase.objectsData.FindIndex(x => x.ID == ID);
        if (_selectedObjectIndex < 0)
        {
            return;
        }
        _gridVisualation.SetActive(true);
        _previewSystem.StartShowingPlacementPreview(_dataBase.objectsData[_selectedObjectIndex].Prefab, 
            _dataBase.objectsData[_selectedObjectIndex].Size);
        _mouseCursor.SetActive(true);
        _inputManager.OnClicked += PlaceStructure;
        _inputManager.OnExit += StopPlacement;

    }


    public void PlaceStructure()
    {
        if(_inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition,_selectedObjectIndex);
        if(!placementValidity)
        {
            return;
        }
        GameObject newObject = Instantiate(_dataBase.objectsData[_selectedObjectIndex].Prefab);
        newObject.transform.position = _grid.CellToWorld(gridPosition);
        placedGameObject.Add(newObject);
        GridData selectedData = _dataBase.objectsData[_selectedObjectIndex].ID == 3 ? floorData : furnutireData;
        selectedData.AddObjectAt(gridPosition, _dataBase.objectsData[_selectedObjectIndex].Size,
            _dataBase.objectsData[_selectedObjectIndex].ID,
            placedGameObject.Count -1  );
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition),false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = _dataBase.objectsData[selectedObjectIndex].ID == 3 ? floorData : furnutireData;

        return selectedData.CanPlaceObjectAt(gridPosition, _dataBase.objectsData[selectedObjectIndex].Size);

    }
}
