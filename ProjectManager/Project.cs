using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    [Serializable]
    class Project
    {
        private int id;
        private string name;
        private string executorname;
        private string authorname;
        private List<ProjectStatus> statuslist;

        public Project(int id, string name, User executor, User author)
        {
            this.id = id;
            this.name = name;
            this.executorname = executor.getName();     //берётся поле name, чтобы сохранить имя, если пользователь удалён, или отсутсвует в БД
            this.authorname = author.getName();         //берётся поле name, чтобы сохранить имя, если пользователь удалён, или отсутсвует в БД

            ProjectStatus projectStatus = new ProjectStatus(StatusEnum.To_do, DateTime.Now);
            statuslist = new List<ProjectStatus>(3);
            statuslist.Add(projectStatus);
        }

        public Project(Project other, int id)
        {
            this.id = id;
            this.name = name;
            this.executorname = other.executorname;
            this.authorname = other.authorname;
        }

        public int getID() { return this.id; }
        public string getName() { return this.name; }

        public string  getExecutorName() { return this.executorname; }

        public string getAuthorName() { return this.authorname; }

        public void modyfy(ProjectStatus projectStatus)
            ///используется для изменения статуса проекта
        {
            statuslist.Add(projectStatus);
        }

        public void print()
        {
            Console.WriteLine("ID-" + this.id.ToString() + "| Name-" + this.name + "| ExecutorName-" + this.executorname
                                + "| AuthorName-" + this.authorname + "| Status-" + this.statuslist.Last().getStatus() + "| Changetime-" + this.statuslist.Last().getTime());
        }

        public void setName(User infochanger, string newname)
        {
            if (infochanger.getrole() == RoleEnum.Administrator)
            {
                this.name = newname;
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }
        }

        public void setExecutor(User infochanger, User executor)
        {
            if (infochanger.getrole() == RoleEnum.Administrator)
            {
                this.executorname = executor.getName();     //берётся поле name, чтобы сохранить имя, если пользователь удалён, или отсутсвует в БД
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }
        }

        public void showhistory()
        {
            Console.WriteLine("=========================================================================================");
            for (int i = 0; i < statuslist.Count(); i++)
            {
                Console.WriteLine("ID-" + this.id.ToString() + "| Name-" + this.name + "| ExecutorName-" + this.executorname
                    + "| AuthorName-" + this.authorname + "| Status-" + this.statuslist[i].getStatus() + "| Changetime-" + this.statuslist[i].getTime());
            }
            Console.WriteLine("=========================================================================================");
        }
    }
}
