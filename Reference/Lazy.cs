//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UGXP.Reference;

//namespace UGXP; //.Reference; - for ease of use

//public abstract class Lazy {
//    internal abstract void Assign(IReferenceable value);
//    internal abstract void Revoke();
//}
//public class Lazy<T> : Lazy where T : IReferenceable
//{ 
//    internal T value;

//    internal Lazy() { }
//    internal Lazy(T value) {
//        this.value = value;
//    }

//    internal override void Assign(IReferenceable value) {
//        Console.WriteLine("Assign: " + this.GetHashCode());
//        this.value = (T) value;
//    }
//    internal override void Revoke() {
//        this.value = default;
//    }

//    public static implicit operator T(Lazy<T> lazy) {
//        if (lazy.value == null && ReferenceManager.state == ReferenceManager.State.SCENE_LOAD)
//            throw new ReferenceException("Invalid state! This object was not yet referenced");
//        if (lazy.value == null)
//            throw new ReferenceException("The reference doesn't exist!");

//        return lazy.value;
//    }

//    public static implicit operator Lazy<T>(T obj) {
//        return new Lazy<T>(obj);
//    }
//}
