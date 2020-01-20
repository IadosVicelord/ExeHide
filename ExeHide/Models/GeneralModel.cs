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
        /// <summary>
        /// Генерация исполняемого файла
        /// </summary>
        /// <param name="filepathHidden">Путь к файлу</param>
        /// <param name="launcherOutPath">Путь, куда нужно разместить сгенерированный файл</param>
        /// <param name="password">Пароль</param>
        public static void GenerateExecutableSharpCode(string filepathHidden, string launcherOutPath, string password)
        {
            //Изменение формата пути к файлу.
            string[] temp = filepathHidden.Split('\u005c');
            filepathHidden = String.Join("/", temp);

            //Объект компитялтора
            CodeCompileUnit Compiler = new CodeCompileUnit();
            CompilerParameters Cparams = new CompilerParameters();

            //Определение нового пространства имен и импортов
            CodeNamespace ProtectedLauncher = new CodeNamespace("ProtectedLauncher");
            ProtectedLauncher.Imports.Add(new CodeNamespaceImport("System"));
            ProtectedLauncher.Imports.Add(new CodeNamespaceImport("System.IO"));
            ProtectedLauncher.Imports.Add(new CodeNamespaceImport("System.Diagnostics"));

            //Добавление пространства имен в компилятор
            Compiler.Namespaces.Add(ProtectedLauncher);

            //Определение основного класса
            CodeTypeDeclaration Program = new CodeTypeDeclaration();
            Program.Name = "Program";
            Program.IsClass = true;
            Program.Attributes = MemberAttributes.Public;
            ProtectedLauncher.Types.Add(Program);

            //Определение точки входа
            CodeEntryPointMethod Main = new CodeEntryPointMethod();
            Main.Name = "Main";
            Main.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            //Определение исходного кода генерируемого файла
            #region sourceCode
            CodeSnippetExpression expPassword = new CodeSnippetExpression($"string password = \"{password}\";");
            CodeSnippetExpression expReadPassword = new CodeSnippetExpression("string input = Console.ReadLine();");
            CodeSnippetExpression expIf = new CodeSnippetExpression("if(input == password) {");
            CodeSnippetExpression newProcess = new CodeSnippetExpression("System.Diagnostics.Process process = new System.Diagnostics.Process();");
            CodeSnippetExpression setStartInfo = new CodeSnippetExpression($"process.StartInfo.FileName = \"{filepathHidden}\";");
            CodeSnippetExpression startProcess = new CodeSnippetExpression("process.Start(); }");

            CodeExpressionStatement cesPassword = new CodeExpressionStatement(expPassword);
            CodeExpressionStatement cesRead = new CodeExpressionStatement(expReadPassword);
            CodeExpressionStatement cesIf = new CodeExpressionStatement(expIf);
            CodeExpressionStatement cesNewProcess = new CodeExpressionStatement(newProcess);
            CodeExpressionStatement cesSetStartInfo = new CodeExpressionStatement(setStartInfo);
            CodeExpressionStatement cesStartProcess = new CodeExpressionStatement(startProcess);

            //Добавление строк кода в функцию Main
            Main.Statements.Add(cesPassword);
            Main.Statements.Add(cesRead);
            Main.Statements.Add(cesIf);
            Main.Statements.Add(cesNewProcess);
            Main.Statements.Add(cesSetStartInfo);
            Main.Statements.Add(cesStartProcess);

            //Добавление функции-точки входа в качестве члена класса
            Program.Members.Add(Main);
            #endregion

            //Установка параметров компилятора
            Cparams.ReferencedAssemblies.Add("System.dll"); // System, System.Net, etc namespaces
            Cparams.GenerateInMemory = false;
            Cparams.GenerateExecutable = true;
            Cparams.MainClass = "ProtectedLauncher.Program";
            Cparams.OutputAssembly = launcherOutPath;
            CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });

            //Запись результатов компиляции и их вывод
            CompilerResults results = provider.CompileAssemblyFromDom(Cparams, Compiler);
            if (results == null || results.Errors.Count > 0)
            {
                foreach (var a in results.Errors)
                    MessageBox.Show(a.ToString());
            }
            else MessageBox.Show("Done!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Заменяет оригинальный исполняемый файл на генерируемое консольное приложение с паролем
        /// </summary>
        /// <param name="password">Устанавливаемый пароль</param>
        public static void SetPassword(string password)
        {
            string ExeFile = "";
            string HiddenExeFile = "";

            //Выбор исполняемого файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe) | *.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                ExeFile = openFileDialog.FileName;
            }

            if (ExeFile.Contains(".exe"))
            {
                //Замена названия оригинального файла
                HiddenExeFile = ExeFile.Replace(".exe", "") + "hidd.exe";
                File.Move(ExeFile, HiddenExeFile);
                //Сокрытие оригинального файла
                File.SetAttributes(HiddenExeFile, FileAttributes.Hidden);
                //Генерация нового исполняемого файла
                GenerateExecutableSharpCode(HiddenExeFile, ExeFile, password);
            }
        }

        /// <summary>
        /// Возвращает оригинальный файл на прежнее место и удаляет подмененный файл
        /// </summary>
        public static void UnsetPassword()
        {
            string ExeFile = "";
            string HiddenExeFile = "";
            //Выбор исполняемого файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files(*.exe) | *.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                ExeFile = openFileDialog.FileName;
            }
            //Возвращение оригинального файла на место
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
