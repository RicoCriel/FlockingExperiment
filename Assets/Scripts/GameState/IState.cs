using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface IState
{
    void Enter();
    void Update();
    void Exit();
}
