using Microsoft.CSharp;
using Microsoft.Win32;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace ExeHide.Models
{
    static class GeneralModel
    {
        public static void GenerateExecutableSharpCode(string filepathHidden, string launcherOutPath, string password)
        {
            string[] temp = filepathHidden.Split('\u005c');
            filepathHidden = String.Join("/", temp);

            CodeCompileUnit Compiler = new CodeCompileUnit();
            CompilerParameters Cparams = new CompilerParameters();

            CodeNamespace ProtectedLauncher = new CodeNamespace("ProtectedLauncher");
            ProtectedLauncher.Imports.Add(new CodeNamespaceImport("System"));
            ProtectedLauncher.Imports.Add(new CodeNamespaceImport("System.IO"));
            ProtectedLauncher.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));

            Compiler.Namespaces.Add(ProtectedLauncher);

            CodeTypeDeclaration Program = new CodeTypeDeclaration();
            Program.Name = "Program";
            Program.IsClass = true;
            Program.Attributes = MemberAttributes.Public;
            ProtectedLauncher.Types.Add(Program);

            CodeEntryPointMethod Main = new CodeEntryPointMethod();
            Main.Name = "Main";
            Main.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            #region sourceCode
            CodeSnippetExpression expPassword = new CodeSnippetExpression($"string password = \"{password}\";");
            CodeSnippetExpression expReadPassword = new CodeSnippetExpression("string input = Console.ReadLine();");
            CodeSnippetExpression expIf = new CodeSnippetExpression("if(input == password) {");
            CodeSnippetExpression exp1 = new CodeSnippetExpression("System.Diagnostics.Process process = new System.Diagnostics.Process();");
            CodeSnippetExpression exp2 = new CodeSnippetExpression($"process.StartInfo.FileName = \"{filepathHidden}\";");
            CodeSnippetExpression exp3 = new CodeSnippetExpression("process.Start(); }");

            CodeExpressionStatement cesPassword = new CodeExpressionStatement(expPassword);
            CodeExpressionStatement cesRead = new CodeExpressionStatement(expReadPassword);
            CodeExpressionStatement cesIf = new CodeExpressionStatement(expIf);
            CodeExpressionStatement ces1 = new CodeExpressionStatement(exp1);
            CodeExpressionStatement ces2 = new CodeExpressionStatement(exp2);
            CodeExpressionStatement ces3 = new CodeExpressionStatement(exp3);

            Main.Statements.Add(cesPassword);
            Main.Statements.Add(cesRead);
            Main.Statements.Add(cesIf);
            Main.Statements.Add(ces1);
            Main.Statements.Add(ces2);
            Main.Statements.Add(ces3);

            Program.Members.Add(Main);
            #endregion

            Cparams.ReferencedAssemblies.Add("System.dll"); // System, System.Net, etc namespaces

            Cparams.GenerateInMemory = false;
            Cparams.GenerateExecutable = true;

            Cparams.MainClass = "ProtectedLauncher.Program";

            Cparams.OutputAssembly = launcherOutPath;

            CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });

            CompilerResults results = provider.CompileAssemblyFromDom(Cparams, Compiler);
            //MessageBox.Show(results.TempFiles.BasePath);
            if (results == null || results.Errors.Count > 0)
            {
                foreach (var a in results.Errors)
                    MessageBox.Show(a.ToString());
            }
            else MessageBox.Show("Done!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void SetPassword(string password)
        {
            string ExeFile = "";
            string HiddenExeFile = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files(*.exe) | *.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                ExeFile = openFileDialog.FileName;
            }

            if (ExeFile.Contains(".exe"))
            {
                HiddenExeFile = ExeFile.Replace(".exe", "") + "hidd.exe";
                File.Move(ExeFile, HiddenExeFile);
                File.SetAttributes(HiddenExeFile, FileAttributes.Hidden);
                GenerateExecutableSharpCode(HiddenExeFile, ExeFile, password);
            }
        }

        public static void UnsetPassword()
        {
            string ExeFile = "";
            string HiddenExeFile = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files(*.exe) | *.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                ExeFile = openFileDialog.FileName;
            }
            HiddenExeFile = ExeFile.Replace(".exe", "hidd.exe");
            if (File.Exists(HiddenExeFile))
            {
                File.Delete(ExeFile);
                File.Move(HiddenExeFile, ExeFile);
                File.SetAttributes(ExeFile, FileAttributes.Normal);
            }
            else
            {
                MessageBox.Show("File not found!");
            }
        }
    }
}
