using SimpleJSON;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Substitute.UI
{
    public class SubstituteUI : MonoBehaviour
    {
        public RectTransform substituteContainer;
        public GameObject substitutePrefab;
        public TextMeshProUGUI substituteRecsText;
        private List<GameObject> substituteSegments = new List<GameObject>();
        public static SubstituteUI instance;

        void Awake()
        {
            instance = this;
        }

        /*
        * create a category segment
        */
        GameObject CreateNewSubstituteSegment(JSONNode substituteRecords, JSONNode ingredientRecords, float ingValue, string ingUnitLabel)
        {
            // instantiate the segment from prefab
            GameObject segmentSubstitute = Instantiate(substitutePrefab);
            segmentSubstitute.transform.SetParent(substituteContainer.transform);
            substituteSegments.Add(segmentSubstitute);
            int indexSub = substituteSegments.IndexOf(segmentSubstitute);
            segmentSubstitute.SetActive(false);
            return segmentSubstitute;
        }

        /*
        * display JSONNode records as substitute segments
        */
        public void SetSubstituteSegments(string ingLabel, JSONNode substituteRecords, string ingUnitLabel, float ingValue, JSONNode ingredientRecords)
        {
            foreach (GameObject segmentSubstitute in substituteSegments)
            {
                segmentSubstitute.SetActive(false);
            }

            for (int i = 0; i <substituteRecords.Count; ++i)
            {
                // create a substitute segment if there aren't as much segments as substitute records
                GameObject substituteSegment = i <substituteSegments.Count ? substituteSegments[i] : CreateNewSubstituteSegment(substituteRecords, ingredientRecords, ingValue, ingUnitLabel);
                substituteSegment.SetActive(true);

                // get substitute component
                TextMeshProUGUI substituteFst = substituteSegment.transform.Find("SubstituteFst").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI unitFst = substituteSegment.transform.Find("UnitFst").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI valueFst = substituteSegment.transform.Find("UnitValueFst").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI purpose = substituteSegment.transform.Find("Purpose").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI substituteSnd = substituteSegment.transform.Find("SubstituteSnd").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI unitSnd = substituteSegment.transform.Find("UnitSnd").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI valueSnd = substituteSegment.transform.Find("UnitValueSnd").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI glutenFst = substituteSegment.transform.Find("GlutenFst").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI glutenSnd = substituteSegment.transform.Find("GlutenSnd").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI divider = substituteSegment.transform.Find("Divider").GetComponent<TextMeshProUGUI>();

                // multiply ingredient values with substitute values
                float unitValSub = ingValue * substituteRecords[i]["value"]["value"];
                float unitValSubSnd = ingValue * substituteRecords[i]["sndValue"]["value"];

                // set text of substitute component if one product substitutes one ingredient
                purpose.text = substituteRecords[i]["purposeL"]["value"];
                unitFst.text = ingUnitLabel;
                glutenFst.text = substituteRecords[i]["gluteninfo"]["value"];
                valueFst.text = unitValSub.ToString();
                substituteFst.text = substituteRecords[i]["substituteL"]["value"];

                // if two products substitute one ingredient then set substitute component text for a second product
                substituteSnd.text = substituteRecords[i]["sndSubstituteL"]["value"];
                glutenSnd.text = substituteRecords[i]["sndGluteninfo"]["value"];
                valueSnd.text = unitValSubSnd.ToString();
                unitSnd.text = ingUnitLabel;
                divider.text ="&";

                string valueNull = valueSnd.text;
                if (valueNull.Equals("0"))
                {
                     valueSnd.text="";
                     unitSnd.text = "";
                     divider.text ="";
                     substituteSnd.text = "";
                     glutenSnd.text = "";
                }
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(substituteContainer);
            substituteContainer.sizeDelta = new Vector2(substituteContainer.sizeDelta.x, GetContainerHeight(substituteRecords.Count));
        }

       /*
       * set container height of prefabs depending on category name and spacing
       */
        float GetContainerHeight(int countedSubs)
        {
            float height = 0.0f;
            // set overall height of substitute segment
            height += countedSubs * (substitutePrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
            // set spacing between substitute segments
            height += countedSubs * substituteContainer.GetComponent<VerticalLayoutGroup>().spacing;
            return height;
        }
    }
}