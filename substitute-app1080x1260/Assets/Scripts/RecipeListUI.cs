using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Category.UI;
using Category.Data;
using SimpleJSON;
using UnityEngine.SceneManagement;
using Static.Vars;
using System;

namespace Recipe.List.UI
{
    public class RecipeListUI : MonoBehaviour
    {
        public RectTransform recipeContainer;
        public GameObject recipePrefab;
        public static RecipeListUI instance;
        private List <GameObject> recipeSegments = new List<GameObject>();
        private string loadRecipe = "RecipeScreen";
        private string goBack = "CategoryListScreen";

        void Awake()
        {
            instance = this;
        }

        /*
        * create a recipe segment
        */
        GameObject CreateNewRecipeSegment(JSONNode recipeRecords)
        {
            // instantiate the segment from prefab
            GameObject recipeSegment = Instantiate(recipePrefab);
            recipeSegment.transform.SetParent(recipeContainer.transform);
            recipeSegments.Add(recipeSegment);
            int index = recipeSegments.IndexOf(recipeSegment);

            // calls function when clicked to display all details of recipe for clicked recipe
            recipeSegment.GetComponent<Button>().onClick.AddListener(()=>{ShowClickedRecipe(recipeRecords, index);});
            recipeSegment.SetActive(false);

            return recipeSegment;
        }

        /*
        * display JSONNode records as recipe segments
        */
        public void SetRecipeSegments(JSONNode recipeRecords)
        {
            foreach(GameObject recipeSegment in recipeSegments)
            {
                recipeSegment.SetActive(false);
            }
            for (int x = 0; x < recipeRecords.Count; ++x)
            {
                // create a new recipe segment if there aren't as much segments as recipe records
                GameObject recipeSegment = x < recipeSegments.Count ? recipeSegments[x] : CreateNewRecipeSegment(recipeRecords);
                recipeSegment.SetActive(true);

                // get recipe component
                TextMeshProUGUI locationText = recipeSegment.transform.Find("RecipeName").GetComponent<TextMeshProUGUI>();
                //set recipe component text
                locationText.text = recipeRecords[x]["recipeLabel"]["value"];
            }
            recipeContainer.sizeDelta = new Vector2(recipeContainer.sizeDelta.x, GetContainerHeight(recipeRecords.Count));
        }

        /*
        * set container height of prefabs depending on recipe name and spacing
        */
        float GetContainerHeight(int countedSubs)
        {
            float height = 0.0f;
            //set overall height of category segment
            height += countedSubs * (recipePrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
            // set spacing between category segments
            height += countedSubs * recipeContainer.GetComponent<VerticalLayoutGroup>().spacing;
            return height;
        }

        /*
        * load scene to show recipes of clicked category
        */
        public void ShowClickedRecipe(JSONNode records, int index)
        {
            StaticVars.selectedRecipe = records[index]["recipeLabel"]["value"];
            SceneManager.LoadScene(loadRecipe);
        }

       public void GoBack()
       {
            SceneManager.LoadScene(goBack);
       }
    }
}