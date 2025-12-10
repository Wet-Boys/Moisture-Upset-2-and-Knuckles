#!/usr/bin/dotnet run

#:property TargetFramework=net10.0
#:property LangVersion=preview
#:property PublishAot=false

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

Regex startRegex = new Regex(@"^\s+scriptingDefineSymbols:");
Regex endRegex = new Regex(@"^\s+additionalCompilerArguments:");

string projectSettingsPath = Path.GetFullPath(Path.Join("MoistureUpsetRemixUnity", "ProjectSettings", "ProjectSettings.asset"));
string[] lines = File.ReadAllLines(projectSettingsPath);

int start = -1;
int end = -1;
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    if (startRegex.IsMatch(line) && start == -1)
        start = i;
    if (endRegex.IsMatch(line) && end == -1 && start >= 0)
        end = i;

    if (start >= 0 && end >= 0)
        break;
}

var startChunk = lines[..start];
var endChunk = lines[end..];

string projectSettings2Path = Path.GetFullPath(Path.Join("MoistureUpsetRemixUnity", "ProjectSettings", "ProjectSettings.asset"));


lines = startChunk.Concat(["  scriptingDefineSymbols: {}"]).Concat(endChunk).ToArray();
File.WriteAllLines(projectSettings2Path, lines);
