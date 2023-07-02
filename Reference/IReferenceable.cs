using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGXP.Reference; 

public interface IReferenceable<T>
{
    public ObjectReference<T> GetReference();
}
