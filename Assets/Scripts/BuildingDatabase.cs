using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingDatabase : MonoBehaviour
{
    private List<Buildings> buildings = new List<Buildings>();
    private JsonData buildingData;

    void Start()
    {
        buildingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Buildings.json"));
        ConstructBuildingDatabase();
        //PrintBuildings();
    }

    void ConstructBuildingDatabase()
    {
        for (int i = 0; i < buildingData.Count; i++)
        {
            Buildings building = new Buildings();
            building.id = (int)buildingData[i]["id"];
            building.title = buildingData[i]["title"].ToString();
            Dictionary<int, int> level1materials = new Dictionary<int, int>();
            for (int j = 0; j < buildingData[i]["level 1 materials"].Count; j++)
            {
                level1materials.Add((int)buildingData[i]["level 1 materials"][j]["material"],
                                    (int)buildingData[i]["level 1 materials"][j]["amount"]);
            }
            Dictionary<int, int> level2materials = new Dictionary<int, int>();
            for (int j = 0; j < buildingData[i]["level 2 materials"].Count; j++)
            {
                level2materials.Add((int)buildingData[i]["level 2 materials"][j]["material"],
                                    (int)buildingData[i]["level 2 materials"][j]["amount"]);
            }
            building.materials.Add(level1materials);
            building.materials.Add(level2materials);
            buildings.Add(building);
        }
    }

    Dictionary<int, int> GetMaterialsNeededForBuilding(int level, int building)
    {
        Dictionary<int, int> materials = new Dictionary<int, int>();

        return materials;
    }

    void PrintBuildings()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            Debug.Log(buildings[i].id);
            for (int j = 0; j < buildings[i].materials.Count; j++)
            {
                foreach (KeyValuePair<int, int> keyValue in buildings[i].materials[j])
                {
                    Debug.Log(GameMaster.gameMaster.GetComponent<ItemDatabase>().FetchItemByID(keyValue.Key).Title);
                    Debug.Log(keyValue.Value);
                }
            }
        }
    }
}

public class Buildings
{
    public int id { get; set; }
    public string title { get; set; }
    public List<Dictionary<int, int>> materials { get; set; }

    public Buildings() { materials = new List<Dictionary<int, int>>(); }
}
