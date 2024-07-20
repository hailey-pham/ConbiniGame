using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConbiniGame.Scripts
{
    internal static class Debug
    {
        public static void Assert(bool cond, string msg)
        {
#if DEBUG
            if (!cond)
            {
                GD.PrintErr("Assert Failed: " + msg);
                throw new ApplicationException("Assert Failed: " + msg);
            }
#endif
        }
    }
}
