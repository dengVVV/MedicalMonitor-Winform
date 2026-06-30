using System;
using System.IO;
using System.Text;

class Program {
    static void Main() {
        var sb = new StringBuilder();
        sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
        sb.AppendLine(@"<Project ToolsVersion=""15.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">");
        sb.AppendLine(@"  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />");
        sb.AppendLine(@"  <PropertyGroup>");
        sb.AppendLine(@"    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>");
        sb.AppendLine(@"    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>");
        sb.AppendLine(@"    <ProjectGuid>{89AB0D60-2A69-48E2-A19B-76660E2ED4E5}</ProjectGuid>");
        sb.AppendLine(@"    <OutputType>WinExe</OutputType>");
        sb.AppendLine(@"    <RootNamespace>MedicalMonitor.WinForm</RootNamespace>");
        sb.AppendLine(@"    <AssemblyName>MedicalMonitor.WinForm</AssemblyName>");
        sb.AppendLine(@"    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>");
        sb.AppendLine(@"    <FileAlignment>512</FileAlignment>");
        sb.AppendLine(@"    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>");
        sb.AppendLine(@"    <Deterministic>true</Deterministic>");
        sb.AppendLine(@"    <LangVersion>7.3</LangVersion>");
        sb.AppendLine(@"  </PropertyGroup>");
        File.WriteAllText(@"E:\Code\C#\Medical_Device_Monitoring_Host_System\MedicalMonitor.WinForm\csproj_header.txt", sb.ToString());
        Console.WriteLine("OK");
    }
}
