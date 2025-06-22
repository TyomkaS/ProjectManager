using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    class ProjectManager
    {
        //Задача этого класса взаимодействие с пользователем,
        //т.е. получение команд от пользователя и вызов соответсвующего метода у базы данных

        private Loader loader;
        private Saver saver;
        private string usercommand;
        private User loggeduser;
        private DataBase db;
        private bool isDBchanged;
        private bool isLoadError;
        private bool isQuit;

        public ProjectManager()
        {
            this.usercommand = new string("");

            this.loader = new Loader();
            this.saver = new Saver();
            this.isDBchanged = false;

            if (!this.loader.load())
            {
                switch (ProjectManager.askSimpleQuestion("The Applicationt could not open DataBase. Would you like to create new?(y/n)"))
                {
                    case Answer.yes:
                        Console.WriteLine("Eneter DataBase name (You may eneter path to Database including name)");
                        this.loader.DBPath=Console.ReadLine();
                        this.db = new DataBase();
                        Console.WriteLine("You've created new DataBase.You have to create new user to continue work. This will be first user and admnistrator");
                        this.createFirstUser();
                        break;
                    case Answer.no:
                        Console.WriteLine("You can not use Application without DataBase.The Application will be closed");
                        this.isLoadError = true;
                        break;
                    default:
                        Console.WriteLine("You have enetered uncorrect answer.You can not use Application without DataBase. The Application will be closed");
                        this.isLoadError = true;
                        break;
                }
            }
            else
            {
                this.db = this.loader.getDB();
                if (this.db!=null)
                {
                    if (!this.db.isHasAnyUser())
                    {
                        switch (ProjectManager.askSimpleQuestion("This DataBase has not any user. Would you like to became first user and admnistrator?(y/n)"))
                        {
                            case Answer.yes:
                                this.createFirstUser();
                                break;
                            case Answer.no:
                                Console.WriteLine("You can not use DataBase without user.The Application will be closed");
                                this.isLoadError = true;
                                break;
                            default:
                                Console.WriteLine("You have eneteret uncorrect answer.You can not use DataBase without user.The Application will be closed");
                                this.isLoadError = true;
                                break;
                        }

                    }
                    else
                    {
                        this.login();
                        if (this.loggeduser==null)
                        {
                            Console.WriteLine("Sorry.You've entered wrong name or password.");
                            Answer ans = ProjectManager.askSimpleQuestion("Would you like to enter one more time?(y/n)");
                            while (this.loggeduser == null && ans==Answer.yes)
                            {
                                this.login();
                                if (this.loggeduser == null)
                                {
                                    ans = ProjectManager.askSimpleQuestion("Would you like to enter one more time?(y/n)");
                                }
                            }

                            if (this.loggeduser == null)
                            {
                                Console.WriteLine("Sorry.You could not login.Application will be closed.");
                                this.isLoadError = true;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Sorry.The Application couldn't opened DataBase. The Application will be closed");
                    this.isLoadError = true;
                }
                
            }
            this.isQuit = false;
        }

        public void run()
        {
            //В работы метода run используется схема условно бесконечного цикла,
            //до тех пор, пока пользователь не нажмёт клавиши, для выхода из цикла.
            //В самом методе обрабатываются команды пользовател

            while (!this.isQuit && !this.isLoadError)
            {

                Console.WriteLine("Enter command");
                usercommand = Console.ReadLine();

                switch (usercommand)
                {
                    case "":
                    case "add project":
                        this.addProject();
                        break;
                    case "add user":
                        this.adduser();
                        break;
                    case "modify project":
                        this.modifyProject();
                        break;
                    case "modify user":
                        this.modifyUser();
                        break;
                    case "remove project":
                        this.removeproject();
                        break;
                    case "remove user":
                        this.removeuser();
                        break;
                    case "show projects":
                        this.db.showprojects();
                        break;
                    case "show project history":
                        this.showprojecthistory();
                        break;
                    case "show users":
                        this.db.showusers();
                        break;
                    case "show user info":
                        this.showuserinfo();
                        break;
                    case "show commands":
                        this.showcommands();
                        break;
                    case "Q":
                        //Этот case нужен только для того, чтобы не попасть в поле default
                    case "q":
                        this.quit();
                        this.isQuit = true;
                        Console.WriteLine("You have entered an exit command. Good bye");
                        break;
                    default:
                        Console.WriteLine("You have entered unknown command. Please try again");
                        Console.WriteLine("If you do not know command. Please enter show commands");
                        break;
                }
            }

            if (this.isLoadError)
            {
                Console.WriteLine("This application could not be loaded because the configuration file was not found or could not be read");
            }
        }

        public static Answer askSimpleQuestion(string question)
        {
            Console.WriteLine(question);
            string answer = Console.ReadLine();
            switch (answer)
            {
                case "y":
                    return Answer.yes;
                case "n":
                    return Answer.no;
                default:
                    return Answer.def;
            }

        }

        private void addProject()
        {
            Console.WriteLine("Enter Project Name");
            string name = Console.ReadLine();
            Console.WriteLine("Enter Executor ID");
            string id = Console.ReadLine();

            if (int.TryParse(id, out int executorId))
            {
                this.db.addProject(name, this.loggeduser.getID(), executorId);
                this.isDBchanged = true;
            }
            else
            {
                Console.WriteLine("You have entered uncorrected Executor ID");
            }    
        }

        private void adduser()
        {
            if (this.loggeduser.getrole() == RoleEnum.Administrator)
            {
                Console.WriteLine("Enter User Name");
                string name = Console.ReadLine();

                Console.WriteLine("Enter User Password");
                string pass = Console.ReadLine();

                Console.WriteLine("Enter User Role (Administrator/Employee)");
                string role = Console.ReadLine();

                switch (role)
                {
                    case "Administrator":
                        this.db.addUser(name, RoleEnum.Administrator, pass);
                        this.isDBchanged = true;
                        break;
                    case "Employee":
                        this.db.addUser(name, RoleEnum.Employee, pass);
                        this.isDBchanged = true;
                        break;
                    default:
                        Console.WriteLine("You have entered uncorrected User Role");
                        break;
                }
            }
            else
            {
                Console.WriteLine("You have to be administrator to execute this command");
            }
            this.usercommand = "";
        }

        private void changeconfig()
        {
            switch (ProjectManager.askSimpleQuestion("Would you like save this DataBase as basic?(y/n)"))
            {
                case Answer.yes:
                    this.saver.saveconfig(this.loader.DBPath);
                    Console.WriteLine("Config file changed. The Application will been closed");
                    break;
                case Answer.no:
                    Console.WriteLine("As you wish.The Application will been closed");
                    break;
                    Console.WriteLine("You have entered uncorrect answer.The Application will been closed");
                default:
                    break;
            }
        }

        private void createFirstUser()
        {
            Console.WriteLine("Enter your Name");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter your Password");
            string userPass = Console.ReadLine();
            this.loggeduser = this.db.createFirstUser(userName, userPass);
            this.isDBchanged = true;
        }

        private void login()
        {
            Console.WriteLine("Enter your Name");
            string userName = Console.ReadLine();
            Console.WriteLine("Enter your Password");
            string userPass = Console.ReadLine();
            this.loggeduser=this.db.loginUser(userName, userPass);
        }

        private void modifyProject()
        {
            Console.WriteLine("Enter Project ID");
            string id = Console.ReadLine();

            if (int.TryParse(id, out int projectId))
            {
                Console.WriteLine("Enter Project status (To_do/In_progress/Done)");
                string status = Console.ReadLine();
                switch (status)
                {
                    case "To_do":
                        this.db.modifyProject(projectId, StatusEnum.To_do, this.loggeduser);
                        this.isDBchanged = true;
                        break;
                    case "In_progress":
                        this.db.modifyProject(projectId, StatusEnum.In_progress, this.loggeduser);
                        this.isDBchanged = true;
                        break;
                    case "Done":
                        this.db.modifyProject(projectId, StatusEnum.Done, this.loggeduser);
                        this.isDBchanged = true;
                        break;
                    default:
                        Console.WriteLine("You have entered uncorrected Project status");
                        break;
                }
                
            }
            else
            {
                Console.WriteLine("You have entered uncorrected Project ID");
            }
            //this.usercommand = "";
        }

        private void modifyUser()
        {
            Console.WriteLine("Enter User ID");
            string id = Console.ReadLine();

            if (int.TryParse(id, out int userId))
            {
                Console.WriteLine("Enter User status (Administrator/Employee)");
                string status = Console.ReadLine();
                switch (status)
                {
                    case "Administrator":
                        this.db.modifyUser(userId,this.loggeduser,RoleEnum.Administrator);
                        this.isDBchanged = true;
                        break;
                    case "Employee":
                        this.db.modifyUser(userId, this.loggeduser, RoleEnum.Employee);
                        this.isDBchanged = true;
                        break;
                    default:
                        Console.WriteLine("You have entered uncorrected User status");
                        break;
                }
            }
            else
            {
                Console.WriteLine("You have entered uncorrected User ID");
            }
            //this.usercommand = "";

        }

        private void removeproject()
        {
            Console.WriteLine("Enter Project ID");
            string id = Console.ReadLine();

            if (int.TryParse(id, out int projectId))
            {
                this.db.removeProject(projectId, this.loggeduser);
                this.isDBchanged = true;
            }
            else
            {
                Console.WriteLine("You have entered uncorrected Project ID");
            }
            //this.usercommand = "";
        }

        private void removeuser()
        {
            Console.WriteLine("Enter User ID");
            string id = Console.ReadLine();

            if (int.TryParse(id, out int userId))
            {
                this.db.removeUser(userId, this.loggeduser);
                this.isDBchanged = true;
            }
            else
            {
                Console.WriteLine("You have entered uncorrected User ID");
            }
            //this.usercommand = "";
        }

        private void showprojecthistory()
        {
            Console.WriteLine("Enter Project ID");
            string id = Console.ReadLine();

            if (int.TryParse(id, out int projectId))
            {
                this.db.showProjectHistory(projectId);
            }
            else
            {
                Console.WriteLine("You have entered uncorrected Project ID");
            }
        }

        private void showuserinfo()
        {
            Console.WriteLine("Enter User ID");
            string id = Console.ReadLine();

            if (int.TryParse(id, out int userId))
            {
                this.db.showUserInfo(userId, this.loggeduser);
            }
            else
            {
                Console.WriteLine("You have entered uncorrected Project ID");
            }
        }
        private void showcommands()
        {
            Console.WriteLine("");
            Console.WriteLine("=========================================================================================");
            Console.WriteLine("Commands list:");
            Console.WriteLine("1  - add project \t\t- for adding new project (administrators rights required)");
            Console.WriteLine("2  - add user  \t\t\t- for adding new users (administrators rights required)");
            Console.WriteLine("3  - modify project \t\t- for changing project status");
            Console.WriteLine("4  - modify user \t\t- for changing user data  (administrators rights required)");
            Console.WriteLine("5  - remove project \t\t- for changing user data  (administrators rights required)");
            Console.WriteLine("6  - remove user \t\t- for changing user data  (administrators rights required)");
            Console.WriteLine("7  - show projects \t\t- for show project list");
            Console.WriteLine("8  - show project history \t- for show project history");
            Console.WriteLine("9  - show users  \t\t- for show users list");
            Console.WriteLine("10 - show user info\t\t- for show user info (administrators rights required)");
            Console.WriteLine("11 - show commands \t\t- for show command list");
            Console.WriteLine("12 - q or Q  \t\t\t- for exit");
            Console.WriteLine("=========================================================================================");
        }

        private void quit()
        {
            this.usercommand = "q";
            if (this.loader.isConfigeerror())           //Проверка с какой базой данных работал пользователь
            {
                if (this.loader.isManualDBPath())
                {
                    if (this.loader.isSuccsesDBLoad())
                    {
                        switch (ProjectManager.askSimpleQuestion("You've opened not basic DatabaBase. Would you like save it as basic?(y/n)"))
                        {
                            case Answer.yes:
                                this.saver.saveconfig(this.loader.DBPath);
                                break;
                            case Answer.no:
                                Console.WriteLine("As you wish. The Application will been closed");
                                break;
                            default:
                                Console.WriteLine("You have entered uncorrect answer.");
                                break;
                        }
                    }
                    else
                    {
                        switch (ProjectManager.askSimpleQuestion("You've created new DatabaBase. Would you like save it?(y/n)"))
                        {
                            case Answer.yes:
                                if (this.saver.savedb(this.db, this.loader.DBPath))
                                {
                                    this.changeconfig();
                                }
                                else
                                {
                                    bool exitCircleRule = false;
                                    Console.WriteLine("Unfortuantly the Application couldn't save this DataBase.");
                                    Answer answer = ProjectManager.askSimpleQuestion("Would you like save this DataBase one more time?(y/n)");
                                    while (!exitCircleRule && answer==Answer.yes)
                                    {
                                        Console.WriteLine("Eneter DataBase name (You may eneter path to Database including name)");
                                        this.loader.DBPath = Console.ReadLine();

                                        if (this.saver.savedb(this.db, this.loader.DBPath))
                                        {
                                            this.changeconfig();
                                            exitCircleRule = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Unfortuantly the Application couldn't save this DataBase.");
                                            answer = ProjectManager.askSimpleQuestion("Would you like save this DataBase one more time?(y/n)");
                                        }
                                    }
                                }
                                break;
                            case Answer.no:
                                Console.WriteLine("As you wish.The Application will been closed");
                                break;
                            default:
                                Console.WriteLine("You have entered uncorrect answer.The Application will been closed");
                                break;
                        }
                    }
                }
            }

            if (this.isDBchanged)
            {
                //Вариант, когда загрузка прошла автоматически, без сбоев
                switch (ProjectManager.askSimpleQuestion("You've changed some data in DatabaBase. Would you save it?(y/n)"))
                {
                    case Answer.yes:
                        this.saver.savedb(this.db, this.loader.DBPath);
                        break;
                    case Answer.no:
                        Console.WriteLine("As you wish.The Application will been closed");
                        break;
                    default:
                        Console.WriteLine("You have entered uncorrect answer.The Application will been closed");
                        break;
                }
            }
        }
    }
}
