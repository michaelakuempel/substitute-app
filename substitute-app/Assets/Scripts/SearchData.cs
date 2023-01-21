using SimpleJSON;
using System;
using System.Text;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Static.Vars;
using Category.UI;
using Search.UI;

namespace Search.Data
{

    public class SearchData: MonoBehaviour
    {
        private string allRecipesListQuery = "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> PREFIX recipe: <http://purl.org/NonFoodKG/1-mio-recipes#>SELECT DISTINCT ?recipe ?recipeLabel WHERE{ ?recipe rdfs:subClassOf*  recipe:Recipe.?recipe rdfs:label ?recipeLabel  filter(langMatches(lang(?recipeLabel),'en')).} ORDER BY ASC (?recipeLabel) #LIMIT 2000";  private JSONNode jsonAllRecipes;
        private UnityWebRequest allRecipesReq;
        string rawAllRecipes;
        private static SearchData instance;

        void Start()
        {
            StartCoroutine(GetSearchData());
        }

       /*
        * get data of recipes
        * create JSONNode with response data if request successful and valid
       */
        public IEnumerator GetSearchData()
        {
            using(allRecipesReq = UnityWebRequest.Get("http://192.168.178.34:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(allRecipesListQuery)))
            {
                allRecipesReq.SetRequestHeader("Accept", "application/sparql-results+json");
                yield return allRecipesReq.SendWebRequest();

                switch (allRecipesReq.result)
                {
                    case UnityWebRequest.Result.Success:
                        try
                        {
                            //create json node from json string
                            rawAllRecipes = Encoding.Default.GetString(allRecipesReq.downloadHandler.data);

                            jsonAllRecipes = JSON.Parse(rawAllRecipes);

                            if (jsonAllRecipes.Count == 0)
                            {
                                 Debug.Log("No Recipes For Search Term Found.");
                            }
                            else
                            {
                                StaticVars.recipeRecords = jsonAllRecipes["results"]["bindings"];
                                SearchUI.instance.SetRecipeSearchSegments(jsonAllRecipes["results"]["bindings"]);
                                Debug.Log("Recipe list successfully loaded!");
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                        break;
                    default:
                        Debug.Log(allRecipesReq.error);
                        break;

                }
            }
        }
    }
}