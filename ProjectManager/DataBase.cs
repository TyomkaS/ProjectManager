using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    [Serializable]
    class DataBase
    {
        //Задача этого класса это хранение, изменение и предоставление данных о пользователях и проектах

        UserBank users;
        ProjectBank projects;

        public DataBase()
        {
            this.users = new UserBank();
            this.projects = new ProjectBank();
        }

        public DataBase(DataBase other)
        {
            this.users = new UserBank(other.users);
            this.projects = new ProjectBank(other.projects);
        }

        public void addProject(string name, int authorId, int executorId)
        {
            User author = this.users.getUserById(authorId);
            if (author != null)
            {
                if (author.getrole() == RoleEnum.Administrator)
                {
                    User executor = this.users.getUserById(executorId);
                    if (executor != null)
                    {
                        this.projects.addProject(name, author, executor);
                    }
                    else
                    {
                        Console.WriteLine("Wrond Executor ID.There is no users in DataBase with such ID. This is impossible to execute command");
                    }
                }
                else
                {
                    //На тот случай, если автором проекта является обычный пользователь
                    Console.WriteLine($"User {author.getName() } have to be an administrator to make this operation");
                }

            }
            else
            {
                Console.WriteLine("Wrond Author ID.There is no users in DataBase with such ID. This is impossible to execute command");
            }
        }

        public void addUser(string name, RoleEnum role, string password)
        ///Добавляет нового пользователя. Необходима проверка на достаточность прав перед вызовом
        //Здесь не делается проверки на роль администратора, т.к. этот метод может быть вызван, когда хранилище пусто, и администратора не существует
        {
            this.users.addUser(name, role, password);
        }

        public User createFirstUser(string username, string pass)
        {
            return this.users.createFirstUser(username,pass);    
        }

        public bool isHasAnyUser()
        {
            return this.users.isHasAnyUser();
        }

        public User loginUser(string username, string pass)
        {
            return this.users.loginUser(username,pass);
        }
        public void modifyProject(int id, StatusEnum status, User caller)
        {
            ProjectStatus newprojectstatus = new ProjectStatus(status, DateTime.Now);
            this.projects.modify(id,newprojectstatus,caller);
        }

        public void modifyUser(int id, User caller, RoleEnum role)
        {
            this.users.modify(id, caller, role);
        }

        public void removeProject(int id, User caller)
        {
            if (caller.getrole() == RoleEnum.Administrator)
            {
                this.projects.removeProjectById(id);
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }
        }

        public void removeUser(int id, User caller)
        {
            if (caller.getrole() == RoleEnum.Administrator)
            {
                this.users.removeUserById(id);
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }

        }

        public void showprojects()
        {
            this.projects.showProjects();
        }

        public void showusers()
        {
            this.users.showUsers();
        }

        public void showUserInfo(int id, User caller)
        {
            if (caller.getrole() == RoleEnum.Administrator)
            {
                this.users.showUserInfo(id, caller);
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }
        }

        public void showProjectHistory(int id)
        {
            this.projects.showProjectHistory(id);
        }
    }
}
