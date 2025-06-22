using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjectManager
{
    class Loader
    {
        //Класс отвечает за загрузку баз данных при запуске приложения

        private string configpath;                  //путь к файлу конфигурации
                                                    //файл конфигурации сделан, чтобы была возможность задавать разные адреса в базе данных
                                                    //и сохранять эти адреса для загрузки

        private string dbpath;                      //путь к базе данных (получается из файла "config.cfg")
        private bool isManualDBpath;                //переменная, для запоминания состояния вводился ли путь к БД автоматически
        private bool isSuccsessDBLoad;              //переменная, для запомнинания успешности состояния загрузки базыданных
        private bool isNewDB;                       //переменная, которая хранит новая база данных, или загруженная
        private bool configerror;

        BinaryFormatter formatter;

        DataBase db;

        public Loader()
        {
            this.configpath = "config.txt";
            this.dbpath = null;
            this.configerror = false;
            this.isManualDBpath = false;
            this.isSuccsessDBLoad = false;

            this.db = null;

            this.formatter = new BinaryFormatter();
        }

        public string DBPath
        {
            get { return this.dbpath; }
            set { this.dbpath = value; }
        }

        public bool IsNewDB
        {
            get { return this.isNewDB; }
            set { this.isNewDB = value; }
        }


        //public string getDBpath() { return this.dbpath };

        public DataBase getDB()
        {
            if (this.db!=null)
            {
                return this.db;
            }
            return null;
        }

        public bool isConfigeerror() { return this.configerror; }

        public bool isManualDBPath() { return this.isManualDBpath; }

        public bool isSuccsesDBLoad() { return this.isSuccsessDBLoad; }

        public bool load()
        {
            if (this.loadConfigError())
            {
                return this.loadConfigErrorHandler();
            }
            else
            {
                if (this.loadDataBaseAuto())
                {
                    return true;
                }
                else
                {
                    this.configerror = true;
                    return this.loadConfigErrorHandler();
                }
            }
        }

        private bool loadDataBase(string path)
        {
            FileStream dbstream = null;
            try
            {
                dbstream = new FileStream(path, FileMode.Open);
                
                this.db = (DataBase)formatter.Deserialize(dbstream);
                this.isSuccsessDBLoad = true;
            }
            catch (Exception)
            {
                Console.WriteLine("Error.Opening databaese unsuccsessful.");
                this.isSuccsessDBLoad = false;
            }
            finally
            {
                dbstream?.Close();
            }

            return this.isSuccsessDBLoad;
        }

        private bool loadConfigError()
            ///загружает путь к базе данных. Вернёт true если загрузка не удалась
        {
            FileStream cfgstream = null;
            try
            {
                //Попытка открыть файл "config.cfg"
                cfgstream = File.OpenRead(this.configpath);
                byte[] file = new byte[cfgstream.Length];
                cfgstream.Read(file, 0, file.Length);
                this.dbpath = System.Text.Encoding.Default.GetString(file);
                Console.WriteLine("DataBase path is " + dbpath);
            }
            catch (Exception)
            {
                this.configerror = true;
            }
            finally
            {
                cfgstream?.Close();    
            }

            return this.configerror;
        }

        private bool loadConfigErrorHandler()
        {
            switch (ProjectManager.askSimpleQuestion("Error.DataBase not found. Would you like enter database path manually?(y/n)"))
            {
                case Answer.yes:
                    this.isManualDBpath = true;
                    return this.loadDataBaseManual();
                case Answer.no:
                    Console.WriteLine("Error.DataBase not found.");
                    return false;
                default:
                    Console.WriteLine("You have entered uncorrect answer.");
                    return false;
            }
        }

        private bool loadDataBaseAuto()
        {
            return this.loadDataBase(this.dbpath);
        }

        private bool loadDataBaseManual()
        {
            Console.WriteLine("Enter database path");
            this.dbpath = Console.ReadLine();
            return this.loadDataBase(this.dbpath);
        }
    }
}
