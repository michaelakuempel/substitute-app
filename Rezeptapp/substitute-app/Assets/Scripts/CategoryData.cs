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

namespace Category.Data
{

    public class CategoryData: MonoBehaviour
    {
        private string categoryListQuery = "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> PREFIX recipe: <http://purl.org/NonFoodKG/1-mio-recipes#> SELECT DISTINCT * WHERE {?categories a <http://purl.org/NonFoodKG/1-mio-recipes#Course>. BIND (REPLACE(STR(?categories), '^.*/([^/]*)$', '$1') as ?categoriesUpper). BIND (CONCAT(LCASE(?categoriesUpper)) AS ?categoryLabel)} ORDER BY ASC (?categoriesLabel) LIMIT 100";
        private string allRecipesListQuery = "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>PREFIX owl: <http://www.w3.org/2002/07/owl#>PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> PREFIX food: <http://purl.org/heals/food/> PREFIX recipe: <http://purl.org/NonFoodKG/1-mio-recipes#>SELECT DISTINCT ?recipe ?recipeLabel WHERE { ?recipe rdfs:subClassOf* recipe:Recipe.?recipe rdfs:label ?recipeLabel filter(langMatches(lang(?recipeLabel),'en')).} ORDER BY ASC (?recipeLabel) LIMIT 15";
        private JSONNode jsonCategory;
        private JSONNode jsonAllRecipes;
        private UnityWebRequest categoryReq;
        private UnityWebRequest allRecipesReq;
        public TextMeshProUGUI categoryInfo;
        string rawAllRecipes;
        string rawCategory ;
        private static CategoryData instance;

        void Start()
        {
            categoryInfo.text = "Categories are loading. Please wait.";
            StartCoroutine(GetCategoryData());
         }

        /*
        * get data of recipe categories
        * create JSONNode with response data if request successful and valid
        */
        public IEnumerator GetCategoryData()
        {
            using(categoryReq = UnityWebRequest.Get("http://localhost:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(categoryListQuery)))
            {
                categoryReq.SetRequestHeader("Accept", "application/sparql-results+json");
                yield return categoryReq.SendWebRequest();

                using(allRecipesReq = UnityWebRequest.Get("http://localhost:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(allRecipesListQuery)))
                {
                    allRecipesReq.SetRequestHeader("Accept", "application/sparql-results+json");
                    yield return allRecipesReq.SendWebRequest();

                    switch (categoryReq.result)
                    {
                        case UnityWebRequest.Result.Success:
                            try
                            {
                                rawCategory = Encoding.Default.GetString(categoryReq.downloadHandler.data);
                                rawAllRecipes = Encoding.Default.GetString(allRecipesReq.downloadHandler.data);

                                jsonCategory = JSON.Parse(rawCategory);
                                jsonAllRecipes = JSON.Parse(rawAllRecipes);

                                if (jsonCategory.Count == 0 || jsonAllRecipes.Count == 0)
                                {
                                    categoryInfo.text = "couldn't be found.";
                                }
                                else
                                {
                                    StaticVars.recipeRecords = jsonAllRecipes["results"]["bindings"];
                                    CategoryUI.instance.SetCategorySegments(jsonCategory["results"]["bindings"]);
                                    SearchUI.instance.SetRecipeSearchSegments(jsonAllRecipes["results"]["bindings"]);

                                    categoryInfo.text = "Loaded " + jsonCategory["results"]["bindings"].Count + " categories.";
                                    Debug.Log("App successfully started!");
                                }
                            }
                            catch (Exception e)
                            {
                                categoryInfo.text = "An error occured: " + e.Message;
                                Debug.LogError(e);
                            }
                            break;
                        default:
                            categoryInfo.text = "can't be loaded. Try again later.";
                            Debug.Log(categoryReq.error);
                            break;
                    }
                }
            }
        }
    }
}