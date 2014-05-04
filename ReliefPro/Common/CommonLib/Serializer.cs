using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
namespace ReliefProCommon
{
    /// <summary>
    /// 把物体serialize
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// serialize成一个memory stream
        /// </summary>
        /// <param name="objectToBeSerialized"></param>
        /// <returns></returns>
        public static MemoryStream Seriaize(object objectToBeSerialized)
        {
            MemoryStream ms =  new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter(); ;
            try
            {
                formatter.Serialize(ms, objectToBeSerialized);
            }
            catch (Exception ex)
            {
                ms = null;
                throw ex;
            }

            return ms;
        }

        /// <summary>
        /// deserialize一个memory stream 成一个物体
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <returns></returns>
        public static object Deserialize(MemoryStream serializedObject)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            serializedObject.Position = 0;
            object originalObject = null;

            try
            {
                originalObject=formatter.Deserialize(serializedObject);
            }
            catch(Exception ex)
            {
                originalObject = null;
                throw ex;
            }
            return originalObject;
        }

        /// <summary>
        /// serialize成xml格式的字符串
        /// </summary>
        /// <param name="objectToBeSerialized"></param>
        /// <returns></returns>
        public static String SeriaizeToXml(object objectToBeSerialized)
        {
            XmlSerializer serializer = new XmlSerializer(objectToBeSerialized.GetType());
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, objectToBeSerialized);
            return writer.ToString();
        }


        /// <summary>
        /// deserialize含有xml格式的字符串成一个物体
        /// </summary>
        /// <param name="stringedObject"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static object DeseriaizeFromXml(String stringedObject, Type objectType)
        {
            XmlSerializer serializer = new XmlSerializer(objectType);
            StringReader reader = new StringReader(stringedObject);

            return serializer.Deserialize(reader);
        }

        /// <summary>
        /// 把一个实体serialize成一个base64的字符串
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static String SerializeToString(object entity)
        {
            
            
            byte[] bytes = null; 
            String stringedEntity = null;

            try
            {
                MemoryStream ms = Seriaize(entity);
                ms.Position = 0;
                bytes = ms.ToArray();
                stringedEntity = System.Convert.ToBase64String(bytes);
            }
            catch (Exception ex)
            {
                stringedEntity = null;
                throw ex;
            }

            return stringedEntity;
        }

        /// <summary>
        /// deserialize一个base64的字符串成一个实体
        /// </summary>
        /// <param name="stringedEntity"></param>
        /// <returns></returns>
        public static object DeserializeFromString(String stringedEntity)
        {
            object entity = null;
            try
            {
                byte[] bytes = System.Convert.FromBase64String(stringedEntity);

                if (bytes != null)
                {
                    MemoryStream ms = new MemoryStream(bytes);
                    ms.Position = 0;
                    entity = Deserialize(ms);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return entity;

        }

        public static T Clone<T>(T objectToBeCloned)
        {
            MemoryStream ms = Seriaize(objectToBeCloned);
            object cloned = null;
            using (ms)
            {
                cloned = Deserialize(ms);
            }

            return (T)cloned;
        }
    }
}
