using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DopeElections.Localizations;
using Localizator;
using RuntimeAssetImporter;
using UnityEngine;

namespace DopeElections.Answer
{
    [Serializable]
    public class Candidate : IAsset
    {
        public int id;

        public string gender;

        public int zip;

        public string city;

        public string country;

        public int incumbent;

        public int elected;

        public string occupation;

        public string hobbies;

        public string books;

        public string music;

        public string movies;

        public string slogan;

        public string topics;

        public string firstName;

        public string lastName;

        public int birthYear;

        public string urlImage;

        public int partyId;

        public SmartSpider smartSpider;

        public string civilStatus;

        public string education;

        public string denomination;

        public int listNumber;

        public int campaignBudget;

        public string campaignBudgetComment;

        public string vestedInterests;

        public TermsInOffices[] termsInOffice;

        public ListPlace[] listPlaces;

        [NonSerialized] public Response[] responses;

        /// <summary>
        /// Match value between 0 and 1.
        /// </summary>
        [NonSerialized] public float match;

        public string matchString => Mathf.RoundToInt(match * 100).ToString();
        public string fullName => $"{firstName} {lastName.ToUpper()}";

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }

        public int Key => id;

        public void RecalculateSmartSpider()
        {
            Dictionary<int, int>[] questionThatMatter = new Dictionary<int, int>[8];
            for (int i = 0; i < questionThatMatter.Length; i++)
            {
                questionThatMatter[i] = new Dictionary<int, int>();
            }

            var user = DopeElectionsApp.Instance.User;
            if (user.Questionnaire == null)
            {
                Debug.LogError("Cannot recalculate smartspider; No questionnaire loaded.");
                return;
            }

            Question[] questions = user.Questionnaire.Questions;

            foreach (var candidateResponse in responses)
            {
                Question actualQuestion = questions.FirstOrDefault(q => q.id == candidateResponse.questionId);
                if (actualQuestion == null)
                    continue;
                foreach (var axis in actualQuestion.axis)
                {
                    int value = axis.value >= 0 ? candidateResponse.value : 100 - candidateResponse.value;
                    questionThatMatter[axis.axis - 1].Add(candidateResponse.questionId, value);
                }
            }

            SmartSpider temp = SmartSpider.RecalculateSmartSpider(questionThatMatter);
            smartSpider = temp;
            /*
            if (!smartSpider.Equals(temp,0.05f))
            {
                Debug.Log("actual election : "+DopeElectionsApp.Instance.User.ElectionId);
                Debug.Log("actual candidate : " +this.id);
                Debug.Log("Don't work! : "+temp +", origin : "+ smartSpider);
                foreach (var response in responses)
                {
                    Debug.Log("raw answer : question_id : " +response.questionId+ " value : "+response.value);

                }
                for(int i=0 ; i < 8; i++)
                {
                    var element = questionThatMatter[i];
                    foreach (var dict_elem in element)
                    {
                        Debug.Log("New dict axis_"+i+" : key :"+dict_elem.Key+" value :" +dict_elem.Value);
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    var element = tempAnswer[i];
                    foreach (var dict_elem in element)
                    {
                        Debug.Log("Old dict axis_"+i+" : key : "+dict_elem.Key + " value :" +dict_elem.Value);
                    }
                }
            }
            else
            {
                Debug.Log("it WORK !!!");
            }*/
        }

        public void RecalculateMatch()
        {
            // Debug.Log(("old match value = "+match));
            var questionnaire = DopeElectionsApp.Instance.User.Questionnaire;
            var userAnswers = questionnaire.Progression.UserAnswers;
            match = CalculateMatch(userAnswers.Where(a => questionnaire.Questions.Any(q => q.id == a.questionId)));
            // Debug.Log("new match value = "+match);
            //Debug.Log("Match is "+match);
        }
        
        public float CalculateMatch(IEnumerable<QuestionAnswer> userAnswers)
        {
            // if we want to add weighted questions, replace 1f with the weight
            return CalculateMatch(userAnswers.ToDictionary(a => a, a => 1f));
        }

        public float CalculateMatch(Dictionary<QuestionAnswer, float> userAnswers)
        {
            return CalculateMatch(userAnswers, responses);
        }

        /// <summary>
        /// Calculates match percentage based on formula from smartvote:
        /// dist_w(v,c)=sqrt(sum{n,i=1}(pow(w_i*(v_i-c_i),2)))
        /// </summary>
        /// <param name="userAnswers">Keys represent v, Values represent w</param>
        /// <param name="candidateResponses">Entries represent c</param>
        /// <returns></returns>
        public static float CalculateMatch(Dictionary<QuestionAnswer, float> userAnswers, Response[] candidateResponses)
        {
            var responseMap = userAnswers
                .Where(e => e.Key.answer >= 0) // undecided is -1, these must be excluded
                .Select(e => e.Key)
                .ToDictionary(
                    a => a,
                    a => candidateResponses.FirstOrDefault(r => r.questionId == a.questionId)
                );
            float dist = 0;
            float sqrtDist = 0;
            float maxDist = 0;
            float tempDist = 0;
            foreach (var entry in responseMap)
            {
                var response = entry.Value;
                if (response == null) continue;
                var userAnswer = entry.Key;
                var weight = userAnswers[userAnswer];
                dist += Mathf.Pow(weight * (userAnswer.answer - response.value), 2);
                sqrtDist = Mathf.Sqrt(dist);
                tempDist += Mathf.Pow(100 * weight, 2);
                var sqrtTempDist = Mathf.Sqrt(tempDist);
                if (sqrtTempDist >= maxDist)
                {
                    maxDist = sqrtTempDist;
                }
            }

            return maxDist != 0 ? (1 - sqrtDist / maxDist) : 0;
            /*
            return Mathf.Sqrt(userAnswers
                .Where(e => e.Key.answer > 0)
                .Select(e => e.Key)
                .ToDictionary(
                    a => a,
                    a => candidateResponses.FirstOrDefault(r => r.questionId == a.questionId)
                )
                .Where(e => e.Value != null && e.Value.value > 0)
                .Sum(e => Mathf.Pow(userAnswers[e.Key] * (e.Key.answer - e.Value.value), 2))
            );*/
        }
    }

    [Serializable]
    public class ListPlace
    {
        public int position;
        public string number;
    }

    [Serializable]
    public class TermsInOffices
    {
        public string dateStart;

        public string dateEnd;

        public string mandate;

        public string ToFormattedString(ILocalization localization)
        {
            var startYear = GetYear(dateStart);
            var endYear = Mathf.Max(startYear, GetYear(dateEnd));

            var startYearString = startYear.ToString();
            var endYearString = endYear == DateTime.Now.Year
                ? localization.GetString(LKey.Components.Candidate.MandateYearCurrent)
                : endYear.ToString();

            return startYearString != endYearString
                ? $"{startYearString} - {endYearString}: {mandate}"
                : $"{startYearString}: {mandate}";
        }

        private int GetYear(string dateString)
        {
            var yearRegex = new Regex("^[0-9]+");
            var match = yearRegex.Match(dateString);
            return match.Groups.Count > 0 && int.TryParse(match.Groups[0].Value, out var year) ? year : 0;
        }
    }
}