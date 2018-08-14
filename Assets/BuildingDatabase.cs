using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingDatabase : MonoBehaviour {

    private List<Buildings> buildings = new List<Buildings>();
    private JsonData buildingData;
    
	// Use this for initialization
	void Start () {

        buildingData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Buildings.json"));
        ConstructBuildingDatabase();
	}
    
    void ConstructBuildingDatabase()
    {
        for (int i = 0; i < buildingData.Count; i++)
        {
            buildings.Add(new Buildings((int)buildingData[i]["id"],
                buildingData[i]["title"].ToString(),
                (int)buildingData[i]["currentLevel"],
                (int)buildingData[i]["wood"],
                (int)buildingData[i]["leather"],
                (int)buildingData[i]["differentStone"]));
        }
    }

    public Buildings FetchBuildingsByID(int id)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i].id == id)
                return buildings[i];
        }
        return null;
    }

}

public class Buildings
{
    public int id { get; set; }
    public string title { get; set; }
    public int currentLevel { get; set; }
    public int wood { get; set; }
    public int leather { get; set; }
    public int differentStones { get; set; }

    public Buildings(int id, string title, int currentLevel, int wood, int leather, int differentStones)
    {
        this.id = id;
        this.title = title;
        this.currentLevel = currentLevel;
        this.wood = wood;
        this.leather = leather;
        this.differentStones = differentStones;
    }

    public Buildings()
    {
    }
}
