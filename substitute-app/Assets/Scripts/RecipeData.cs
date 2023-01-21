using System;
using System.Text;
using System.Collections;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Recipe.UIData;
using Static.Vars;
using Search.UI;

namespace Recipe.Data
{
    public class RecipeData : MonoBehaviour
    {
        private string recipeInstrFstQuery = "PREFIX rec: <http://purl.org/NonFoodKG/1-mio-recipes#>SELECT DISTINCT ?instructions ?seeAlso WHERE { rec:";
        private string recipeInstrSndQuery = " rec:instructions ?instructions. rec:";
        private string recipeInstrTrdQuery = " rec:link ?seeAlso. }";
        private string recipeIngrFstQuery = "PREFIX owl: <http://www.w3.org/2002/07/owl#> PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> PREFIX oboI: <http://www.geneontology.org/formats/oboInOwl#> SELECT DISTINCT ?iri ?unitValue ?DbXRef ?label ?unit  ?unitLabel WHERE { <http://purl.org/NonFoodKG/1-mio-recipes#";
        private string recipeIngrSndQuery = "> rdfs:subClassOf ?intersection BIND(?intersection AS ?intersection2) BIND(?intersection AS ?intersection3) BIND(?intersection AS ?intersection4) OPTIONAL {?intersection3 owl:intersectionOf ?restriction. ?restriction rdf:first/(owl:someValuesFrom) ?iri. ?iri rdfs:label ?label. OPTIONAL { ?iri oboI:hasDbXref ?DbXRef. } ?restriction rdf:rest/rdf:first/(owl:cardinality) ?unitValue. } OPTIONAL { ?intersection4 owl:intersectionOf ?restriction2. ?restriction2 rdf:first/(owl:someValuesFrom) ?iri4. BIND(?iri4 AS ?iri) ?iri4 rdfs:label ?label. OPTIONAL { ?iri4 oboI:hasDbXref ?DbXRef. } ?restriction2 rdf:rest/rdf:first ?andClass. ?andClass owl:qualifiedCardinality ?unitValue. ?andClass owl:onClass ?unit. ?unit rdfs:label ?unitLabel. } OPTIONAL { ?intersection2 owl:onProperty <http://purl.org/NonFoodKG/1-mio-recipes#hasIngredient>. ?intersection2 owl:someValuesFrom ?iri3. ?iri3 rdfs:label ?label. OPTIONAL {      ?iri3 oboI:hasDbXref ?DbXRef. } } OPTIONAL { BIND(?iri3 AS ?iri) } FILTER (!isBlank(?iri) || !isBlank (?value) || !isBlank (?DbXRef) || !isBlank(?label) || !isBlank(?unit) || !isBlank(?unitLabel)) } LIMIT 100";
        private string allSubs = "PREFIX owl: <http://www.w3.org/2002/07/owl#> PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> PREFIX oboI: <http://www.geneontology.org/formats/oboInOwl#> SELECT ?iri ?label ?DbXRef WHERE { ?iri rdfs:subClassOf* <http://purl.org/NonFoodKG/product-taxonomy#Product>. ?iri rdfs:label ?label. ?iri oboI:hasDbXref ?DbXRef. filter(langMatches(lang(?label),'en')).}";
        private JSONNode recipeInstrJson;
        private JSONNode recipeIngrJson;
        private JSONNode allSubsJson;
        private UnityWebRequest recipeInstr;
        private UnityWebRequest recipeIngrReq;
        private UnityWebRequest allSubsReq;
        public TextMeshProUGUI ingredientRecsText;
        string recipeDetailsRaw;
        string recipeSubsRaw;
        string recipeIngRaw;
        string recipeInstrQuery;
        string recipeIngrQuery;

        void Start()
        {
            ingredientRecsText.text = "Ingredients are loading. Please wait.";
            StartCoroutine(getRecipeData());
        }

        /*
        * removes unwanted characters in string
        */
        string rmvChar()
        {
            string recipeName = StaticVars.selectedRecipe;
            string encodedName = Uri.EscapeUriString(recipeName);
            var removeChar = new string[] {"%E2%80%8B"};

            foreach(var chr in removeChar)
            {
                string cleanName = encodedName.Replace(chr, string.Empty);
                return cleanName;
            }
            return encodedName;
        }

