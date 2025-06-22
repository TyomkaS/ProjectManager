using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    [Serializable]
    class ProjectBank
    {
        //Задача этого класса это хранение, изменение и предоставление данных проектах
        private List<Project> projectbank;      //используется List для простоты использования
        private int lastuserID;                 //переменная нужная, что бы ID пользователя всегда было уникальным, даже, если како-то пользователь был удалён

        public ProjectBank()
        {
            this.projectbank = null;
            lastuserID = 1;
        }

        public ProjectBank(ProjectBank other)
        //Конструктор копирования нужен для загрузки из файла
        {
            this.lastuserID = other.lastuserID;
            if (other.projectbank != null)
            {
                this.projectbank = new List<Project>(other.projectbank.Count());

                for (int i = 0; i < other.projectbank.Count(); i++)
                {
                    Project tmp = new Project(other.projectbank[i], i++);
                    this.projectbank.Add(tmp);
                }
            }
            else
            { this.projectbank = null; }
        }

        public void addProject(string name, User author, User executor)
        ///Добавляет новый проект
        {
            if (author.getrole() == RoleEnum.Administrator)
            {
                if (this.projectbank == null)
                {
                    //Создаётся новый список, с первоначальной ёмкостью 1,т.к. возможно больше не потребуется 
                    this.projectbank = new List<Project>(1);

                }

                Project tmp = new Project(this.lastuserID,name,executor,author);
                this.projectbank.Add(tmp);
                this.lastuserID++;//увеличивается lastuserID т.к. добавлен ещё один пользователь
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }
        }

        /*public Project getProjectById(int id)
        Оставлен на всякий случай, вдруг пригодится
        ///Предоставляет ссылку на объект класса Project
        {
            if (this.projectbank != null)
            {
                for (int i = 0; i < this.projectbank.Count(); i++)
                {
                    if (id == this.projectbank[i].getID())
                    {
                        return this.projectbank[i];
                    }
                }
            }
            return null;
        }*/

        public void modify(int id, ProjectStatus status, User caller)
        {
            if (this.projectbank != null)
            {
                for (int i = 0; i < this.projectbank.Count(); i++)
                {
                    if (this.projectbank[i].getID()==id)
                    {
                        if (this.projectbank[i].getExecutorName().Equals(caller.getName()) || caller.getrole().Equals(RoleEnum.Administrator))
                        {
                            this.projectbank[i].modyfy(status);
                        }
                        else
                        {
                            Console.WriteLine("You are not executor of this project and you are not administrator. This is impossible to execute command");
                        }
                        return;
                    }
                }
                Console.WriteLine("Wrond Project ID.There is no project in DataBase with such ID. This is impossible to execute command");
            }
            else
            {
                Console.WriteLine("There is no projects in DataBase. This is impossible to execute command");
            }
        }

        public void removeProjectById(int id)
        ///Удаляет Project с соответсвующим id. Возврщает false, если объект не найден
        {
            if (this.projectbank != null)
            {
                for (int i = 0; i < this.projectbank.Count(); i++)
                {
                    if (id == this.projectbank[i].getID())
                    {
                        string message = "Project " + this.projectbank[i].getName() + " has been removed";
                        this.projectbank.RemoveAt(i);  //исключения не будет, т.к. в цикле последовательно перебираются элементы
                        Console.WriteLine(message);

                        if (this.projectbank.Count() == 0)
                        {
                            //если список пуст, инициализируется null
                            this.projectbank = null;
                        }
                        return;
                    }
                }

                Console.WriteLine("The projects with such ID has not found in DataBase. This is impossible to execute command");
            }
            else
            {
                Console.WriteLine("There is no projects in DataBase. This is impossible to execute command");
            }
        }

        public void showProjects()
        {
            Console.WriteLine("=========================================================================================");
            if (this.projectbank != null)
            {
                foreach (var item in this.projectbank)
                {
                    item.print();
                }
            }
            else
            {
                Console.WriteLine("There is no projects in DataBase. This is impossible to execute command");
            }
            Console.WriteLine("=========================================================================================");
        }

        public void showProjectHistory(int id)
        {
            if (this.projectbank != null)
            {
                for (int i = 0; i < this.projectbank.Count(); i++)
                {
                    if (id == this.projectbank[i].getID())
                    {
                        this.projectbank[i].showhistory();
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("There is no projects in DataBase. This is impossible to execute command");
            }

        }
    }
}
