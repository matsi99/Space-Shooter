using UnityEngine;
using UnityEditor;
using System.IO;

namespace Assets.Scripts.AI
{
    public class HandleTextFile
    {
        [MenuItem("Tools/Write file")]
        public static void WriteString(string toWrite)
        {
            string path = "Assets/AI_DATA.txt";

            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(toWrite);
            writer.Close();

            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(path);
            //TextAsset asset = (TextAsset)Resources.Load("test");

            ////Print the text from the file
            //Debug.Log(asset.text);
        }

        [MenuItem("Tools/Read file")]
        public static void ReadString()
        {
            string path = "Assets/AI_DATA.txt";

            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            Debug.Log(reader.ReadToEnd());
            reader.Close();
        }

    }
}