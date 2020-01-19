namespace InversionOfControl.UnitTests
{
    public interface ITestType { }

    public class TestType : ITestType
    {
        public string TestString { get; set; }
    }

    public class A
    {
        public readonly B B;
        public readonly C C;

        public A(B b, C c)
        {
            B = b;
            C = c;
        }
    }

    public class B
    {
        public readonly D D;
        public readonly E E;

        public B(D d, E e)
        {
            D = d;
            E = e;
        }
    }

    public class C
    {
        public readonly F F;
        public readonly G G;

        public C(F f, G g)
        {
            F = f;
            G = g;
        }
    }

    public class D { }

    public class E { }

    public class F { }

    public class G { }
}