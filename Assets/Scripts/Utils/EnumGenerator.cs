#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace Utils
{
    /// <summary>
    /// Generates an enum type using a given enumerable list of strings. Creates/Overrides the file in the specified
    /// path.
    /// </summary>
    public class EnumGenerator
    {
        /// <summary>
        /// The path to the file containing the enum.
        /// </summary>
        private string _path;

        /// <summary>
        /// The name of the file.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The namespace. Null if the namespace will be ignored.
        /// </summary>
        private string _nameSpace;

        private int _tabNumber;
        private string Tab => new string('\t', _tabNumber);

        private StreamWriter _writer;

        /// <summary>
        /// <para>
        /// REQUIRED!
        /// </para>
        /// Sets the path, the enum-type name and adds the namespace, if wanted.
        /// </summary>
        /// <param name="path">The path of the enum-file.</param>
        /// <param name="fileName">The name of the file that will be created/overwritten.</param>
        /// <param name="addNameSpace">Whether to add a namespace, enabled by default.</param>
        /// <exception cref="ArgumentException">The path must end with "/".</exception>
        public EnumGenerator(string path, string fileName, bool addNameSpace = true)
        {
            if (!path.EndsWith("/"))
                throw new ArgumentException("Path must end with \"/\".");
            _path = path;
            _fileName = fileName;
            string pathToFile = $"{_path + _fileName}.cs";

            _writer = new StreamWriter(pathToFile, false, Encoding.UTF8);

            if (addNameSpace)
                _nameSpace = Directory.Exists(_path)
                    ? new DirectoryInfo(_path).Name
                    : Directory.CreateDirectory(_path).Name;

            AddNameSpace(_nameSpace);
        }

        /// <summary>
        /// Generates the enum elements from the given list of strings.
        /// </summary>
        /// <param name="enumEntries">Strings to be written to the file as enums.</param>
        /// <param name="enumName">The name of the enum.</param>
        public void GenerateEnums(IEnumerable<string> enumEntries, string enumName)
        {
            WriteDataToFile(enumEntries, enumName);
            // Close all brackets that are still open.
            FinishWriting();
            // Reload/Compile written file, allowing other classes to use it.
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Generates multiple enums within a class.
        /// </summary>
        /// <param name="listOfEnumEntries">List of lists of strings to be written to the file.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="enumNames">The names of the enums. Make sure they are correctly ordered.</param>
        public void GenerateEnums(IEnumerable<IEnumerable<string>> listOfEnumEntries, string className,
            string[] enumNames)
        {
            WriteDataToFile(listOfEnumEntries, className, enumNames);
            // Close all brackets that are still open.
            FinishWriting();
            // Reload/Compile written file, allowing other classes to use it.
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Adds the enums to file.
        /// </summary>
        /// <param name="enumEntries">Strings to be written to the file as enums.</param>
        /// <param name="enumName">The name of the enum.</param>
        private void WriteDataToFile(IEnumerable<string> enumEntries, string enumName)
        {
            AddEnum(enumName);
            AddEnumElements(enumEntries);
            CloseCurrent();
        }

        /// <summary>
        /// Adds the enums to file but wraps them into a class, allowing multiple enums.
        /// </summary>
        /// <param name="listOfEnumEntries">List of lists of strings to be written to the file.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="enumNames">The names of the enums. Make sure they are correctly ordered.</param>
        private void WriteDataToFile(IEnumerable<IEnumerable<string>> listOfEnumEntries, string className,
            string[] enumNames)
        {
            AddClass(className);
            int i = 0;
            foreach (IEnumerable<string> enumEntries in listOfEnumEntries)
            {
                WriteDataToFile(enumEntries, enumNames[i]);
                i++;
            }
        }

        private void AddNameSpace(string nameSpace)
        {
            _writer.WriteLine("namespace " + nameSpace);
            _writer.WriteLine("{");
            _tabNumber++;
        }

        private void AddClass(string className)
        {
            _writer.WriteLine($"{Tab}public static class {className}");
            _writer.WriteLine(Tab + "{");
            _tabNumber++;
        }

        private void AddEnum(string enumName)
        {
            _writer.WriteLine($"{Tab}public enum {enumName}");
            _writer.WriteLine(Tab + "{");
            _tabNumber++;
        }

        private void AddEnumElements(IEnumerable<string> enumEntries)
        {
            foreach (string enumEntry in enumEntries)
                _writer.WriteLine($"{Tab}\t{enumEntry},");
        }

        private void CloseCurrent()
        {
            _tabNumber--;
            _writer.WriteLine(Tab + "}");
        }

        private void FinishWriting()
        {
            int closings = _tabNumber;
            for (int i = 0; i < closings; i++)
            {
                _tabNumber--;
                _writer.WriteLine(Tab + "}");
            }

            _writer.Close();
        }
    }
}
#endif