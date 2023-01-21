using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using SimpleJSON;
using UnityEngine.SceneManagement;
using Static.Vars;

namespace Category.UI
{

    public class CategoryUI: MonoBehaviour
    {
        public RectTransform categoryContainer;
        public GameObject categoryPrefab;
        public static CategoryUI instance;
        private List <GameObject> categorySegments = new List <GameObject>();
        private string loadRecipeList = "RecipeListScreen";

        void Awake()
        {
          instance = this;
        }

        /*
        * create a category segment
        */
        GameObject CreateNewCategorySegments(JSONNode categoryRecords)
        {
            // instantiate the segment from prefab
            GameObject categorySegment = Instantiate(categoryPrefab);
            categorySegment.transform.SetParent(categoryContainer.transform);
            categorySegments.Add(categorySegment);
            int index = categorySegments.IndexOf(categorySegment);

            // calls function when clicked to display all recipes for clicked category
            categorySegment.GetComponent<Button>().onClick.AddListener(() => {ShowClickedCategory(categoryRecords, index);});
            categorySegment.SetActive(false);

            return categorySegment;
        }

        /*
        * display JSONNode records as category segments
        */
        public void SetCategorySegments(JSONNode categoryRecords)
        {
            foreach(GameObject categorySegment in categorySegments)
            {
                categorySegment.SetActive(false);
            }
            for (int x = 0; x < categoryRecords.Count; ++x)
            {
                // create a new category segment if there aren't as much segments as category records
                GameObject categorySegment = x < categorySegments.Count ? categorySegments[x] : CreateNewCategorySegments(categoryRecords);
                categorySegment.SetActive(true);

                // get category component
                TextMeshProUGUI categoryText = categorySegment.transform.Find("CategoryName").GetComponent<TextMeshProUGUI>();
                //set category component text
                categoryText.text = categoryRecords[x]["categoryLabel"]["value"];
            }
            categoryContainer.sizeDelta = new Vector2(categoryContainer.sizeDelta.x, GetContainerHeight(categoryRecords.Count));
        }

        /*
        * set container height of prefabs depending on category name and spacing
        */
        float GetContainerHeight(int countedCategories)
        {
          float height = 0.0f;
          //set overall height of category segment
          height += countedCategories * (categoryPrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
          // set spacing between category segments
          height += countedCategories * categoryContainer.GetComponent<VerticalLayoutGroup>().spacing;
          return height;
        }

        //load scene to show recipes of clicked category
        public void ShowClickedCategory(JSONNode categoryRecords, int index)
        {
          StaticVars.selectedCategory = categoryRecords[index]["categories"]["value"];
          StaticVars.selectedCategoryLabel = categoryRecords[index]["categoryLabel"]["value"];
          SceneManager.LoadScene(loadRecipeList);
          Debug.Log("Selected Category Name\n" + categoryRecords[index]["categoryLabel"]["value"]);
        }
    }
}