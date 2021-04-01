using System;
using System.Collections.Generic;
using System.Linq;
using RuntimeAssetImporter;
using UnityEngine;

namespace DopeElections.Answer
{
    [Serializable]
    public class SmartSpider : IAsset
    {
        public int Key => 1;

        public float axis_1;
        public float axis_2;
        public float axis_3;
        public float axis_4;
        public float axis_5;
        public float axis_6;
        public float axis_7;
        public float axis_8;

        public override string ToString()
        {
            return UnityEngine.JsonUtility.ToJson(this, true);
        }

        public bool Equals(SmartSpider spider, float delta)
        {
            bool temp_axis_1 = Mathf.Abs(axis_1 - spider.axis_1) < delta;
            bool temp_axis_2 = Mathf.Abs(axis_2 - spider.axis_2) < delta;
            bool temp_axis_3 = Mathf.Abs(axis_3 - spider.axis_3) < delta;
            bool temp_axis_4 = Mathf.Abs(axis_4 - spider.axis_4) < delta;
            bool temp_axis_5 = Mathf.Abs(axis_5 - spider.axis_5) < delta;
            bool temp_axis_6 = Mathf.Abs(axis_6 - spider.axis_6) < delta;
            bool temp_axis_7 = Mathf.Abs(axis_7 - spider.axis_7) < delta;
            bool temp_axis_8 = Mathf.Abs(axis_8 - spider.axis_8) < delta;

            return temp_axis_1 
                   && temp_axis_2
                   && temp_axis_3
                   && temp_axis_4
                   && temp_axis_5
                   && temp_axis_6
                   && temp_axis_7
                   && temp_axis_8;
        }

        public float[] Values => new[] {axis_1, axis_2, axis_3, axis_4, axis_5, axis_6, axis_7, axis_8};
        public bool HasData => axis_1 + axis_2 + axis_3 + axis_4 + axis_5 + axis_6 + axis_7 + axis_8 > 0;

        public float this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }

        private float GetValue(int index)
        {
            switch (index)
            {
                case 0: return axis_1;
                case 1: return axis_2;
                case 2: return axis_3;
                case 3: return axis_4;
                case 4: return axis_5;
                case 5: return axis_6;
                case 6: return axis_7;
                case 7: return axis_8;
                default: throw new IndexOutOfRangeException("SmartSpider has no axis " + index + "! Available: 0-7");
            }
        }

        private void SetValue(int index, float value)
        {
            switch (index)
            {
                case 0:
                    axis_1 = value;
                    break;
                case 1:
                    axis_2 = value;
                    break;
                case 2:
                    axis_3 = value;
                    break;
                case 3:
                    axis_4 = value;
                    break;
                case 4:
                    axis_5 = value;
                    break;
                case 5:
                    axis_6 = value;
                    break;
                case 6:
                    axis_7 = value;
                    break;
                case 7:
                    axis_8 = value;
                    break;
                default: throw new IndexOutOfRangeException("SmartSpider has no axis " + index + "! Available: 0-7");
            }
        }

        static public SmartSpider RecalculateSmartSpider(Dictionary<int, int>[] answerDiciotionary)
        {
            SmartSpider temp_spider = new SmartSpider();

            for (int i = 0; i < 8; i++)
            {
                int temps_axis = answerDiciotionary[i].Values.ToArray().Sum();

                int n_of_question_axis = answerDiciotionary[i].Count;
                
                n_of_question_axis *= 100;
                if (n_of_question_axis == 0)
                    continue;
                switch (i)
                {
                    case 0:
                        temp_spider.axis_1 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_1 < 0)
                            temp_spider.axis_1 = 0;
                        break;
                    case 1:
                        temp_spider.axis_2 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_2 < 0)
                            temp_spider.axis_2 = 0;
                        break;
                    case 2:
                        temp_spider.axis_3 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_3 < 0)
                            temp_spider.axis_3 = 0;
                        break;
                    case 3:
                        temp_spider.axis_4 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_4 < 0)
                            temp_spider.axis_4 = 0;
                        break;
                    case 4:
                        temp_spider.axis_5 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_5 < 0)
                            temp_spider.axis_5 = 0;
                        break;
                    case 5:
                        temp_spider.axis_6 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_6 < 0)
                            temp_spider.axis_6 = 0;
                        break;
                    case 6:
                        temp_spider.axis_7 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_7 < 0)
                            temp_spider.axis_7 = 0;
                        break;
                    case 7:
                        temp_spider.axis_8 = (float) temps_axis / n_of_question_axis;
                        if (temp_spider.axis_8 < 0)
                            temp_spider.axis_8 = 0;
                        break;
                    default:
                        break;
                }
            }

            return temp_spider;
        }
    }
}