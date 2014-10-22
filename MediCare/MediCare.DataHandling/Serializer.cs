using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


/**
 * @Author: Frank
 * @version: 1.0
 * This class Serializes objects. U can sthrow in everything you want. It does currently not check if the file exists already.
 * That may be a future feature.
 * 
 * Tested fully operational.
 *
 */

namespace MediCare.DataHandling
{
    [Serializable()]
    public class Serializer
    {
        public Serializer()
        {
        }

        public void SerializeObject(string filename, object objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public object DeSerializeObject(string filename)
        {
            object objectToSerialize;
            Stream stream = File.Open(filename, FileMode.Open);
            if (stream.Length < 1)
            {
                stream.Close();
                return null;
            }
            else
            {
                BinaryFormatter bFormatter = new BinaryFormatter();
                objectToSerialize = bFormatter.Deserialize(stream);
                stream.Close();
                return objectToSerialize;
            }
        }
    }
}
