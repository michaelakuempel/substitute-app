using SimpleJSON;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Collections;
using System.Net;
using UnityEngine.Networking;
using Substitute.UI;
using UnityEngine.SceneManagement;

using Static.Vars;

namespace Recipe.UIData
{
    public class RecipeUIData : MonoBehaviour
    {
        private string subsFstQuery = "PREFIX owl: <http://www.w3.org/2002/07/owl#> PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#> PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> PREFIX substitute: <http://purl.org/NonFoodKG/product-taxonomy#> PREFIX prop: <http://purl.org/NonFoodKG/substitutes#> PREFIX label: <http://purl.org/NonFoodKG/label#> PREFIX gluten: <http://purl.obolibrary.org/obo/FOODON_03420177> SELECT DISTINCT ?purposeL ?substitute ?substituteL ?value ?quantity ?quantityL ?gluteninfo ?sndSubstitute ?sndSubstituteL ?sndQuantity ?sndQuantityL ?sndValue ?sndGluteninfo WHERE {<";
        private string subsSndQuery = "> rdfs:subClassOf ?intersection.?intersection owl:intersectionOf ?restriction. ?restriction rdf:rest/rdf:first/( owl:someValuesFrom) ?purpose.?restriction rdf:first/(owl:intersectionOf) ?inner.BIND(?inner AS ?inner2)BIND(?inner AS ?inner3)OPTIONAL {?inner2 rdf:first/(owl:intersectionOf) ?first. ?inner2 rdf:rest/rdf:first/(owl:intersectionOf) ?second. ?first rdf:rest/rdf:rest/rdf:first/(owl:hasValue | owl:someValuesFrom) ?value2.?first rdf:rest+/rdf:first/(owl:someValuesFrom) ?quantity2.?first rdf:first/( owl:someValuesFrom) ?sndSubstitute.?second rdf:first/(owl:someValuesFrom) ?substitute2.?second rdf:rest/rdf:rest/rdf:first/(owl:hasValue) ?sndValue.?second rdf:rest+/rdf:first/( owl:someValuesFrom) ?sndQuantity.?sndQuantity rdfs:label ?sndQuantitylabel.filter(langMatches(lang(?sndQuantitylabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?sndQuantitylabel, 1, 1)), SUBSTR(?sndQuantitylabel, 2)) as ?sndQuantityL)?sndSubstitute rdfs:label ?sndSubstituteLabel.FILTER(langMatches(lang(?sndSubstituteLabel),'en')). BIND(CONCAT(LCASE(SUBSTR(?sndSubstituteLabel, 1, 1)), SUBSTR(?sndSubstituteLabel, 2)) as ?sndSubstituteL)BIND(?sndSubstitute AS ?proddd2) BIND(?sndSubstitute AS ?proddd3) BIND(?sndSubstitute AS ?proddd4)BIND(?sndSubstitute AS ?proddd5) OPTIONAL {?proddd2 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_high; owl:someValuesFrom gluten: ].BIND('has high gluten compound' AS ?sndGluteninfo).}OPTIONAL {?proddd3 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_low; owl:someValuesFrom gluten: ].BIND('has low gluten compound' AS ?sndGluteninfo).}OPTIONAL {?proddd4 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound; owl:someValuesFrom gluten:].BIND('has gluten compound' AS ?sndGluteninfo).}OPTIONAL {?proddd5 rdfs:subClassOf ?prddd.?prddd owl:onProperty label:has_label.?prddd owl:someValuesFrom ?xy.BIND('glutenfree' AS ?sndGluteninfo).}} OPTIONAL {?inner3  rdf:rest/rdf:rest/rdf:first/(owl:hasValue | owl:someValuesFrom) ?valueOne.?inner3 rdf:rest+/rdf:first/(owl:someValuesFrom) ?quantityOne.?inner3 rdf:first/( owl:someValuesFrom) ?substituteOne. } BIND(?value2 AS ?value)BIND(?substitute2 AS ?substitute)BIND(?quantity2 AS ?quantity)OPTIONAL {BIND(?valueOne AS ?value)BIND(?substituteOne AS ?substitute)BIND(?quantityOne AS ?quantity)}?prod rdfs:subClassOf* ?substitute.BIND(?prod AS ?prod2)BIND(?prod AS ?prod3)BIND(?prod AS ?prod4)BIND(?prod AS ?prod5)OPTIONAL {OPTIONAL {?prod2 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_high; owl:someValuesFrom gluten: ].BIND('has high gluten compound' AS ?gluteninfo).}OPTIONAL {?prod3 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_low; owl:someValuesFrom gluten:].BIND('has low gluten compound' AS ?gluteninfo).}OPTIONAL {?prod4 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound;owl:someValuesFrom gluten:].BIND('has gluten compound' AS ?gluteninfo).}OPTIONAL {?prod5 rdfs:subClassOf ?pr.?pr owl:onProperty label:has_label.?pr owl:someValuesFrom ?glutenLabel.?glutenLabel rdfs:label ?glutenfree. BIND('glutenfree' AS ?glutenfreetext).BIND(?glutenfreetext AS ?gluteninfo)FILTER(langMatches(lang(?glutenfree),'en')).}}?purpose rdfs:label ?purposelabel.filter(langMatches(lang(?purposelabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?purposelabel, 1, 1)), SUBSTR(?purposelabel, 2)) as ?purposeL)?substitute rdfs:label ?substituteLabel.FILTER(langMatches(lang(?substituteLabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?substituteLabel, 1, 1)), SUBSTR(?substituteLabel, 2)) as ?substituteL)?quantity rdfs:label ?quantityLabel.filter(langMatches(lang(?quantityLabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?quantityLabel, 1, 1)), SUBSTR(?quantityLabel, 2)) as ?quantityL)FILTER NOT EXISTS {?otherSub rdfs:subClassOf ?intersection. ?directSub rdfs:subClassOf ?otherSub. FILTER (?otherSub != ?directSub)}}";
        private string subsTrdQuery = "> rdfs:subClassOf ?intersection.?intersection owl:intersectionOf ?restriction. ?restriction rdf:rest/rdf:first/( owl:someValuesFrom) ?purpose.?restriction rdf:first/(owl:intersectionOf) ?inner.BIND(?inner AS ?inner2)BIND(?inner AS ?inner3)OPTIONAL {?inner2 rdf:first/(owl:intersectionOf) ?first. ?inner2 rdf:rest/rdf:first/(owl:intersectionOf) ?second. ?first rdf:rest/rdf:rest/rdf:first/(owl:hasValue | owl:someValuesFrom) ?value2.?first rdf:rest+/rdf:first/(owl:someValuesFrom) ?quantity2.?first rdf:first/( owl:someValuesFrom) ?sndSubstitute.?second rdf:first/(owl:someValuesFrom) ?substitute2.?second rdf:rest/rdf:rest/rdf:first/(owl:hasValue) ?sndValue.?second rdf:rest+/rdf:first/( owl:someValuesFrom) ?sndQuantity.?sndQuantity rdfs:label ?sndQuantitylabel.filter(langMatches(lang(?sndQuantitylabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?sndQuantitylabel, 1, 1)), SUBSTR(?sndQuantitylabel, 2)) as ?sndQuantityL)?sndSubstitute rdfs:label ?sndSubstituteLabel.FILTER(langMatches(lang(?sndSubstituteLabel),'en')). BIND(CONCAT(LCASE(SUBSTR(?sndSubstituteLabel, 1, 1)), SUBSTR(?sndSubstituteLabel, 2)) as ?sndSubstituteL)BIND(?sndSubstitute AS ?proddd2) BIND(?sndSubstitute AS ?proddd3) BIND(?sndSubstitute AS ?proddd4)BIND(?sndSubstitute AS ?proddd5) OPTIONAL {?proddd2 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_high; owl:someValuesFrom gluten: ].BIND('has high gluten compound' AS ?sndGluteninfo).}OPTIONAL {?proddd3 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_low; owl:someValuesFrom gluten: ].BIND('has low gluten compound' AS ?sndGluteninfo).}OPTIONAL {?proddd4 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound; owl:someValuesFrom gluten:].BIND('has gluten compound' AS ?sndGluteninfo).}OPTIONAL {?proddd5 rdfs:subClassOf ?prddd.?prddd owl:onProperty label:has_label.?prddd owl:someValuesFrom ?xy.BIND('glutenfree' AS ?sndGluteninfo).}} OPTIONAL {?inner3  rdf:rest/rdf:rest/rdf:first/(owl:hasValue | owl:someValuesFrom) ?valueOne.?inner3 rdf:rest+/rdf:first/(owl:someValuesFrom) ?quantityOne.?inner3 rdf:first/( owl:someValuesFrom) ?substituteOne. } BIND(?value2 AS ?value)BIND(?substitute2 AS ?substitute)BIND(?quantity2 AS ?quantity)OPTIONAL {BIND(?valueOne AS ?value)BIND(?substituteOne AS ?substitute)BIND(?quantityOne AS ?quantity)}?prod rdfs:subClassOf* ?substitute.BIND(?prod AS ?prod2)BIND(?prod AS ?prod3)BIND(?prod AS ?prod4)BIND(?prod AS ?prod5)OPTIONAL {OPTIONAL {?prod2 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_high; owl:someValuesFrom gluten: ].BIND('has high gluten compound' AS ?gluteninfo).}OPTIONAL {?prod3 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound_low; owl:someValuesFrom gluten:].BIND('has low gluten compound' AS ?gluteninfo).}OPTIONAL {?prod4 rdfs:subClassOf [a owl:Restriction;owl:onProperty prop:hasCompound;owl:someValuesFrom gluten:].BIND('has gluten compound' AS ?gluteninfo).}OPTIONAL {?prod5 rdfs:subClassOf ?pr.?pr owl:onProperty label:has_label.?pr owl:someValuesFrom ?glutenLabel.?glutenLabel rdfs:label ?glutenfree. BIND('glutenfree' AS ?glutenfreetext).BIND(?glutenfreetext AS ?gluteninfo)FILTER(langMatches(lang(?glutenfree),'en')).}}?purpose rdfs:label ?purposelabel.filter(langMatches(lang(?purposelabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?purposelabel, 1, 1)), SUBSTR(?purposelabel, 2)) as ?purposeL)?substitute rdfs:label ?substituteLabel.FILTER(langMatches(lang(?substituteLabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?substituteLabel, 1, 1)), SUBSTR(?substituteLabel, 2)) as ?substituteL)?quantity rdfs:label ?quantityLabel.filter(langMatches(lang(?quantityLabel),'en')).BIND(CONCAT(LCASE(SUBSTR(?quantityLabel, 1, 1)), SUBSTR(?quantityLabel, 2)) as ?quantityL)}";
        string subsQuery;
        private UnityWebRequest subsReq;
        private JSONNode jsonSubs;
        string rawSubs;
        public TextMeshProUGUI recipeNameText;
        public TextMeshProUGUI substituteRecsText;
        public TextMeshProUGUI infoSubText;
        public Text instructionsText;
        public RectTransform ingredientContainer;
        public GameObject ingredientPrefab;
        public GameObject substituteList;
        public GameObject infoPanel;
        public RectTransform reloadIngr;
        public static RecipeUIData instance;
        private List <GameObject> ingredientSegments = new List<GameObject>();
        private string goHome = "CategoryListScreen";

