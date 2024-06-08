using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class ExpressionEvaluatorWindow : EditorWindow
{
    private string equation = "2(2+1) + (3 + 2)x";
    private float xValue = 21;
    private string result = "";

    [MenuItem("Tools/Expression Evaluator")]
    public static void ShowWindow()
    {
        GetWindow<ExpressionEvaluatorWindow>("Expression Evaluator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Evaluate Expression", EditorStyles.boldLabel);

        equation = EditorGUILayout.TextField("Equation", equation);
        xValue = EditorGUILayout.FloatField("Value of x", xValue);

        if (GUILayout.Button("Evaluate"))
        {
            var variables = new Dictionary<string, float> { { "x", xValue } };
            float evalResult = ExampleScript.EvaluateExpression(equation, variables);
            result = evalResult.ToString();
        }

        GUILayout.Label("Result: " + result);
    }
}
