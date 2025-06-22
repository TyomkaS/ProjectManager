using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    [Serializable]
    class User
    {
        private int id;
        private string name;
        private RoleEnum role;
        private string password;

        public User(int id, string name, RoleEnum role, string password)
        {
            this.id = id;
            this.name = name;
            this.role = role;
            this.password = password;
        }

        public User(User other, int id)
        {
            this.id = id;
            this.name = other.name;
            this.role = other.role;
            this.password = other.password;
        }

        public int getID() { return this.id; }
        public string getName() { return this.name; }
        public RoleEnum getrole() { return this.role; }
        public string getpass(User inforequirer)
        {
            return(inforequirer.getrole() == RoleEnum.Administrator ? (this.password):(null));    
        }

        public bool isCorrectPassword(string pass)
            /// Проверяет правильность пароля
            // Используется при регистрации пользователя
        {
            return this.password.Equals(pass) ? true : false;
        }

        public void print()
        {
            Console.WriteLine("ID -" + this.id.ToString() + "| User Name-" + this.name + "| User Role-" + this.role);
        }

        public void printfullinfo(User caller)
        {
            if (caller.getrole() == RoleEnum.Administrator)
            {
                Console.WriteLine("ID -" + this.id.ToString() + "| User Name-" + this.name + "| User Role-" + this.role + "| User Pass-" + this.password);
            }
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

        public void setRole(User infochanger, RoleEnum newRole)
        {
            if (infochanger.getrole() == RoleEnum.Administrator)
            {
                this.role = newRole;
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }
        }

        public void setPassword(User infochanger, string newpassword)
        {
            if (infochanger.getrole() == RoleEnum.Administrator)
            {
                this.password = newpassword;
            }
            else
            {
                Console.WriteLine("You have to be an administrator to make this operation ");
            }
        }
    }
}
