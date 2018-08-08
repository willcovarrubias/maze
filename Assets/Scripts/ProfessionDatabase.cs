using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.IO;

public class ProfessionDatabase : MonoBehaviour {

    private List<Professions> professions = new List<Professions>();
    private JsonData professionData;
        

	// Use this for initialization
	void Start () {

    
        professionData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Classes.json"));
        ConstructProfessionDatabase();

    }

    public Professions FetchProfessionByID(int id)
    {
        for (int i = 0; i < professions.Count; i++)
        {
            if (professions[i].id == id)
                return professions[i];
        }
        return null;
    }



    void ConstructProfessionDatabase()
    {
        for (int i = 0; i < professionData.Count; i++)
        {
            professions.Add(new Professions((int)professionData[i]["id"],
                professionData[i]["title"].ToString(),
                (int)professionData[i]["tier"],
                (int)professionData[i]["numberOfAttacks"],
                (double)professionData[i]["hpMod"],
                (double)professionData[i]["mpMod"],
                (double)professionData[i]["attackMod"],
                (double)professionData[i]["specialMod"],
                (double)professionData[i]["defenseMod"],
                (double)professionData[i]["speedMod"],
                (double)professionData[i]["luckMod"],
                (double)professionData[i]["sizeMod"]));
        }
    }

    public int GetCountOfProfessionsList()
    {
        int count = professions.Count;
        return count;
    }
}

public class Professions
{
    public int id { get; set; }
    public string title { get; set; }
    public int tier { get; set; }
    public int numberOfAttacks { get; set; }
    public double hpMod { get; set; }
    public double mpMod { get; set; }
    public double attackMod { get; set; }
    public double specialMod { get; set; }
    public double defenseMod { get; set; }
    public double speedMod { get; set; }
    public double luckMod { get; set; }
    public double itemsMod { get; set; }


    public Professions(int id, string title, int tier, int numberOfAttacks, double hpMod, double mpMod, double attackMod, double specialMod,
        double defenseMod, double speedMod, double luckMod, double itemsMod)
    {
        this.id = id;
        this.title = title;
        this.tier = tier;
        this.numberOfAttacks = numberOfAttacks;
        this.hpMod = hpMod;
        this.mpMod = mpMod;
        this.attackMod = attackMod;
        this.specialMod = specialMod;
        this.defenseMod = defenseMod;
        this.speedMod = speedMod;
        this.luckMod = luckMod;
        this.itemsMod = itemsMod;
    }

    public Professions()
    {
        this.id = -1;
    }

}
