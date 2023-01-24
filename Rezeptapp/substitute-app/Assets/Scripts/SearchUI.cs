using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using SimpleJSON;
using UnityEngine.SceneManagement;
using Static.Vars;
using System;
using Recipe.Data;

namespace Search.UI
{
    public class SearchUI: MonoBehaviour
    {
        public GameObject[] recipeSugg;
        public GameObject searchBar;
        public GameObject searchBtn;
        public TextMeshProUGUI recipeNameInput;
        public RectTransform recipeContainer;
        public GameObject recipePrefab;
        public GameObject suggList;
        public static SearchUI instance;
        int totalRecipes;
        private List <GameObject> recipeSegments = new List <GameObject>();
        private string loadRecipe = "RecipeScreen";

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

            // calls function when clicked to display check search input
            recipeSegment.GetComponent<Button>().onClick.AddListener(()=>{AddSearchToBar(recipeRecords, index); CheckSearchInput();});
            recipeSegment.SetActive(false);

            return recipeSegment;
        }

        /*
        * display JSONNode records as recipe segments
        */
        public void SetRecipeSearchSegments(JSONNode recipeRecords)
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
                // get category component
                TextMeshProUGUI recipeText = recipeSegment.transform.Find("RecipeName").GetComponent<TextMeshProUGUI>();
                // set category component
                recipeText.text = recipeRecords[x]["recipeLabel"]["value"];
            }
            recipeContainer.sizeDelta = new Vector2(recipeContainer.sizeDelta.x, GetContainerHeight(recipeRecords.Count));

            totalRecipes = recipeContainer.transform.childCount;
            recipeSugg = new GameObject[totalRecipes];

            for (int i = 0; i < totalRecipes; i++)
            {
                recipeSugg[i] = recipeContainer.transform.GetChild(i).gameObject;
            }
        }

        /*
        * set container height of prefabs depending on category name and spacing
        */
        float GetContainerHeight(int countedRecipes)
        {
          float height = 0.0f;
          height += countedRecipes * (recipePrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
          height += countedRecipes * recipeContainer.GetComponent<VerticalLayoutGroup>().spacing;
          return height;
        }

        /*
        * check if input recipe name is an actual recipe
        */
        public void CheckSearchInput()
        {
            string cleanName = rmvChar();
            JSONNode allRecipes = StaticVars.recipeRecords;
            Dictionary<string,int> rec = new Dictionary<string,int>();
            for (int x = 0; x < allRecipes.Count; ++x)
            {
                rec.Add(Uri.EscapeUriString(allRecipes[x]["recipeLabel"]["value"]), x);
            }
            if (rec.ContainsKey(cleanName))
            {
                int value = rec[cleanName];
                StaticVars.selectedRecipe = allRecipes[value]["recipeLabel"]["value"];
                SceneManager.LoadScene(loadRecipe);
            }
            else
            {
                Debug.Log("Recipename not found");
            }
        }

        /*
        * removes unwanted characters in string
        */
        string rmvChar()
        {
            string encodedName = Uri.EscapeUriString(recipeNameInput.text);
            var removeChar = new string[] {"%E2%80%8B"};
            foreach(var chr in removeChar)
            {
                string cleanName = encodedName.Replace(chr, string.Empty);
                return cleanName;
            }
            return encodedName;
        }

        /*
        * add clicked recipe name on suggestion list as text to searchbar
        */
        void AddSearchToBar(JSONNode getRecipeName, int index)
        {
            string s = getRecipeName[index]["recipeLabel"]["value"];
            TMP_InputField searchBarText = searchBar.GetComponent<TMP_InputField>();
            searchBarText.text = s;
        }

        /*
        * display a list of recipe suggestions depending on search bar input
        */
        public void DisplayRecipeSuggestions()
        {
            string searchText = searchBar.GetComponent<TMP_InputField>().text;
            int searchTxtLength = searchText.Length;
            int searchedElements = 0;
            bool isActive = suggList.activeSelf;

            if (searchTxtLength > 0)
            {
                suggList.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(recipeContainer);
                foreach(GameObject ele in recipeSugg)
                {
                    searchedElements += 1;
                    if (ele.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Length >= searchTxtLength)
                    {
                        if (searchText.ToLower() == ele.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Substring(0, searchTxtLength).ToLower())
                        {
                            ele.SetActive(true);
                        }
                        else
                        {
                            ele.SetActive(false);
                        }
                     }
                }
            }
        }

        /*
        * close the dropdown recipe suggestion list via button click
        */
        public void CloseSearchDropDown()
        {
            if (suggList != null)
            {
                bool isActive = suggList.activeSelf;
                if (isActive == true)
                {
                    suggList.SetActive(false);
                }
            }
        }
    }
}