using luanvanthacsi.Data.Extentions;
using System.ComponentModel;
using static luanvanthacsi.Data.Components.Enum;

namespace luanvanthacsi.Ultils
{
    public static class StringExtensions
    {
        public static string FormatDegree(this string degree)
        {
            string result = string.Empty;
            if (degree.IsNotNullOrEmpty())
            {
                if (degree.Equals("Tiến sĩ", StringComparison.OrdinalIgnoreCase))
                {
                    result = "TS.";
                }
                else if (degree.Equals("Phó giáo sư tiến sĩ", StringComparison.OrdinalIgnoreCase))
                {
                    result = "PGS.TS.";
                }
                else if (degree.Equals("Giáo sư", StringComparison.OrdinalIgnoreCase))
                {
                    result = "GS.";
                }
            }
            return result;
        }

        public static string GetDegreeFee(this EvaluationRole role)
        {
            string result = string.Empty;
            if (role == EvaluationRole.CounterAttack)
            {
                result = "500,000";
            }
            else if (role == EvaluationRole.Scientist)
            {
                result = "350,000";
            }
            else if (role == EvaluationRole.President)
            {
                result = "450,000";
            }   
            else if (role == EvaluationRole.Secretary)
            {
                result = "400,000";
            }        
            else if(role == EvaluationRole.Instructor)
            {
                result = "250,000";
            }

            return result;
        }

    }
}
