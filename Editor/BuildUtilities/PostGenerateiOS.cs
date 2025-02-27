using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// Edit the UnityAppController.mm to send notification when Unity is about to load.
/// </summary>
public static class PostGenerateiOS
{
    private const string UnityAppControllerPath = "/Classes/UnityAppController.mm";
    private const string UnityStartMethodName = "startUnity";
    private const string UnityWillStartNotificationName = "UnityWillStart";
    private const string WillStartIdentifier = "WillStartNotifier";

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
    {
        if (target != BuildTarget.iOS)
            return;

        var appControllerPath = Path.Join(pathToBuildProject, UnityAppControllerPath);
        EditFile(appControllerPath,
            code =>
            {
                if (code.Contains(WillStartIdentifier))
                    return code;

                var marker = $"// [{WillStartIdentifier}] --- AUTO-GENERATED, DO NOT MODIFY";
                return PrependMethod(code, UnityStartMethodName, 4, new[]
                {
                    marker,
                    CreateNotifier(UnityWillStartNotificationName),
                    marker
                });
            });
    }

    private static string PrependMethod(string source, string methodName, int indentLevel, string[] linesToAdd)
    {
        var regex = new Regex($@"-\s*\(\s*.*\s*\)\s*{methodName}\s*:.*\s*{{");
        var match = regex.Match(source);
        if (!match.Success)
        {
            return source;
        }


        var appendIndex = match.Index + match.Length;
        var codeToAppend = "\n" + string.Join("\n",
            linesToAdd.Select(l => string.Concat(Enumerable.Repeat(" ", indentLevel).Append(l))));
        var modifiedSource = source.Insert(appendIndex, codeToAppend);

        return modifiedSource;
    }

    private static string CreateNotifier(string notificationName, string obj = "self")
    {
        return $"[[NSNotificationCenter defaultCenter] postNotificationName: @\"{notificationName}\" object:{obj}];";
    }

    private static void EditFile(string path, Func<string, string> handler)
    {
        var code = File.ReadAllText(path);
        var newCode = handler(code);
        File.WriteAllText(path, newCode);
    }
}