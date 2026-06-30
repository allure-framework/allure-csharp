using System.Threading;
using System.Threading.Tasks;

namespace Allure.Xunit.Internal;

static class Extensions
{
    extension (Task task)
    {
        public void SpinWait()
        {
            var spin = new SpinWait();

            while (!task.IsCompleted)
            {
                spin.SpinOnce();
            }

            task.GetAwaiter().GetResult();
        }
    }

    extension<T> (Task<T> task)
    {
        public T SpinWait()
        {
            var spin = new SpinWait();

            while (!task.IsCompleted)
            {
                spin.SpinOnce();
            }

            return task.GetAwaiter().GetResult();
        }
    }
}