using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager
{
    [Serializable]
    class ProjectStatus
    {
        //Задача этого класса хранить информацию о статусе проекта и времени изменения статуса
        private StatusEnum status;
        private DateTime time;

        public ProjectStatus(StatusEnum status, DateTime time)
        {
            this.status = status;
            this.time = time;
        }

        public ProjectStatus(ProjectStatus otherstatus)
        {
            this.status = otherstatus.status;
            this.time = otherstatus.time;
        }

        public StatusEnum getStatus() { return this.status; }
        public DateTime getTime() { return this.time; }
    }
}
