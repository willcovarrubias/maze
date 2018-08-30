using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingDatabase : MonoBehaviour
{
    List<Buildings> buildings = new List<Buildings>();
    JsonData buildingData;

    void Start()
    {
        buildingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Buildings.json"));
        ConstructBuildingDatabase();
    }

    void ConstructBuildingDatabase()
    {
        for (int i = 0; i < buildingData.Count; i++)
        {
            Buildings building = new Buildings();
            building.id = (int)buildingData[i]["id"];
            building.title = buildingData[i]["title"].ToString();
            building.materials.Add(GetMaterials("level 1 materials", i));
            building.materials.Add(GetMaterials("level 2 materials", i));
            building.materials.Add(GetMaterials("level 3 materials", i));
            building.materials.Add(GetMaterials("level 4 materials", i));
            building.materials.Add(GetMaterials("level 5 materials", i));
            for (int j = 0; j < buildingData[i]["levels text"].Count; j++)
            {
                building.levelsDescription.Add(buildingData[i]["levels text"][j].ToString());
            }
            buildings.Add(building);
        }
    }

    Dictionary<int, int> GetMaterials(string jsonText, int i)
    {
        Dictionary<int, int> levelMaterials = new Dictionary<int, int>();
        for (int j = 0; j < buildingData[i][jsonText].Count; j++)
        {
            levelMaterials.Add((int)buildingData[i][jsonText][j]["material"],
                                (int)buildingData[i][jsonText][j]["amount"]);
        }
        return levelMaterials;
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

    public List<Buildings> GetBuildingsData()
    {
        return buildings;
    }
}

public class Buildings
{
    public int id { get; set; }
    public string title { get; set; }
    public List<Dictionary<int, int>> materials { get; set; }
    public List<string> levelsDescription { get; set; }

    public Buildings()
    {
        materials = new List<Dictionary<int, int>>();
        levelsDescription = new List<string>();
    }
}