        /*
        * get data for links and instructions of recipe of clicked recipe
        * get all food products of substitute ontology
        * get all ingredients of clicked recipe
        * store response data in JSONNodes
        */
        IEnumerator getRecipeData()
        {
            string encodedName = rmvChar();
            string recipeName = StaticVars.selectedRecipe;

            if (string.IsNullOrEmpty(encodedName))
            {
                Debug.Log("Input recipe name is not valid.");
            }
            else
            {
                recipeInstrQuery = recipeInstrFstQuery + encodedName + recipeInstrSndQuery + encodedName + recipeInstrTrdQuery;
                recipeIngrQuery = recipeIngrFstQuery + encodedName + recipeIngrSndQuery;

                //get data for links and instructions of recipe
                using(recipeInstr = UnityWebRequest.Get("http://192.168.178.34:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(recipeInstrQuery)))
                {
                    recipeInstr.SetRequestHeader("Accept", "application/sparql-results+json");
                    yield return recipeInstr.SendWebRequest();
                    switch (recipeInstr.result)
                    {
                        case UnityWebRequest.Result.Success:
                            try
                            {
                                recipeDetailsRaw = Encoding.Default.GetString(recipeInstr.downloadHandler.data);
                                recipeInstrJson = JSON.Parse(recipeDetailsRaw);
                                if (recipeInstrJson == 0) {
                                    Debug.Log("No recipe details found.");
                                }
                                else
                                {
                                    RecipeUIData.instance.SetRecipeDetails(recipeName, recipeInstrJson["results"]["bindings"]);
                                    Debug.Log("Recipe details successfully loaded.");
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                            break;
                        default:
                            Debug.Log(recipeInstr.error);
                            break;
                    }
                }

                //get all food products of substitute ontology that can be used as substitute for ingredients
                using(allSubsReq = UnityWebRequest.Get("http://192.168.178.34:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(allSubs)))
                {
                    allSubsReq.SetRequestHeader("Accept", "application/sparql-results+json");
                    yield return allSubsReq.SendWebRequest();

                    //get all ingredients of recipe
                    using(recipeIngrReq = UnityWebRequest.Get("http://192.168.178.34:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(recipeIngrQuery)))
                    {
                        recipeIngrReq.SetRequestHeader("Accept", "application/sparql-results+json");
                        yield return recipeIngrReq.SendWebRequest();

                        switch (allSubsReq.result)
                        {
                            case UnityWebRequest.Result.Success:
                                try
                                {
                                    recipeSubsRaw = Encoding.Default.GetString(allSubsReq.downloadHandler.data);
                                    recipeIngRaw = Encoding.Default.GetString(recipeIngrReq.downloadHandler.data);
                                    allSubsJson = JSON.Parse(recipeSubsRaw);
                                    recipeIngrJson = JSON.Parse(recipeIngRaw);
                                    if (allSubsJson.Count == 0 || recipeIngrJson.Count == 0)
                                    {
                                        ingredientRecsText.text = "Ingredients couldn't be found.";
                                        Debug.Log("No ingredients or substitute products found.");
                                    }
                                    else
                                    {
                                        ingredientRecsText.text = "Successfully loaded " + recipeIngrJson["results"]["bindings"].Count + " ingredients.";

                                        RecipeUIData.instance.SetIngredientSegments(recipeIngrJson["results"]["bindings"], allSubsJson["results"]["bindings"]);
                                        Debug.Log("Recipe ingredients successfully loaded\n" + recipeIngRaw + "\nsubstitute products successfully loaded\n" + recipeSubsRaw);
                                    }
                                }
                                catch(Exception e)
                                {
                                    ingredientRecsText.text =  "An error occured: " + e.Message;
                                    Debug.LogError(e);
                                }
                                break;
                            default:
                             ingredientRecsText.text = "Ingredients can't be loaded. Try again later.";
                             Debug.Log(allSubsReq.error + ", " + recipeIngrReq.error);
                             break;
                        }
                    }
                }
            }
        }
    }
}
