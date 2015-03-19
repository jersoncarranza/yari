using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Json;
using System.Collections;

namespace yari.Models
{
    public class UsuarioEventArgs : EventArgs
    {
        public IList<Usuario> Results { get; private set; }
        public UsuarioEventArgs(IList<Usuario> results)
        {
            this.Results = results;
        }

    }

    public class UsuarioLoginEventArgs:EventArgs
    {
        public bool Results { get; private set; }

        public UsuarioLoginEventArgs(bool results)
        {
            this.Results = results;
        }

    }

}
