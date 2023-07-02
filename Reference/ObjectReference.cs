using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGXP.Reference;
public class ObjectReference<T>
{
    internal string name;
    internal int id;

    public T GetObject() {
        // placeholder
        return (T) new object();

        // return ReferenceManager.Get<T>(this.id);
    }
}

// not necessary if using generics
//internal enum ReferenceType {
//    GAME_OBJECT = 0,
//    ASSET = 1
//}

