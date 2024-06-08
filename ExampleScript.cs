using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ExampleScript : MonoBehaviour
{
    public static float EvaluateExpression(string equation, Dictionary<string, float> variables)
    {
        //replace pi with is mathematical version
        //equation = HandlePi(equation);
        // Substitute variables in trigonometric functions first
        equation = EvaluateTrigonometricFunctions(equation, variables);

        // Handle implicit multiplication
        equation = InsertImplicitMultiplication(equation);

        // Replace all other variables
        foreach (var variable in variables)
        {
            equation = equation.Replace(variable.Key, variable.Value.ToString());
        }

        // Handle nested expressions
        equation = EvaluateNestedExpressions(equation);

        // Evaluate the final equation
        if (ExpressionEvaluator.Evaluate(equation, out float result))
        {
            return result;
        }
        else
        {
            Debug.LogError("Failed to evaluate expression: " + equation);
            return float.NaN; // Return NaN if the evaluation fails
        }
    }

/*    private static string HandlePi(string equation)
    {
        // Replace any instance of π or 'pi' with Mathf.PI
        equation = equation.Replace("π", Mathf.PI.ToString());
        equation = equation.Replace("pi", Mathf.PI.ToString());

        return equation;
    }
*/
    private static string InsertImplicitMultiplication(string equation)
    {
        // Handle numbers and variables adjacent to parentheses
        equation = Regex.Replace(equation, @"([a-zA-Z])\(", "$1*("); // e.g., x(2-1) -> x*(2-1)
        equation = Regex.Replace(equation, @"(\d)\(", "$1*("); // e.g., 2(2+1) -> 2*(2+1)

        // Handle cases where there's a variable or number before a closing parenthesis
        equation = Regex.Replace(equation, @"(\))([a-zA-Z])", "$1*$2");
        equation = Regex.Replace(equation, @"(\))(\d)", "$1*$2");

        // Handle expressions like (2-1)x or x(2-1)
        equation = Regex.Replace(equation, @"(\d)([a-zA-Z])", "$1*$2"); // e.g., 2x -> 2*x
        equation = Regex.Replace(equation, @"([a-zA-Z])(\d)", "$1*$2"); // e.g., x2 -> x*2

        return equation;
    }

    private static string EvaluateTrigonometricFunctions(string equation, Dictionary<string, float> variables)
    {
        // Replace variables inside trigonometric functions
        Regex trigRegex = new Regex(@"\b(sin|cos|tan)\(([^)]+)\)");

        // Handle expressions like (2-1)x or x(2-1)
        equation = Regex.Replace(equation, @"(\d)([a-zA-Z])", "$1*$2"); // e.g., 2x -> 2*x
        equation = Regex.Replace(equation, @"([a-zA-Z])(\d)", "$1*$2"); // e.g., x2 -> x*2
        equation = Regex.Replace(equation, @"(x)([a-zA-Z])", "$1*$2"); // e.g., 2x -> 2*x
        equation = Regex.Replace(equation, @"([a-zA-Z])(x)", "$1*$2"); // e.g., x2 -> x*2
        equation = Regex.Replace(equation, @"(\))(\d)", "$1*$2");
        equation = Regex.Replace(equation, @"(\))([a-zA-Z])", "$1*$2");

        equation = trigRegex.Replace(equation, match =>
        {
            string function = match.Groups[1].Value;
            string innerExpression = match.Groups[2].Value;

            // Replace variables in the inner expression
            foreach (var variable in variables)
            {
                innerExpression = innerExpression.Replace(variable.Key, variable.Value.ToString());
            }
//THIS IS WHERE YOU CAN CHOOSE EITHER DEGREES OR RADIANS, JUST DELETE MATH.F DEG2RAD IF YOU WANT IT TO BE RADIANS
            if (ExpressionEvaluator.Evaluate(innerExpression, out float value))
            {
                switch (function)
                {
                    case "sin":
                        if (value%180<0.01){
                            return 0.ToString();
                        }else{
                            return Mathf.Sin(value * Mathf.Deg2Rad).ToString();
                        }
                    case "cos":
                        if ((value+90)%180<0.01){
                            return 0.ToString();
                        }else{
                            return Mathf.Cos(value * Mathf.Deg2Rad).ToString();
                        }
                    case "tan":
                        return Mathf.Tan(value * Mathf.Deg2Rad).ToString();
                    default:
                        return match.Value; // Return original if function not recognized
                }
            }
            else
            {
                Debug.LogError("Failed to evaluate expression inside trigonometric function: " + innerExpression);
                return "NaN";
            }
        });

        return equation;
    }

    private static string EvaluateNestedExpressions(string equation)
    {
        Regex nestedExpressionRegex = new Regex(@"\(([^()]+)\)");

        while (nestedExpressionRegex.IsMatch(equation))
        {
            equation = nestedExpressionRegex.Replace(equation, match =>
            {
                string innerExpression = match.Groups[1].Value;
                if (ExpressionEvaluator.Evaluate(innerExpression, out float innerResult))
                {
                    return innerResult.ToString();
                }
                else
                {
                    Debug.LogError("Failed to evaluate nested expression: " + innerExpression);
                    return "NaN"; // Return NaN if the evaluation fails
                }
            });
        }

        return equation;
    }
}