        void Awake()
        {
            instance = this;
        }

        /*
        * create a ingredient segment
        */
        GameObject CreateNewIngredientSegment(JSONNode ingredientRecords, JSONNode substituteRecords)
        {
            GameObject segmentIngredient = Instantiate(ingredientPrefab);
            segmentIngredient.transform.SetParent(ingredientContainer.transform);
            ingredientSegments.Add(segmentIngredient);
            int indexIng = ingredientSegments.IndexOf(segmentIngredient);
            infoSubText.text = "Click on 'Show Substitute' to see if there are any substitutes for an ingredient available.";
            // calls function when clicked to display all substitutes for clicked ingredient
            segmentIngredient.transform.Find("SubstituteButton").GetComponent<Button>().onClick.AddListener(()=>{RecipeUIData.instance.GetSubstitute(substituteRecords, ingredientRecords, indexIng);});
            segmentIngredient.SetActive(false);
            infoPanel.SetActive(false);
            return segmentIngredient;
        }

        /*
        * display JSONNode records as ingredient segments with name. value and unit
        */
        public void SetIngredientSegments(JSONNode ingredientRecords, JSONNode substituteRecords)
        {
            foreach(GameObject ingSeg in ingredientSegments)
            {
                ingSeg.SetActive(false);
            }

            for (int i = 0; i <ingredientRecords.Count; ++i)
            {
                // create a new ingredient segment if there aren't as much segments as recipe records
                GameObject segmentIngredient = i < ingredientSegments.Count ? ingredientSegments[i] : CreateNewIngredientSegment(ingredientRecords, substituteRecords);
                segmentIngredient.SetActive(true);
                // get ingredient components
                TextMeshProUGUI ingredient = segmentIngredient.transform.Find("Ingredient").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI unitValue = segmentIngredient.transform.Find("UnitValue").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI unit = segmentIngredient.transform.Find("Unit").GetComponent <TextMeshProUGUI>();
                //set ingredient component text
                ingredient.text = ingredientRecords[i]["label"]["value"];
                unitValue.text = ingredientRecords[i]["unitValue"]["value"];
                string unitLabel = ingredientRecords[i]["unitLabel"]["value"];
                unit.text = unitLabel;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(reloadIngr);
            ingredientContainer.sizeDelta = new Vector2(ingredientContainer.sizeDelta.x, GetContainerHeight(ingredientRecords.Count));
        }

        /*
        * set container height of prefabs depending on recipe name and spacing
        */
         float GetContainerHeight(int countedSubs)
         {
            float height = 0.0f;
            height += countedSubs * (ingredientPrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
            height += countedSubs * ingredientContainer.GetComponent<VerticalLayoutGroup>().spacing;
            return height;
         }

        /*
        * set text of detailed recipe information like recipe instructions or links
        */
        public void SetRecipeDetails(string recipeName, JSONNode recipeDetailsJson)
        {
            string instructions = recipeDetailsJson[0]["instructions"]["value"];
            string seeAlso = recipeDetailsJson[0]["seeAlso"]["value"];
            GameObject.Find("SeeAlsoButton").GetComponent<Button>().onClick.AddListener(()=>{  UnityEngine.Application.OpenURL(seeAlso);});
            recipeNameText.text = recipeName;
            instructionsText.text = instructions;
         }

        /*
        * get values of substitutes of clicked ingredient
        */
        void GetSubstitute(JSONNode substituteRecords, JSONNode ingredientRecords, int indexIng)
        {
            infoSubText.gameObject.SetActive(false);
            substituteRecsText.text = "Loading Substitutes.";

            substituteRecsText.gameObject.SetActive(false);
            substituteList.SetActive(false);

            //get values of ingredient
            string ingLabel = ingredientRecords[indexIng]["label"]["value"];
            string ingDbRef = ingredientRecords[indexIng]["DbXRef"]["value"];
            string ingIri = ingredientRecords[indexIng]["iri"]["value"];
            string ingUnitLabel = ingredientRecords[indexIng]["unitLabel"]["value"];
            float ingValue = ingredientRecords[indexIng]["unitValue"]["value"];

            StaticVars.ingredientIndex = indexIng;

            string ingLabelLow = ingLabel.ToLower();
            string ingDbRefLow = ingDbRef.ToLower();
            string ingIriLow = ingIri.ToLower();

            // get all values of substitutes of selected ingredient
            for (int i = 0; i < substituteRecords.Count; ++i)
            {
                string subLabel = substituteRecords[i]["label"]["value"];
                string subDbRef = substituteRecords[i]["DbXRef"]["value"];
                string subIri = substituteRecords[i]["iri"]["value"];

                string subLabelLow = subLabel.ToLower();
                string subDbRefLow = subDbRef.ToLower();
                string subIriLow = subIri.ToLower();
                //compare label of ingredient and substitute, iri or database reference to find same products of both ontologies
                if (subLabelLow.Equals(ingLabelLow) || subDbRefLow.Equals(ingDbRefLow) || subIriLow.Equals(ingIriLow))
                {
                    string foundSubsIri = substituteRecords[i]["iri"]["value"];
                    string encodedSub = Uri.EscapeDataString(foundSubsIri);
                    substituteList.SetActive(true);
                    substituteRecsText.gameObject.SetActive(true);

                    StaticVars.ingLabel = ingLabel;
                    StaticVars.foundSubsIri = foundSubsIri;
                    StaticVars.ingUnitLabel = ingUnitLabel;
                    StaticVars.ingValue = ingValue;
                    StaticVars.ingredientRecords = ingredientRecords;

                    StartCoroutine(SubstitutesData(ingLabel,foundSubsIri,ingUnitLabel,ingValue,ingredientRecords));
                }
            }

            bool isActive = substituteList.activeSelf;
            if (isActive == false)
            {
                substituteRecsText.gameObject.SetActive(true);
                substituteRecsText.text = "No Substitutes found.";
            }

        }

        /*
        * close or open info panel depending on active status
        */
        public void OpenCloseInfoPanel ()
        {
            if (infoPanel != null)
            {
                bool isActive = infoPanel.activeSelf;
                if (isActive == true)
                {
                    infoPanel.SetActive(false);
                }
                else
                {
                    infoPanel.SetActive(true);
                }
            }
        }

        public void GoHome ()
        {
            SceneManager.LoadScene(goHome);
        }

        /*
        * get data of substitutes
        * create JSONNode with response data if request successful and valid
       */
        public IEnumerator SubstitutesData(string ingr, string foundSub, string ingrUnit, float ingrVal, string ingrRec)
        {
            if (string.IsNullOrEmpty(foundSub))
            {
                Debug.Log("Found substitute is not valid.");
            }
            else
            {
                if (foundSub == "http://purl.org/NonFoodKG/product-taxonomy#ClarifiedButter" || foundSub == "http://purl.org/NonFoodKG/product-taxonomy#ButterGhee")
                {
                    subsQuery = subsFstQuery + foundSub + subsSndQuery;
                }
                subsQuery = subsFstQuery + foundSub + subsTrdQuery;
                subsReq = UnityWebRequest.Get("http://localhost:7200/repositories/substitute-app?query=" + Uri.EscapeDataString(subsQuery));

                subsReq.SetRequestHeader("Accept", "application/sparql-results+json");
                yield return subsReq.SendWebRequest();

                switch (subsReq.result)
                {
                    case UnityWebRequest.Result.Success:

                        try
                        {
                            rawSubs = Encoding.Default.GetString(subsReq.downloadHandler.data);
                            jsonSubs = JSON.Parse(rawSubs);
                            if (jsonSubs.Count == 0)
                            {
                                Debug.Log("No Substitutes Found.");
                            }
                            else
                            {
                                substituteRecsText.text = jsonSubs["results"]["bindings"].Count + " substitutes found for " + ingr + ".";
                                SubstituteUI.instance.SetSubstituteSegments(ingr, jsonSubs["results"]["bindings"], ingrUnit, ingrVal, ingrRec);
                                Debug.Log("Substitutes successfully loaded!");
                            }
                        }
                       catch (Exception e)
                       {
                           Debug.LogError(e);
                       }
                       break;
                   default:
                       Debug.Log("No connection for request can be established.");
                      break;
                }
            }
        }
    }
}
