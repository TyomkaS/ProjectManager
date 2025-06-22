using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjectManager
{
    class Saver
    {
        //класс отвечает за сохранение данных в файл
        private string configpath;
        private bool isBasicDBchanged;
        BinaryFormatter formatter;

        public Saver()
        {
            this.configpath = "config.txt";
            this.isBasicDBchanged = false;
            this.formatter = new BinaryFormatter();
        }

        public bool getIsBasicDBchanged() { return this.isBasicDBchanged; }
        public void saveconfig(string dbpath)
        {
            File.Delete(this.configpath);   //файл удалается перед записью новых данных (очистка)

            FileStream cfgstream = new FileStream(this.configpath, FileMode.OpenOrCreate);
            //byte[] path = System.Text.Encoding.UTF8.GetBytes(dbpath);
            byte[] path = System.Text.Encoding.ASCII.GetBytes(dbpath);
            cfgstream.Write(path, 0, path.Length);
            cfgstream.Close();

            this.isBasicDBchanged = true;
        }

        public bool savedb(DataBase db, string path)
        {
            FileStream dbstream = null;
            bool isSaveSuccesfully = false;
            try
            {
                dbstream = new FileStream(path, FileMode.OpenOrCreate);
                this.formatter.Serialize(dbstream, db);
                isSaveSuccesfully = true;
            }
            catch (Exception)
            {
                isSaveSuccesfully = false;

            }
            finally
            {
                dbstream?.Close();
            }

            return isSaveSuccesfully;
        }
    }
}
