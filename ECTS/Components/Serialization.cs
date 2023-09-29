using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using ECTS.Objects.GameObjects;

namespace ECTS.Components
{
    /// <summary>
    /// Class for (De)-Serializing all kind of objects.
    /// Serialize single objects or the whole game state or game settings
    /// </summary>
    public sealed class Serialization
    {
        private GameLoop GameLoop { get; }
        internal Serialization(GameLoop gameLoop)
        {
            GameLoop = gameLoop;
        }

        /// <summary>
        /// Tests for existing save game.
        /// </summary>
        /// <param name="path">(optional: default "Save_Data manager.xml")</param>
        /// <returns>bool</returns>
        internal static bool ExistsSaveGame(string path = "Save_DataManager.xml")
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Serializes running game and creates .xml Files containing the game status.
        /// </summary>
        internal void SaveGame()
        {
            try
            {
                // Call all (needed) Serializing Methods:
                GameLoop.ObjectManager.Seri();
                GameLoop.RenderManager.Seri();
                GameLoop.SaveSettings();
            }
            catch (Exception)
            {
                GameLoop.RenderManager.SoundManager.PlaySound("error");
            }
            finally
            {
                GameLoop.RenderManager.SoundManager.PlaySound("save-game");
            }
        }

        /// <summary>
        /// Deserializes saved game if SaveGame exists
        /// </summary>
        internal void LoadGame()
        {
            try
            {
                //Do nothing if no SaveGame exists.
                if (!ExistsSaveGame())
                {
                    return;
                }
                // Call all (needed) Deserializing Methods:
                GameLoop.ObjectManager.Deseri();
                GameLoop.RenderManager.Deseri();
            }
            catch (Exception)
            {
                GameLoop.RenderManager.SoundManager.PlaySound("error");
            }
            finally
            {
                GameLoop.RenderManager.SoundManager.PlaySound("load-game");
            }
        }

        /// <summary>
        /// Serializes given object and saves it in given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        internal static void Serialize(string path, object obj)
        {
            var objType = obj.GetType();
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            var seri = new DataContractSerializer(objType, GameObject.GetKnownTypes(), maxItemsInObjectGraph: 9999999, ignoreExtensionDataObject: false, preserveObjectReferences: true, dataContractSurrogate: null);
            seri.WriteObject(fileStream, obj);
            fileStream.Close();
        }
        /// <summary>
        /// Creates object from given files (path) and object (Type) and returns it
        /// </summary>
        /// <param name="path"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static object Deserialize(string path, object obj)
        {
            var objType = obj.GetType();
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas());
            var seri = new DataContractSerializer(objType, GameObject.GetKnownTypes(), maxItemsInObjectGraph: 9999999, ignoreExtensionDataObject: false, preserveObjectReferences: true, dataContractSurrogate: null);
            obj = seri.ReadObject(reader, true);
            reader.Close();
            return obj;
        }
    }
}
