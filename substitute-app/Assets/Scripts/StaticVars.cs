using UnityEngine;
using SimpleJSON;
namespace Static.Vars
{
    public class StaticVars : MonoBehaviour
    {
        public static string selectedRecipe;
        public static int ingredientIndex;
        public static string selectedCategory;
        public static string selectedCategoryLabel;
        public static string ingLabel;
        public static string foundSubsIri;
        public static string ingUnitLabel;
        public static float ingValue;
        public static string ingredientRecords;
        public static JSONNode recipeRecords;
        public static JSONNode categoryRecords;
    }
}

