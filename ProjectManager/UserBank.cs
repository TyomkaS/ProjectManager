using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    [Serializable]
    class UserBank
    {
        //Задача этого класса это хранение, изменение и предоставление данных о пользователях
        private List<User> userbank;    //используется List для простоты использования
        private int lastuserID;         //переменная нужная, что бы ID пользователя всегда было уникальным, даже, если како-то пользователь был удалён 

        public UserBank()
        {
            this.userbank = null;
            lastuserID = 1;
        }

        public UserBank(UserBank other)
        //Конструктор копирования нужен для загрузки из файла
        {
            this.lastuserID = other.lastuserID;
            if (other.userbank!=null)
            {
                this.userbank = new List<User>(other.userbank.Count());

                for (int i = 0; i < other.userbank.Count(); i++)
                {
                    User tmp = new User(other.userbank[i],i++);
                    this.userbank.Add(tmp);   
                }
            }
            else
            { this.userbank = null; } 
        }

        public void addUser(string name, RoleEnum role, string password)
            ///Добавляет нового пользователя. Необходима проверка на достаточность прав перед вызовом
        //Здесь не делается проверки на роль администратора, т.к. этот метод может быть вызван, когда хранилище пусто, и администратора не существует
        {
            if (this.userbank == null)
            {
                //Создаётся новый список, с первоначальной ёмкостью 1,т.к. возможно больше не потребуется 
                this.userbank = new List<User>(1);

            }

            User tmp = new User(this.lastuserID, name, role, password);
            this.userbank.Add(tmp);
            this.lastuserID++;//увеличивается lastuserID т.к. добавлен ещё один пользователь
        }

        public User createFirstUser(string username, string pass)
        {
            if (!this.isHasAnyUser())
            {
                this.addUser(username, RoleEnum.Administrator, pass);
                return this.userbank[0];
            }
            return null;
        }

        public bool isHasAnyUser()
        {
            if (this.userbank!= null)
            {
                if (this.userbank.Count > 0)
                {
                    return true;
                }
                
            }
            return false;
        }

        public User loginUser(string username, string pass)
        {
            if (isHasAnyUser())
            {
                for (int i = 0; i < this.userbank.Count; i++)
                {
                    if (this.userbank[i].getName().Equals(username))
                    {
                        if (this.userbank[i].isCorrectPassword(pass))
                        {
                            return this.userbank[i];
                        }
                    }
                    
                }
            }
            return null;
        }

        public User getUserById(int id)
            ///Предоставляет ссылку на объект класса User
        {
            if (this.userbank != null)
            {
                for (int i = 0; i < this.userbank.Count(); i++)
                {
                    if (id == this.userbank[i].getID())
                    {
                        return this.userbank[i];
                    }
                }
            }
            return null;
        }

        public int getAdministratorCount()
        {
            if (this.userbank!=null)
            {
                int adminCount = 0;
                for (int i = 0; i < this.userbank.Count(); i++)
                {
                    if (this.userbank[i].getrole().Equals(RoleEnum.Administrator))
                    {
                        adminCount++;
                    }
                }
                return adminCount;
            }
            return 0;
        }

        public void modify(int id, User caller, RoleEnum role)
        {
            if (this.userbank != null)
            {
                for (int i = 0; i < this.userbank.Count(); i++)
                {
                    if (this.userbank[i].getID() == id)
                    {
                        if (this.userbank[i].getrole().Equals(RoleEnum.Administrator) && role.Equals(RoleEnum.Employee))
                        {
                            if (this.getAdministratorCount() > 1)
                            {
                                this.userbank[i].setRole(caller, role);
                                return;
                            }
                            else
                            {
                                //Если в базе данных остался только 1 администратор, тогда не выполнять изменение,
                                //т.к. для работы с остальными пользователями необходим хотя бы 1 администратор
                                Console.WriteLine("This User is last Administrator in DataBase. Make any other user the Administrator or Add new one before change modify this User");
                                return;
                            }
                        }
                        else
                        {
                            if (!this.userbank[i].getrole().Equals(role))
                            {
                                this.userbank[i].setRole(caller, role);
                                Console.WriteLine("User modified succsefully");
                                return;
                            }

                        }
                    }
                }
                Console.WriteLine("Wrond User ID.There is no users in DataBase with such ID. This is impossible to execute command");
            }
            else
            {
                Console.WriteLine("There is no Users in DataBase. This is impossible to execute command");
            }
        }

        public void removeUserById(int id)
        ///Удаляет User с соответсвующим id. Возврщает false, если объект не найден
        {
            if (this.userbank != null)
            {
                for (int i = 0; i < this.userbank.Count(); i++)
                {
                    if (id == this.userbank[i].getID())
                    {
                        if (this.userbank[i].getrole() == RoleEnum.Administrator)
                        {
                            if (this.getAdministratorCount() > 1)
                            {
                                string message = "User " + this.userbank[i].getName() + " has been removed";
                                this.userbank.RemoveAt(i);  //исключеия не будет, т.к. в цикле последовательно перебираются элементы
                                Console.WriteLine(message);
                            }
                            else
                            {
                                //Если в базе данных остался только 1 администратор, тогда не выполнять удаление,
                                //т.к. для работы с остальными пользователями необходим хотя бы 1 администратор
                                Console.WriteLine("This User is last Administrator in DataBase. Make any other user the Administrator or Add new one before change modify this User");
                            }
                        }
                        else
                        {
                            string message = "User " + this.userbank[i].getName() + " has been removed";
                            this.userbank.RemoveAt(i);  //исключеия не будет, т.к. в цикле последовательно перебираются элементы
                            Console.WriteLine(message);
                        }
                       
                        if (this.userbank.Count() == 0)
                        {
                            //если список пуст, инициализируется null
                            this.userbank = null;
                        }
                        return;
                    }
                }

                Console.WriteLine("The user with such ID has not found in DataBase. This is impossible to execute command");
            }
            else
            {
                Console.WriteLine("There is no users in DataBase. This is impossible to execute command");
            }
        }

        public void showUsers()
        {
            Console.WriteLine("=========================================================================================");
            if (this.userbank != null)
            {
                foreach (var item in this.userbank)
                {
                    item.print();
                }
            }
            else
            {
                Console.WriteLine("There is no user in DataBase. This is impossible to execute command");
            }
            Console.WriteLine("=========================================================================================");

        }

        public void showUserInfo(int id, User caller)
        {
            if (caller.getrole() == RoleEnum.Administrator)
            {
                if (this.userbank!=null)
                {
                    if (this.userbank.Count>0)
                    {
                        for (int i = 0; i < this.userbank.Count; i++)
                        {
                            if (this.userbank[i].getID()==id)
                            {
                                Console.WriteLine("=========================================================================================");
                                this.userbank[i].printfullinfo(caller);
                                Console.WriteLine("=========================================================================================");
                                return;
                            }
                        }
                        Console.WriteLine("=========================================================================================");
                        Console.WriteLine("The User with such ID has not found in DataBase. This is impossible to execute command");
                        Console.WriteLine("=========================================================================================");
                    }
                }
            }
        }
    }
}
