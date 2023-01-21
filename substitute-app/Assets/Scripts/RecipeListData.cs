using SimpleJSON;
using System;
using System.Text;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Recipe.List.UI;
using Static.Vars;
using Search.UI;

namespace Recipe.List.Data
{
    public class RecipeListData: MonoBehaviour
    {
        private string recipeListFstQuery = "PREFIX owl: <http://www.w3.org/2002/07/owl#>PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> PREFIX food: <http://purl.org/heals/food/> PREFIX recipe: <http://purl.org/NonFoodKG/1-mio-recipes#>SELECT DISTINCT ?recipe ?recipeLabel WHERE{ ?recipe rdfs:subClassOf* recipe:Recipe. ?recipe rdfs:subClassOf [ a owl:Restriction; owl:onProperty food:isRecommendedForCourse; owl:hasValue <";
        private string recipeListSndQuery = "> ]. ?recipe rdfs:label ?recipeLabel.filter(langMatches(lang(?recipeLabel),'en')).} ORDER BY ASC (?recipeLabel)";
        private string recipeListTrdQuery = "> ].?recipe rdfs:label ?recipeLabel.filter(langMatches(lang(?recipeLabel),'en')).} ORDER BY ASC (?recipeLabel) LIMIT 1500";
        private string recipeListQuery;
        private JSONNode jsonRecipe;
        private UnityWebRequest recipeReq;
        public TextMeshProUGUI recipeInfo;
        public TextMeshProUGUI category;
        string rawRecipe;
        private static RecipeListData instance;

        void Start()
        {
            string categoryNameLabel = StaticVars.selectedCategoryLabel;
            recipeInfo.text = "Recipes are loading. Please wait.";
            StartCoroutine(getRecipeData());
        }

        /*
        * get recipe list data of clicked category
        * create JSONNode with response data if request successful and valid
        */
        public IEnumerator getRecipeData()
        {
            string recipeName = StaticVars.selectedCategory;
            string categoryNameLabel = StaticVars.selectedCategoryLabel;
            recipeListQuery = recipeListFstQuery + recipeName + recipeListSndQuery;

            using(recipeReq = UnityWebRequest.Get("http://192.168.178.34:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(recipeListQuery)))
            {
                recipeReq.SetRequestHeader("Accept", "application/sparql-results+json");
                yield return recipeReq.SendWebRequest();

                switch (recipeReq.result)
                {
                    case UnityWebRequest.Result.Success:
                        try
                        {
                            rawRecipe = Encoding.Default.GetString(recipeReq.downloadHandler.data);
                            jsonRecipe = JSON.Parse(rawRecipe);

                            if (jsonRecipe.Count == 0)
                            {
                                recipeInfo.text = "Recipes couldn't be found.";
                            }
                            else
                            {
                                RecipeListUI.instance.SetRecipeSegments(jsonRecipe["results"]["bindings"]);
                                category.text = categoryNameLabel;
                                if(jsonRecipe["results"]["bindings"].Count <= 0)
                                {
                                    recipeInfo.text = "Category has no recipes yet.";
                                }
                                else
                                {
                                    recipeInfo.text = "Loaded " + jsonRecipe["results"]["bindings"].Count + " recipes.";
                                    Debug.Log("Recipes successfully loaded!");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            recipeInfo.text = "An error occured: " + e.Message;
                            Debug.LogError(e);
                        }
                        break;
                    default:
                        recipeInfo.text = "Recipes can't be loaded. Try again later.";
                        Debug.Log(recipeReq.error);
                        break;
                }
            }
        }
    }
}